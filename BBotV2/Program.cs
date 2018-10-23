using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2
{
    class Program
    {
        public static Bot bot;

        public static string token;
        public static ulong botOwnerId;

        static void Main(string[] args)
        {
            Misc.FileManager.CheckDefaultFiles();

            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText("files/config.json"));
            token = (string)json.token;
            botOwnerId = Convert.ToUInt64((string)json.botOwnerId);

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            bot = new Bot(token);

            await Task.Delay(-1);
        }
    }
}
