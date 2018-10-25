using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2.Misc
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IsAllowedAttribute : CheckBaseAttribute
    {
        public string type { get; }

        public IsAllowedAttribute(string type)
        {
            this.type = type;
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (type == "mod") return Perms.UserIsMod(ctx.Member, ctx.Channel);
            else if (type == "botowner") return ctx.Member.Id == Program.botOwnerId;
            else return false;
        }
    }
}
