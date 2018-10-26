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
    }
}
