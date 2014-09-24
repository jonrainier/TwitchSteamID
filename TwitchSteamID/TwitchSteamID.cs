using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LitJson;

namespace TwitchSteamID
{
    public class TwitchSubCheck
    {
        /*
	        @author: Jonathan "Pwnoz0r" Rainier
	        @description: Takes the given Steam64UID and finds the Twitch name associated with it.
	        @usage: Check https://github.com/Pwnoz0r/TwitchSteamID for usage.
	        @notes:
		        1. User joins the server.
		        2. User SteamUID is queried to Twitch API: http://api.twitch.tv/api/steam/76561198035644989
		        3. Have cron run for sub list every 24 hours.
		        4. Cross check user name with streamer sub list.
		        5. If user name is on the list, allow on the server, if not/return 404, kick.
        */
        private static string _twitchQueryURL = "http://api.twitch.tv/api/steam/$STEAMUID$";
        private static string _directoryCurrent = Directory.GetCurrentDirectory();
        private static string _fileSubList = Path.Combine(_directoryCurrent, "sublist.txt");
        public static string subListData;
        public static Dictionary<string, ulong> whitelistedTwitchUserNames = new Dictionary<string, ulong>();

        /// <summary>
        /// Reads the "sublist.txt" file
        /// </summary>
        /// <returns>All data inside of the sub list.</returns>
        public static string SubListRead()
        {
            if (!File.Exists(_fileSubList))
                File.Create(_fileSubList).Close();
            return File.ReadAllText(_fileSubList);
        }

        /// <summary>
        /// Queries the specified Steam64UID
        /// </summary>
        /// <param name="userSteam64UID">Specify a users Steam64UID</param>
        public static void QuerySteamUID(ulong userSteam64UID)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Proxy = null;
                    TwitchUserInformation.twitchRawData = wc.DownloadString(_twitchQueryURL.Replace("$STEAMUID$", userSteam64UID.ToString()));
                    JsonMapper.ToObject<TwitchUserInformation>(TwitchUserInformation.twitchRawData);
                    SubListCheck(TwitchUserInformation.name, userSteam64UID);
                }
                catch (WebException) { /*404 - Steam Account Not Connected*/ }
            }
        }

        /// <summary>
        /// Checks of the SteamUID is whitelisted.
        /// </summary>
        /// <param name="userSteam64UID">Clients Steam64UID</param>
        /// <returns>True or False</returns>
        public static bool SubListCheckStatus(ulong userSteam64UID)
        {
            if (whitelistedTwitchUserNames.ContainsValue(userSteam64UID))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Out puts the whole sublist using Console.WriteLine();
        /// </summary>
        public static void SubListOutput()
        {
            if (whitelistedTwitchUserNames.Count < 1)
                Console.WriteLine("No users have been whitelisted!");
            else
            {
                Console.WriteLine("Current whitelisted users:");
                foreach (var userData in whitelistedTwitchUserNames)
                    Console.WriteLine("{0}:{1}", userData.Key, userData.Value);
            }
        }

        private static void SubListCheck(string twitchUserName, ulong userSteam64UID)
        {
            if (subListData.Contains(twitchUserName))
                whitelistedTwitchUserNames.Add(twitchUserName, userSteam64UID);
        }
    }

    public class TwitchUserInformation
    {
        public static string twitchRawData { get; set; }
        public static int _id { get; set; }
        public static string name { get; set; }
    }
}
