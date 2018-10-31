using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2.Misc
{
    class Logger
    {
        public static Dictionary<ulong, ulong> logChannels;

        private static bool init;
        
        public static void Init(DiscordClient client)
        {
            logChannels = new Dictionary<ulong, ulong>();
            foreach (ulong id in client.Guilds.Keys) UpdateLogChannel(id);
            init = true;
        }
        
        public static void UpdateLogChannel(ulong guildID)
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{guildID}/config.json"));
            if (logChannels.ContainsKey(guildID)) logChannels.Remove(guildID);
            if (UInt64.TryParse((string)json.logchannel, out ulong id)) logChannels.Add(guildID, id);
            else logChannels.Add(guildID, 0);
        }
        
        public static async Task LogCommand(CommandExecutionEventArgs e)
        {
            if (!init || logChannels[e.Context.Guild.Id] == 0) return;

            var embed = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = e.Context.Member.Username + "#" + e.Context.Member.Discriminator + $" ({e.Context.Member.Id})",
                    IconUrl = e.Context.Member.AvatarUrl,
                },
                Timestamp = DateTime.Now,
                Color = new DiscordColor("#f4dc41"),
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = "Message ID: " + e.Context.Message.Id
                },
                Description = $"**Command executed in {e.Context.Channel.Mention}:**\n{e.Context.Message.Content}"
            };

            await e.Context.Client.SendMessageAsync(e.Context.Guild.GetChannel(logChannels[e.Context.Guild.Id]), "", embed: embed);
        }

        public static async Task LogDeletedMessage(MessageDeleteEventArgs e)
        {
            if (!init || logChannels[e.Guild.Id] == 0 || Program.bot.startTime > e.Message.CreationTimestamp || e.Message.Author.IsBot) return;

            string deleter = "";

            if (Perms.BotHasGuildPerm(e.Guild, Permissions.ViewAuditLog))
            {
                var audit = (await e.Guild.GetAuditLogsAsync(limit: 1, action_type: AuditLogActionType.MessageDelete)).FirstOrDefault();
                if ((DateTime.Now - audit.CreationTimestamp).TotalSeconds < 1) deleter = $" by {audit.UserResponsible.Mention}";
            }
            
            var embed = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = e.Message.Author.Username + "#" + e.Message.Author.Discriminator + $" ({e.Message.Author.Id})",
                    IconUrl = e.Message.Author.AvatarUrl,
                },
                Timestamp = DateTime.Now,
                Color = new DiscordColor("#bc5400"),
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = "Message ID: " + e.Message.Id
                },
                Description = $"**Message sent by {e.Message.Author.Mention} deleted in {e.Channel.Mention}{deleter}:**\n{e.Message.Content}"
            };

            
            if (e.Message.Attachments.Count > 0)
            {
                string _s = "s";
                if (e.Message.Attachments.Count == 1)
                    _s = "";

                string a = "";
                foreach (DiscordAttachment att in e.Message.Attachments)
                    a += att.FileName + "\n";

                embed.AddField("Attachment" + _s, a);
            }

            
            await e.Client.SendMessageAsync(e.Guild.GetChannel(logChannels[e.Guild.Id]), "", embed: embed);
        }

        public static async Task LogBulkDeletedMessages(MessageBulkDeleteEventArgs e)
        {
            if (!init || logChannels[e.Channel.GuildId] == 0) return;

            var embed = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = e.Channel.Guild.Name,
                    IconUrl = e.Channel.Guild.IconUrl,
                },
                Timestamp = DateTime.Now,
                Color = new DiscordColor("#bc5400"),
                Description = $"**Bulk deleted `{e.Messages.Count}` messages from {e.Channel.Mention}.**"
            };
            Console.WriteLine("test");
            await e.Client.SendMessageAsync(e.Channel.Guild.GetChannel(logChannels[e.Channel.Guild.Id]), "", embed: embed);
        }

        public static async Task LogOther(DiscordClient c, DiscordGuild g, DiscordEmbed e)
        {
            if (!init || logChannels[g.Id] == 0) return;
            await c.SendMessageAsync(g.GetChannel(logChannels[g.Id]), "", embed: e);
        }

        public static async Task LogMessageEdit(MessageUpdateEventArgs e)
        {
            if (!init || logChannels[e.Guild.Id] == 0 || e.Message.Author.IsBot) return;

            var embed = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = e.Message.Author.Username + "#" + e.Message.Author.Discriminator + $" ({e.Message.Author.Id})",
                    IconUrl = e.Message.Author.AvatarUrl,
                },
                Color = new DiscordColor("#42e5f4"),
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = "Message ID: " + e.Message.Id
                },
                Description = "Message edited in " + e.Channel.Mention + ":"
            };
            if (e.MessageBefore != null) embed.AddField("Before", "\u200b" + e.MessageBefore.Content);
            embed.AddField("After", "\u200b" + e.Message.Content);

            await e.Client.SendMessageAsync(e.Guild.GetChannel(logChannels[e.Guild.Id]), "", embed: embed);
        }

        public static async Task LogVoice(VoiceStateUpdateEventArgs e)
        {
            if (!init || logChannels[e.Guild.Id] == 0 || e.User.IsBot) return;

            string state = "joined";
            DiscordChannel channel = default;

            if (e.Before == null)
                channel = e.After.Channel;
            else if (e.After == null)
            {
                state = "left";
                channel = e.Before.Channel;
            }
            else
                return;

            var embed = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = e.User.Username + "#" + e.User.Discriminator + $" ({e.User.Id})",
                    IconUrl = e.User.AvatarUrl,
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = "Channel ID: " + channel.Id
                },
                Description = $"{e.User.Mention} {state} voice channel {channel.Name}.",
                Color = new DiscordColor("#d942f4")
            };

            await e.Client.SendMessageAsync(e.Guild.GetChannel(logChannels[e.Guild.Id]), "", embed: embed);
        }
    }
}
