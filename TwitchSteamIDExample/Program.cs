using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchSteamID;

namespace TwitchSteamIDExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Reads the subcriber list (sublist.txt) located in the root directory of the application.
            TwitchSubCheck.subListData = TwitchSubCheck.SubListRead();
            // Query the specified Steam64UID and begin the whitelist checking process.
            TwitchSubCheck.QuerySteamUID(76561198035644989); // pwnoz0r
            // Output the current TwitchUsername:Steam64UID that are whitelisted.
            TwitchSubCheck.SubListOutput();
            // Query the specified Steam64UID and check if the client has actually been whitelisted. (Returns true/false)
            if (TwitchSubCheck.SubListCheckStatus(76561198035644989))
                Console.WriteLine("Specified Steam64UID is whitelisted!");

            Console.ReadKey();
        }
    }
}
