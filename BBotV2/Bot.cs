using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Net.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2
{
    class Bot
    {
        public DiscordClient client;
        public InteractivityExtension inter;
        public CommandsNextExtension cnext;

        public DateTime startTime;

        public int totalUsers = 0;
        public int totalGuilds = 0;

        public Bot(string token)
        {
            var config = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Critical,
                AutoReconnect = true
            };
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.Major == 6 &&
                Environment.OSVersion.Version.Minor == 1)
                config.WebSocketClientFactory = WebSocketSharpClient.CreateNew;
            
            client = new DiscordClient(config);

            inter = client.UseInteractivity(new InteractivityConfiguration() { Timeout = new TimeSpan(0, 1, 30) });

            cnext = client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDms = false,
                EnableDefaultHelp = false,
                PrefixResolver = PrefixPredicateAsync,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true
            });

            cnext.RegisterCommands<CNext.Main>();

            client.GuildCreated += async e =>
            {
                if (Directory.Exists($"guilds/old/{e.Guild.Id}"))
                    Directory.Move($"guilds/old/{e.Guild.Id}", $"guilds/{e.Guild.Id}");

                Misc.FileManager.CheckGuildFiles(e.Guild.Id);
                
                totalGuilds = e.Client.Guilds.Count;
                totalUsers = 0;
                foreach (DiscordGuild g in e.Client.Guilds.Values.Where(g => g.Id != 264445053596991498 && g.Id != 110373943822540800))
                    totalUsers += g.MemberCount;
            };


            client.GuildDeleted += async e =>
            {
                Directory.Move($"guilds/{e.Guild.Id}", $"guilds/old/{e.Guild.Id}");

                totalGuilds = e.Client.Guilds.Count;
                totalUsers = 0;
                foreach (DiscordGuild g in e.Client.Guilds.Values)
                    totalUsers += g.MemberCount;
            };


            client.Ready += async e =>
            {
                await client.UpdateStatusAsync(new DiscordActivity("u.help • bigft.io/URBot", ActivityType.Playing));
            };


            client.GuildAvailable += async e =>
            {
                totalGuilds = e.Client.Guilds.Count;
                foreach (DiscordGuild g in e.Client.Guilds.Values.Where(g => g.Id != 264445053596991498 && g.Id != 110373943822540800))
                    totalUsers += g.MemberCount;
            };

            client.ConnectAsync();
            startTime = DateTime.Now;
        }

        private static Task<int> PrefixPredicateAsync(DiscordMessage m)
        {
            //dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{m.Channel.Guild.Id}/config.json"));

            string pref = ".";
            //pref = (string)json.prefix;

            return Task.FromResult(m.GetStringPrefixLength(pref));
        }

    }
}
