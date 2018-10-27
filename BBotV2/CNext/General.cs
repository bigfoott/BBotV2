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
    class General : BaseCommandModule
    {
        [Command("help"), Aliases("h")]
        public async Task Help(CommandContext ctx)
        {
            string p = ((dynamic)JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{ctx.Guild.Id}/config.json"))).prefix;

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Help",
                Color = Program.bot.embedColor,
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "🔨 = Moderator only command." }
            };

            embed.AddField("General Commands", $"• **{p}help:** Show this message." +
                                             $"\n• **{p}whois <user>:** Get info about a user.");
            embed.AddField("Fun Commands", $"• **{p}ascii <message>:** Create ascii art of text." +
                                         $"\n• **{p}math <equation>:** Calcuate a math equation.");
            embed.AddField("Tag Commands", $"• **{p}tag <tag name> [args]:** Display a tag." +
                                         $"\n• **{p}listtags:** List all tags on this server." +
                                         $"\n• **{p}rawtag <tag name>:** Display the raw text of a tag." +
                                         $"\n• **{p}createtag/edittag <tag name> <message>:** Create or edit a tag. \\🔨" +
                                         $"\n• **{p}deletetag <tag name>:** Delete a tag. \\🔨");
            embed.AddField("Moderation COmmands", $"• **{p}delete [amount]:** Delete a certain amount of messages in chat. \\🔨" );
            embed.AddField("Config Commands", $"• **{p}prefix <new prefix>:** Set a new prefix for the server. \\🔨");

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
