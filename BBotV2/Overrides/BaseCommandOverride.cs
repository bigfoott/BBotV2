using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBotV2.Overrides
{
    abstract class BaseCommandOverride : BaseCommandModule
    {
        public override Task BeforeExecutionAsync(CommandContext ctx)
        {
            Misc.Logger.LogCommand(ctx);
            return base.BeforeExecutionAsync(ctx);
        }
    }
}
