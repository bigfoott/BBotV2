using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace BBotV2.Misc
{
    class LinkPreviewer
    {
        public static async Task MessageCreated(MessageCreateEventArgs e)
        {
            string[] links = e.Message.Content.Split(new[] { "https://discordapp.com/channels/" }, StringSplitOptions.None);
            
            if (links.Length > 1)
            {
                for (int i = 1; i < links.Length; i++)
                {
                    string[] parts = links[i].Split('/');
                    if (parts.Length == 3)
                    {
                        if (parts[0] == e.Guild.Id.ToString() && UInt64.TryParse(parts[1], out ulong ch) && UInt64.TryParse(parts[2], out ulong ms))
                        {
                            var msg = await e.Guild.GetChannel(ch).GetMessageAsync(ms);

                            string content = msg.Content;
                            if (content.Length > 1024)
                                content = content.Substring(0, 1021) + "...";

                            var embed = new DiscordEmbedBuilder()
                            {
                                Author = new DiscordEmbedBuilder.EmbedAuthor()
                                {
                                    Name = msg.Author.Username + "#" + msg.Author.Discriminator,
                                    IconUrl = msg.Author.AvatarUrl
                                },
                                Description = content,
                                Timestamp = msg.Timestamp,
                                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "ID: " + msg.Id }
                            };

                            if (msg.Attachments.Count > 0)
                            {
                                string str = "";
                                foreach (DiscordAttachment a in msg.Attachments)
                                {
                                    string m = $"[{a.FileName}]({a.Url})\n";
                                    if (str.Length + m.Length > 1024) break;
                                    str += m;
                                }
                                string s = "";
                                if (msg.Attachments.Count > 1) s = "s";

                                embed.AddField("Attachment" + s, str);
                            }

                            await e.Channel.SendMessageAsync("", embed: embed);
                        }
                    }
                }
            }
        }
    }
}
