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
    class Fun : Overrides.BaseCommandOverride
    {
        [Command("ascii")]
        public async Task Ascii(CommandContext ctx, [RemainingText] string message = "")
        {
            if (message == "")  await Bot.SendError(ctx, "Ascii", "Missing: `Message`");
            else await ctx.RespondAsync($"```{new WebClient().DownloadString($"http://artii.herokuapp.com/make?text={message}")}```");
        }

        [Command("math")]
        public async Task Math(CommandContext ctx, [RemainingText] string equation = "")
        {
            if (equation == "") await Bot.SendError(ctx, "Math", $"Missing: `Equation`");
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
                    await Bot.SendError(ctx, "Math", $"{e.Message}\n\n*(Show this to <@{Program.botOwnerId}>)*");
                    return;
                }

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Math",
                    Description = $"Equation: **`{equation}`**\n\nResult: **`{val}`**",
                    Color = Program.embedColor
                };
                await ctx.RespondAsync("", embed: embed);
            }
        }

        [Command("morse")]
        public async Task Morse(CommandContext ctx, [RemainingText] string message = "")
        {
            if (message == "") await Bot.SendError(ctx, "Morse Translator", "Missing: `message`");
            else if (message.ToCharArray().All(c => "-_. ".Contains(c)))
            {
                // morse to text
                dynamic json = JsonConvert.DeserializeObject(new WebClient().DownloadString("http://www.morsecode-api.de/decode?string=" + message));

                string text = json.plaintext;
                string morse = json.morsecode;

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Morse Translator",
                    Color = Program.embedColor
                };
                embed.AddField("Morse Code", morse);
                embed.AddField("Plain Text", text);

                await ctx.RespondAsync("", embed: embed);
            }
            else
            {
                // text to morse
                dynamic json = JsonConvert.DeserializeObject(new WebClient().DownloadString("http://www.morsecode-api.de/encode?string=" + message));

                string text = json.plaintext;
                string morse = json.morsecode;

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Morse Translator",
                    Color = Program.embedColor
                };
                embed.AddField("Plain Text", text);
                embed.AddField("Morse Code", morse);

                await ctx.RespondAsync("", embed: embed);
            }
        }
    }
}
