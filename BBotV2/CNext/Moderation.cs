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
using DSharpPlus;

namespace BBotV2.CNext
{
    class Moderation : BaseCommandModule
    {
        [Command("vkick"), IsAllowed("mod")]
        public async Task KickVoice(CommandContext ctx, DiscordMember m = default)
        {
            if (m == default) await Program.bot.SendError(ctx, "Voicekick", "Missing: `User to kick`");
            else if (!ctx.Guild.Channels.Any(c => c.Type == ChannelType.Voice && c.Users.Any(u => u.Id == m.Id)))
                await Program.bot.SendError(ctx, "Voicekick", "That user is not in a voice channel.");
            else if (m.IsOwner && m != ctx.Member) await Program.bot.SendError(ctx, "Voicekick", "That user has a higher role than you.");
            else
            {
                DiscordRole kicked = m.Roles.OrderByDescending(r => r.Position).FirstOrDefault();
                DiscordRole kicking = ctx.Member.Roles.OrderByDescending(r => r.Position).FirstOrDefault();

                int kickedInt = -1;
                int kickingInt = -1;

                if (kicked != null) kickedInt = kicked.Position;
                if (kicking != null) kickingInt = kicking.Position;

                if (m != ctx.Member && kickedInt >= kickingInt) await Program.bot.SendError(ctx, "Voicekick", "That user has a higher role than you.");
                else
                {
                    DiscordChannel vc = ctx.Guild.Channels.FirstOrDefault(c => c.Type == ChannelType.Voice && c.Users.Any(u => u.Id == m.Id));

                    bool manageChannels = ctx.Guild.CurrentMember.Roles.Any(r => r.Permissions.HasFlag(Permissions.ManageChannels) || r.Permissions.HasFlag(Permissions.Administrator));
                    bool moveMembers = ctx.Guild.CurrentMember.Roles.Any(r => r.Permissions.HasFlag(Permissions.MoveMembers) || r.Permissions.HasFlag(Permissions.Administrator));

                    if (!manageChannels && !moveMembers) await Program.bot.SendError(ctx, "Voicekick", $"Missing permissions: `Manage Channels, Move Members`");
                    else if (!manageChannels) await Program.bot.SendError(ctx, "Voicekick", $"Missing permission: `Manage Channels`");
                    else if (!moveMembers) await Program.bot.SendError(ctx, "Voicekick", $"Missing permission: `Move Members`");
                    else
                    {
                        DiscordChannel channel = default;
                        if (vc.Parent != null) channel = await ctx.Guild.CreateChannelAsync("Kicking", ChannelType.Voice, vc.Parent);
                        else channel = await ctx.Guild.CreateChannelAsync("Kicking", ChannelType.Voice);
                        
                        await channel.PlaceMemberAsync(m);
                        await channel.DeleteAsync();

                        var embed = new DiscordEmbedBuilder()
                        {
                            Title = "Votekick",
                            Description = $"{m.Mention} has been kicked from voice chat.",
                            Color = Program.bot.embedColor
                        };

                        await ctx.RespondAsync("", embed: embed);
                    }
                }
            }
        }
    }
}
