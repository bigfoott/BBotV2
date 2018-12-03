using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2.CNext
{
    class General : Overrides.BaseCommandOverride
    {
        [Command("help"), Aliases("h")]
        public async Task Help(CommandContext ctx)
        {
            string p = Bot.prefixes[ctx.Guild.Id];

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Help",
                Color = Program.embedColor,
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "🔨 = Moderator only command." }
            };

            embed.AddField("General Commands", $"• **{p}help:** Show this message." +
                                             $"\n• **{p}info:** Get info about the bot." +
                                             $"\n• **{p}whois <user>:** Get info about a user.");
            embed.AddField("Fun Commands", $"• **{p}ascii <message>:** Create ascii art from text." +
                                         $"\n• **{p}math <equation>:** Calcuate a math equation." +
                                         $"\n• **{p}morse <message/code>:** Translate to/from morse code.");
            embed.AddField("Tag Commands", $"• **{p}tag <tag name> [args]:** Display a tag." +
                                         $"\n• **{p}listtags:** List all tags on this server." +
                                         $"\n• **{p}rawtag <tag name>:** Display the raw text of a tag." +
                                         $"\n• **{p}createtag/edittag <tag name> <message>:** Create or edit a tag. \\🔨" +
                                         $"\n• **{p}deletetag <tag name>:** Delete a tag. \\🔨");
            embed.AddField("Moderation Commands", $"• **{p}vkick <user>:** Kick a user from voice chat. \\🔨" +
                                                $"\n• **{p}delete [amount]:** Delete a certain amount of messages in chat. \\🔨" +
                                                $"\n• **{p}clean [amount]:** Clear chat of bot commands and messages. \\🔨" );
            embed.AddField("Config Commands", $"• **{p}prefix <new prefix>:** Set a new prefix for the server. \\🔨" +
                                            $"\n• **{p}logchannel [channel]:** Set the log channel for the server. \\🔨" +
                                            $"\n• **{p}autorole [role]:** Set the autorole for this server. \\🔨");
            
            await ctx.RespondAsync("", embed: embed);
        }
        
        [Command("info")]
        public async Task Info(CommandContext ctx)
        {
            int sec = (int)Math.Truncate((DateTime.Now - Bot.startTime).TotalSeconds), min = 0, hour = 0, day = 0, week = 0;
            while (sec >= 60) { sec = sec - 60; min++; }
            while (min >= 60) { min = min - 60; hour++; }
            while (hour >= 24) { hour = hour - 24; day++; }
            while (day >= 7) { day = day - 7; week++; }
            string uptime = $"{week}w {day}d {hour}h {min}m {sec}s";
            
            string leftCol = $"• **Creator**: <@{Program.botOwnerId}>\n" +
                             $"• **GitHub**: [Click Here](https://github.com/bigfoott/BBotV2)";
            string rightCol = $"• **Servers**: {Bot.totalGuilds}\n" +
                              $"• **Users**: {Bot.totalUsers}\n" +
                              $"• **Uptime**: {uptime}";

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Info",
                Description = "BBot V2 <:POGGIES:492954535170408453>",
                Color = Program.embedColor
            };
            embed.AddField("\u200b", leftCol, true).AddField("\u200b", rightCol, true);
            await ctx.RespondAsync("", embed: embed);
        }
        
        [Command("whois")]
        public async Task Whois(CommandContext ctx, DiscordMember m = default)
        {
            if (m == default) m = ctx.Member;

            DiscordColor embedColor = DiscordColor.Grayple;
            
            var role = m.Roles.Where(r => r.Color.Value != 0).OrderByDescending(r => r.Position).FirstOrDefault();
            if (role != null) embedColor = role.Color;

            string status = "Offline";
            if (m.Presence != null && m.Presence.Activity != null && m.Presence.Activity.ActivityType == ActivityType.Streaming)
                status = "Streaming";
            else if (m.Presence != null)
                status = m.Presence.Status + "";
            status = status
                .Replace("DoNotDisturb", "<:dnd:504931722693967872> Do Not Disturb")
                .Replace("Online", "<:online:504931722706812928> Online")
                .Replace("Idle", "<:away:504931722610081792> Idle")
                .Replace("Offline", "<:offline:504931722723459072> Offline")
                .Replace("Streaming", "<:streaming:504931722698293249> Streaming");

            string suffix = "";
            if (m.IsBot) suffix += "<:bot:504931722220142593>";

            string game = "None";
            if (m.Presence != null && m.Presence.Activity != null && m.Presence.Activity.Name != null) game = m.Presence.Activity.Name;

            string roles = "";
            foreach (DiscordRole r in m.Roles.OrderByDescending(r => r.Position)) roles += r.Mention + ", ";
            
            var embed = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = m.Username + "#" + m.Discriminator,
                    IconUrl = m.AvatarUrl,
                    Url = m.AvatarUrl
                },
                Color = embedColor,
                Description = $"{m.DisplayName} {suffix} ({m.Mention})"
            };
            
            embed.AddField("Status", status, true)
                 .AddField("Game", game, true)
                 .AddField("ID", "" + m.Id, true)
                 .AddField("Registered Account", m.CreationTimestamp.AddHours(-5).ToString("ddd, MMM d, yyyy @ h:mm tt"), true)
                 .AddField("Joined Server", m.JoinedAt.ToString("ddd, MMM d, yyyy @ h:mm tt"), true)
                 .AddField($"Roles ({m.Roles.Count()})", roles.Substring(0, roles.Length - 2));

            await ctx.RespondAsync("", embed: embed);
        }
    }
}
