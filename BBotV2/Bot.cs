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
using BBotV2.Misc;
using BBotV2.CNext;

namespace BBotV2
{
    class Bot
    {
        public DiscordClient client;
        public InteractivityExtension inter;
        public CommandsNextExtension cnext;

        public DateTime startTime;
        public DiscordColor embedColor;

        public int totalUsers = 0;
        public int totalGuilds = 0;

        public Bot(string token, string activity = "", ActivityType activityType = ActivityType.Playing, UserStatus status = UserStatus.Online, string embedColor = "#4f545c")
        {
            this.embedColor = new DiscordColor(embedColor);
            Program.Log("Initialized embed color.");

            var config = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Critical,
                AutoReconnect = true
            };
            Program.Log("Initialized config object.");
            
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.Major == 6 &&
                Environment.OSVersion.Version.Minor == 1)
            {
                config.WebSocketClientFactory = WebSocketSharpClient.CreateNew;
                Program.Log("Switched websocket to WebSocketSharp. (Windows 7)");
            }
            
            client = new DiscordClient(config);
            Program.Log("Initialized client.");

            inter = client.UseInteractivity(new InteractivityConfiguration() { Timeout = new TimeSpan(0, 1, 30) });
            Program.Log("Initialized interactivity extension.");

            cnext = client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDms = false,
                EnableDefaultHelp = false,
                PrefixResolver = PrefixPredicateAsync,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true
            });

            cnext.RegisterCommands<General>();
            cnext.RegisterCommands<Tag>();
            cnext.RegisterCommands<Config>();

            Program.Log("Initialized cnext extension.");
            
            client.GuildCreated += async e =>
            {
                if (Directory.Exists($"guilds/old/{e.Guild.Id}"))
                    Directory.Move($"guilds/old/{e.Guild.Id}", $"guilds/{e.Guild.Id}");
                FileManager.CheckGuildFiles(e.Guild.Id);

                UpdateCount();
            };
            client.GuildDeleted += async e =>
            {
                Directory.Move($"guilds/{e.Guild.Id}", $"guilds/old/{e.Guild.Id}");

                UpdateCount();
            };
            client.GuildAvailable += async e =>
            {
                FileManager.CheckGuildFiles(e.Guild.Id);
                UpdateCount();
            };
            client.Ready += async e =>
            {
                await client.UpdateStatusAsync(new DiscordActivity(activity, activityType), status);

                startTime = DateTime.Now;

                Program.Log("Setup completed.");
            };
            Program.Log("Subscribed to events.");

            Program.Log("Connecting..."); 
            client.ConnectAsync();
            Program.Log("Connected.");
        }

        private static Task<int> PrefixPredicateAsync(DiscordMessage m)
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{m.Channel.Guild.Id}/config.json"));

            string pref = ".";
            pref = (string)json.prefix;
            
            return Task.FromResult(m.GetStringPrefixLength(pref));
        }

        private void UpdateCount()
        {
            totalGuilds = client.Guilds.Count;
            totalUsers = 0;
            foreach (DiscordGuild g in client.Guilds.Values)
                totalUsers += g.MemberCount;
        }

        public async Task SendError(CommandContext ctx, string title, string message)
        {
            var embed = new DiscordEmbedBuilder()
            {
                Title = title,
                Description = "**ERROR**\n\n" + message,
                Color = DiscordColor.DarkRed
            };

            var msg = await ctx.RespondAsync("", embed: embed);
            await Task.Delay(new TimeSpan(0, 0, 5));
            await msg.DeleteAsync();
        }
    }
}
