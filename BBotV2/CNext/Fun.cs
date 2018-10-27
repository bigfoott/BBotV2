using BBotV2.Misc;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2.CNext
{
    class Fun : BaseCommandModule
    {
        [Command("ascii")]
        public async Task Ascii(CommandContext ctx, [RemainingText] string message = "")
        {
            if (message == "")  await Program.bot.SendError(ctx, "Ascii", "Missing: `Message`");
            else await ctx.RespondAsync($"```{new WebClient().DownloadString($"http://artii.herokuapp.com/make?text={message}")}```");
        }

        [Command("math")]
        public async Task Math(CommandContext ctx, [RemainingText] string equation = "")
        {
            if (equation == "") await Program.bot.SendError(ctx, "Math", $"Missing: `Equation`");
            else
            {
                MathParser parser = new MathParser();
                double val = 0.0;
                try
                {
                    val = parser.Parse(equation);
                }
                catch (Exception e)
                {
                    await Program.bot.SendError(ctx, "Math", $"{e.Message}\n\n*(Show this to <@{Program.botOwnerId}>)*");
                    return;
                }

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Math",
                    Description = $"Equation: **`{equation}`**\n\nResult: **`{val}`**",
                    Color = Program.bot.embedColor
                };
                await ctx.RespondAsync("", embed: embed);
            }
        }
    }
}
