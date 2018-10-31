using BBotV2.Misc;
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
        [Command("prefix"), IsAllowed("mod")]
        public async Task Help(CommandContext ctx, string newPrefix = "")
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{ctx.Guild.Id}/config.json"));
            
            if (newPrefix == "")
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Prefix",
                    Description = $"This server's prefix is currently set to `{json.prefix}`.",
                    Color = Program.embedColor
                };
                
                await ctx.RespondAsync("", embed: embed);
            }
            else
            {
                if (newPrefix.Length > 5) await Program.bot.SendError(ctx, "Prefix", "Prefix cannot be longer than 5 characters.");
                else
                {
                    json.prefix = newPrefix;
                    File.WriteAllText($"guilds/{ctx.Guild.Id}/config.json", JsonConvert.SerializeObject(json, Formatting.Indented));

                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "Prefix",
                        Description = $"This server's prefix is now set to `{json.prefix}`.",
                        Color = Program.embedColor
                    };

                    await ctx.RespondAsync("", embed: embed);
                }
            }
        }

        [Command("logchannel"), IsAllowed("mod")]
        public async Task LogChannel(CommandContext ctx, DiscordChannel c = default)
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{ctx.Guild.Id}/config.json"));
            if (c == default)
            {
                if (UInt64.TryParse((string)json.logchannel, out ulong id))
                {
                    c = ctx.Guild.GetChannel(id);
                    if (c == null) await Program.bot.SendError(ctx, "Log Channel", "Unknown channel.");
                    else
                    {
                        var embed = new DiscordEmbedBuilder()
                        {
                            Title = "Log Channel",
                            Description = $"This server's log channel is set to {c.Mention}.",
                            Color = Program.embedColor,
                        };
                        await ctx.RespondAsync("", embed: embed);
                    }
                }
                else await Program.bot.SendError(ctx, "Log Channel", "No log channel has been set.");
            }
            else
            {
                if (c == null) await Program.bot.SendError(ctx, "Log Channel", "Unknown channel.");
                else
                {
                    json.logchannel = "" + c.Id;
                    File.WriteAllText($"guilds/{ctx.Guild.Id}/config.json", JsonConvert.SerializeObject(json, Formatting.Indented));
                    Logger.UpdateLogChannel(ctx.Guild.Id);

                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "Log Channel",
                        Description = $"Log channel has been set to {c.Mention}.",
                        Color = Program.embedColor
                    };
                    await ctx.RespondAsync("", embed: embed);
                }
            }
        }
    }
}
