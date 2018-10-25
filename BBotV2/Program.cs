using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBotV2.Misc;

namespace BBotV2
{
    class Program
    {
        public static Bot bot;

        public static string token;
        public static ulong botOwnerId;

        static void Main(string[] args)
        {
            Log("Starting...");

            FileManager.CheckDefaultFiles();
            Log("Checked default files.");
            
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText("files/config.json"));
            if (json.token == "")
            {
                Log("Error: You need to fill out the 'token' field in 'files/config.json' before starting.\n\n&7Press any key to continue...", "&c");
                Console.ReadLine();
                return;
            }
           
            token = (string)json.token;
            botOwnerId = Convert.ToUInt64((string)json.botOwnerId);
            Log("Loaded token and bot owner ID.");

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        
        static async Task MainAsync(string[] args)
        {
            Log("MainAsync started.");
            
            Log("Creating bot object.");
            bot = new Bot(token, "you 👀", ActivityType.Watching, UserStatus.Idle, "#ff7700");
            
            await Task.Delay(-1);
        }
        
        public static void Log(string message, string color = "&7")
        {
            FConsole.WriteLine($"{color}[{DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")}] &f{message}");
        }
    }
}
