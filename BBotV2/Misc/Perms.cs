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
        public static bool CanDelete(DiscordMember m, DiscordChannel c)
        {
            Permissions p = m.PermissionsIn(c);
            return p.HasFlag(Permissions.Administrator) || p.HasFlag(Permissions.ManageMessages);
        }
    }
}
