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

            if (!File.Exists("files/config.json"))
                File.WriteAllText("files/config.json", "{\"token\": \"\",\"botOwnerId\": \"\"}");
        }

        public static void CheckGuildFiles(ulong id)
        {
            if (!Directory.Exists($"guilds/{id}"))
                Directory.CreateDirectory($"guilds/{id}");

            if (!File.Exists($"guilds/{id}/config.json"))
                File.WriteAllText($"guilds/{id}/config.json", "{\"prefix\": \".\"}");
            if (!File.Exists($"guilds/{id}/tags.json"))
                File.WriteAllText($"guilds/{id}/tags.json", "{}");
            if (!File.Exists($"guilds/{id}/flairs.json"))
                File.WriteAllText($"guilds/{id}/flairs.json", "{}");
        }
    }
}
