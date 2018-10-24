using DSharpPlus;
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
using BBotV2.Misc;

namespace BBotV2.CNext
{
    class Tag : BaseCommandModule
    {
        [Command("tag")]
        public async Task TagCmd(CommandContext ctx, string name = "", [RemainingText] string args = "")
        {
            ulong id = ctx.Guild.Id;

            var tags = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"guilds/{id}/tags.json"));

            if (name == "") await Program.bot.SendError(ctx, "Tags", "Missing: `Tag name`");
            else if (!tags.ContainsKey(name)) await Program.bot.SendError(ctx, "Tags", "That tag doesn't exist.");
            else
            {
                string message = tags[name];
                if (args != "") message = message.Replace("%arg%", args);
                else message = message.Replace(" %arg%", "").Replace("%arg% ", "");

                if (Perms.BotCanDelete(ctx.Guild.CurrentMember, ctx.Channel)) await ctx.Message.DeleteAsync();
                await ctx.RespondAsync(message);
            }
        }

        [Command("createtag"), Aliases("edittag")]
        public async Task CreateTag(CommandContext ctx, string name = "", [RemainingText] string message = "")
        {
            if (!Perms.UserIsMod(ctx.Member, ctx.Channel)) return;

            if (name == "") await Program.bot.SendError(ctx, "Tags", "Missing: `Tag name`, `Tag message`");
            else if (message == "") await Program.bot.SendError(ctx, "Tags", "Missing: `Tag message`");
            else
            {
                ulong id = ctx.Guild.Id;
                name = name.ToLower();

                var tags = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"guilds/{id}/tags.json"));

                string action = "created";
                if (tags.ContainsKey(name))
                {
                    tags.Remove(name);
                    action = "edited";
                }
                tags.Add(name, message);

                File.WriteAllText($"guilds/{id}/tags.json", JsonConvert.SerializeObject(tags, Formatting.Indented));

                bool hasCodeBlock = message.Contains("```");
                if (hasCodeBlock)
                    message = message.Replace("```", "'''");

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Tags",
                    Description = $"Successfully {action} tag `{name}` with message:\n```{message}```",
                    Color = Program.bot.embedColor
                };
                if (hasCodeBlock)
                    embed.WithFooter("Note: Triple back-quotes (```) replaced with triple apostrophes (''') in this message.");

                await ctx.RespondAsync("", embed: embed);
            }
        }

        [Command("deletetag"), Aliases("deltag")]
        public async Task DeleteTag(CommandContext ctx, string name = "")
        {
            if (!Perms.UserIsMod(ctx.Member, ctx.Channel)) return;

            ulong id = ctx.Guild.Id;

            var tags = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"guilds/{id}/tags.json"));

            if (name == "") await Program.bot.SendError(ctx, "Tags", "Missing: `Tag name`");
            else if (!tags.ContainsKey(name)) await Program.bot.SendError(ctx, "Tags", "That tag doesn't exist.");
            else
            {
                tags.Remove(name);

                File.WriteAllText($"guilds/{id}/tags.json", JsonConvert.SerializeObject(tags, Formatting.Indented));

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Tags",
                    Description = $"Successfully deleted tag `{name}`.",
                    Color = Program.bot.embedColor
                };

                await ctx.RespondAsync("", embed: embed);
            }
        }

        [Command("rawtag")]
        public async Task RawTag(CommandContext ctx, string name = "")
        {
            ulong id = ctx.Guild.Id;

            var tags = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"guilds/{id}/tags.json"));

            if (name == "") await Program.bot.SendError(ctx, "Tags", "Missing: `Tag name`");
            else if (!tags.ContainsKey(name)) await Program.bot.SendError(ctx, "Tags", "That tag doesn't exist.");
            else
            {
                string message = tags[name];
                bool hasCodeBlock = message.Contains("```");
                if (hasCodeBlock)
                    message = message.Replace("```", "'''");

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Tags",
                    Description = $"Raw message for `{name}`:\n```{message}```",
                    Color = Program.bot.embedColor
                };
                if (hasCodeBlock)
                    embed.WithFooter("Note: Triple back-quotes (```) replaced with apostrophes (''') in this message.");
                
                await ctx.RespondAsync("", embed: embed);
            }
        }

        [Command("listtags"), Aliases("tags")]
        public async Task ListTags(CommandContext ctx)
        {
            var tags = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"guilds/{ctx.Guild.Id}/tags.json"));

            if (tags.Count == 0) await Program.bot.SendError(ctx, "Tags", "There are no tags on this server.");
            else
            {
                string message = "**Tags in this server:**\n```";
                foreach (KeyValuePair<string, string> tag in tags) message += tag.Key + ", ";
                message = message.Substring(0, message.Length - 2);
                message += "```";

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Tags",
                    Description = message,
                    Color = Program.bot.embedColor
                };

                await ctx.RespondAsync("", embed: embed);
            }
        }
    }
}
