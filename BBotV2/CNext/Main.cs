using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2.CNext
{
    class Main : BaseCommandModule
    {
        [Command("help"), Aliases("h")]
        public async Task Help(CommandContext ctx)
        {
            
            var embed = new DiscordEmbedBuilder()
            {
                Title = "BBot V2",
                Description = "Test successful!",
                Color = new DiscordColor("#640f7e")
            };

            
            await ctx.RespondAsync("", embed: embed);
        }
    }
}
