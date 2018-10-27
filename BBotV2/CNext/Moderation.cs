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
using DSharpPlus.Exceptions;

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

                    bool manageChannels = Perms.BotHasGuildPerm(ctx.Guild, Permissions.ManageChannels);
                    bool moveMembers = Perms.BotHasGuildPerm(ctx.Guild, Permissions.MoveMembers);

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

        [Command("delete"), IsAllowed("mod")]
        public async Task Delete(CommandContext ctx, int count = 1)
        {
            if (!Perms.UserHasChannelPerm(ctx.Guild.CurrentMember, ctx.Channel, Permissions.ManageMessages))
                await Program.bot.SendError(ctx, "Delete", "Missing permission: `Manage Messages`");
            else
            {
                await ctx.Message.DeleteAsync();

                var embed = new DiscordEmbedBuilder()
                {
                    Title = "Delete",
                    Description = "<a:loading:505521532957884417> Deleting messages...",
                    Color = Program.bot.embedColor
                };

                DiscordMessage msg = await ctx.RespondAsync("", embed: embed);

                if (count > 100) count = 100;

                List<DiscordMessage> msgs = (await ctx.Channel.GetMessagesAsync(limit: count + 1)).Where(m => m.Id != msg.Id).ToList();

                int deleted = msgs.Count;
                try
                {
                    await ctx.Channel.DeleteMessagesAsync(msgs);
                }
                catch (Exception ex)
                {
                    deleted = 0;
                    if (ex is BadRequestException)
                    {
                        List<DiscordMessage> delete = new List<DiscordMessage>();
                        foreach (DiscordMessage m in msgs)
                        {
                            if (m.CreationTimestamp > DateTimeOffset.UtcNow.AddDays(-14))
                            {
                                Console.WriteLine(m.Content);
                                delete.Add(m);
                            }
                        }
                        deleted = delete.Count;
                        await ctx.Channel.DeleteMessagesAsync(delete);
                    }
                    else if (ex is UnauthorizedException)
                    {
                        List<DiscordMessage> delete = new List<DiscordMessage>();
                        foreach (DiscordMessage _m in msgs.Where(m => m.CreationTimestamp > DateTimeOffset.UtcNow.AddDays(-14))) delete.Add(_m);
                        deleted = delete.Count;
                        await ctx.Channel.DeleteMessagesAsync(delete);
                    }
                    else
                    {
                        await msg.DeleteAsync();
                        await Program.bot.SendError(ctx, "Delete", $"{ex}\n\n*(Show this to <@{Program.botOwnerId}>)*");
                    }

                }
                embed = new DiscordEmbedBuilder()
                {
                    Title = "Delete",
                    Description = $"Successfully deleted **`{deleted}`** messages.",
                    Color = Program.bot.embedColor
                };
                await msg.ModifyAsync("", embed: (DiscordEmbed)embed);
                await Task.Delay(new TimeSpan(0, 0, 5)).ContinueWith(t => msg.DeleteAsync());
            }
        }
    }
}
