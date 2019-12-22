

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Slavestefan.Aphrodite.Web.Helpers
{
    public class RequireUserPrecondition : PreconditionAttribute
    {
        private readonly ulong[] _users;

        public RequireUserPrecondition(ulong[] users)
        {
            _users = users;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(_users.Contains(context.User.Id) ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("Unauthorized"));
        }
    }
}