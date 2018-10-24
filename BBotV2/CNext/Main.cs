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
                Title = "Help",
                Description = "ok so basically im monky",
                Color = Program.bot.embedColor
            };

            await ctx.RespondAsync("", embed: embed);
        }
    }
}
