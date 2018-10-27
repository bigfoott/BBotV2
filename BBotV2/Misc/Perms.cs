using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2.Misc
{
    class Perms
    {
        public static bool UserHasChannelPerm(DiscordMember m, DiscordChannel c, Permissions perm)
        {
            Permissions p = m.PermissionsIn(c);
            return p.HasFlag(Permissions.Administrator) || p.HasFlag(perm);
        }

        public static bool BotHasGuildPerm(DiscordGuild g, Permissions perm)
        {
            return g.CurrentMember.Roles.Any(r => r.Permissions.HasFlag(perm) || r.Permissions.HasFlag(Permissions.Administrator));
        }
        
        public static bool UserIsMod(DiscordMember m, DiscordChannel c)
        {
            Permissions p = m.PermissionsIn(c);
            return p.HasFlag(Permissions.Administrator) || p.HasFlag(Permissions.ManageGuild) || m.Roles.Any(r => r.Name.ToLower() == "bbot mod");
        }
    }
}
