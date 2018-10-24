using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
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
                //Description = "",
                Color = Program.bot.embedColor,
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "🔨 = Moderator only command." }
            };

            embed.AddField("General Commands", $"• **{p}help:** Show this message.");
            embed.AddField("Tag Commands", $"• **{p}tag <tag name> [args]:** Display a tag." +
                                         $"\n• **{p}listtags:** List all tags on this server." +
                                         $"\n• **{p}rawtag <tag name>:** Display the raw text of a tag." +
                                         $"\n• **{p}createtag/edittag <tag name> <message>:** Create or edit a tag. \\🔨" +
                                         $"\n• **{p}deletetag <tag name>:** Delete a tag. \\🔨");
            embed.AddField("Config Commands", $"• **{p}prefix <new prefix>:** Set a new prefix for the server. \\🔨");

            await ctx.RespondAsync("", embed: embed);
        }
    }
}
