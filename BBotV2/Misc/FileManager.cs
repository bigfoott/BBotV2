using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2.Misc
{
    class FileManager
    {
        public static void CheckDefaultFiles()
        {
            if (!Directory.Exists("files"))
                Directory.CreateDirectory("files");
            if (!Directory.Exists("guilds"))
                Directory.CreateDirectory("guilds");
            if (!Directory.Exists("guilds/old"))
                Directory.CreateDirectory("guilds/old");

            if (!File.Exists("files/config.json"))
                File.WriteAllText("files/config.json", "{\"token\": \"\",\"botOwnerId\": \"\"}");
        }
        
        public static void CheckGuildFiles(ulong id)
        {
            if (!Directory.Exists($"guilds/{id}"))
                Directory.CreateDirectory($"guilds/{id}");

            if (!File.Exists($"guilds/{id}/config.json"))
                File.WriteAllText($"guilds/{id}/config.json", "{\"prefix\": \".\",\"logchannel\": \"null\", \"autorole\": \"null\"}");
            if (!File.Exists($"guilds/{id}/tags.json"))
                File.WriteAllText($"guilds/{id}/tags.json", "{}");
        }
    }
}
