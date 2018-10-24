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
    class Config : BaseCommandModule
    {
        [Command("prefix")]
        public async Task Help(CommandContext ctx, string newPrefix = "")
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{ctx.Guild.Id}/config.json"));

            if (newPrefix == "")
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Prefix",
                    Description = $"This server's prefix is currently set to `{json.prefix}`.",
                    Color = Program.bot.embedColor
                };
                
                await ctx.RespondAsync("", embed: embed);
            }
            else
            {
                if (newPrefix.Length > 5)
                {
                    await  Program.bot.SendError(ctx, "Prefix", "Prefix cannot be longer than 5 characters.");
                }
                else
                {
                    json.prefix = newPrefix;
                    File.WriteAllText($"guilds/{ctx.Guild.Id}/config.json", JsonConvert.SerializeObject(json, Formatting.Indented));

                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "Prefix",
                        Description = $"This server's prefix is now set to `{json.prefix}`.",
                        Color = Program.bot.embedColor
                    };

                    await ctx.RespondAsync("", embed: embed);
                }
            }
        }
    }
}
