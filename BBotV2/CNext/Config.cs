using BBotV2.Misc;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
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
    class Config : Overrides.BaseCommandOverride
    {
        [Command("prefix"), IsAllowed("mod")]
        public async Task Help(CommandContext ctx, string newPrefix = "")
        {
            if (newPrefix == "")
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Prefix",
                    Description = $"This server's prefix is currently set to `{Bot.prefixes[ctx.Guild.Id]}`.",
                    Color = Program.embedColor
                };
                
                await ctx.RespondAsync("", embed: embed);
            }
            else
            {
                if (newPrefix.Length > 5) await Bot.SendError(ctx, "Prefix", "Prefix cannot be longer than 5 characters.");
                else
                {
                    dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{ctx.Guild.Id}/config.json"));
                    json.prefix = newPrefix;
                    File.WriteAllText($"guilds/{ctx.Guild.Id}/config.json", JsonConvert.SerializeObject(json, Formatting.Indented));
                    Bot.UpdateGuildPrefix(ctx.Guild.Id, newPrefix);

                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "Prefix",
                        Description = $"This server's prefix is now set to `{newPrefix}`.",
                        Color = Program.embedColor
                    };

                    await ctx.RespondAsync("", embed: embed);
                }
            }
        }

        [Command("logchannel"), IsAllowed("mod")]
        public async Task LogChannel(CommandContext ctx, string channeltext = "")
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{ctx.Guild.Id}/config.json"));
            if (channeltext == "")
            {
                if (UInt64.TryParse((string)json.logchannel, out ulong id))
                {
                    DiscordChannel c = ctx.Guild.GetChannel(id);
                    if (c == null) await Bot.SendError(ctx, "Log Channel", "Unknown channel.");
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
                else await Bot.SendError(ctx, "Log Channel", "No log channel has been set.");
            }
            else if (channeltext == "null" || channeltext == "none" || channeltext == "off")
            {
                json.logchannel = "null";
                File.WriteAllText($"guilds/{ctx.Guild.Id}/config.json", JsonConvert.SerializeObject(json, Formatting.Indented));
                Logger.UpdateLogChannel(ctx.Guild.Id);

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Log Channel",
                    Description = $"Log channel has been removed.",
                    Color = Program.embedColor
                };
                await ctx.RespondAsync("", embed: embed);
            }
            else
            {
                DiscordChannel c = default;
                try { c = (DiscordChannel)await new DiscordChannelConverter().ConvertAsync(channeltext, ctx); } catch { };
                if (c == default) await Bot.SendError(ctx, "Log Channel", "Unknown chanel.");
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

        [Command("autorole"), IsAllowed("mod")]
        public async Task AutoRole(CommandContext ctx, string roletext = "")
        {
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText($"guilds/{ctx.Guild.Id}/config.json"));
            
            if (roletext == "")
            {
                if (UInt64.TryParse((string)json.autorole, out ulong id))
                {
                    DiscordRole r = ctx.Guild.GetRole(id);
                    if (r == null) await Bot.SendError(ctx, "Auto Role", "Unknown role.");
                    else
                    {
                        var embed = new DiscordEmbedBuilder()
                        {
                            Title = "Auto Role",
                            Description = $"This server's auto role is set to {r.Mention}.",
                            Color = Program.embedColor,
                        };
                        await ctx.RespondAsync("", embed: embed);
                    }
                }
                else await Bot.SendError(ctx, "Auto Role", "No auto role has been set.");
            }
            else if (roletext == "null" || roletext == "none" || roletext == "off")
            {
                json.autorole = "null";
                File.WriteAllText($"guilds/{ctx.Guild.Id}/config.json", JsonConvert.SerializeObject(json, Formatting.Indented));

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Auto Role",
                    Description = $"Auto role has been removed.",
                    Color = Program.embedColor
                };
                await ctx.RespondAsync("", embed: embed);
            }
            else
            {
                DiscordRole r = default;
                try { r = (DiscordRole)await new DiscordRoleConverter().ConvertAsync(roletext, ctx); } catch { };
                if (r == default) await Bot.SendError(ctx, "Auto Role", "Unknown role.");
                else
                {
                    json.autorole = "" + r.Id;
                    File.WriteAllText($"guilds/{ctx.Guild.Id}/config.json", JsonConvert.SerializeObject(json, Formatting.Indented));

                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = "Auto Role",
                        Description = $"Auto role has been set to {r.Mention}.",
                        Color = Program.embedColor
                    };
                    await ctx.RespondAsync("", embed: embed);
                }
            }
        }
    }
}
