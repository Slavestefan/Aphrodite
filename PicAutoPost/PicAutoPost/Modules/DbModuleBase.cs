using System;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Slavestefan.Aphrodite.Web.Modules
{
    public class DbModuleBase<TDbContext> : ModuleBase<SocketCommandContext> where TDbContext : DbContext, IDisposable
    {
        protected TDbContext TypedDbContext
        {
            get;
        }

        protected IServiceScope Scope
        {
            get;
        }

        public DbModuleBase(IServiceProvider services)
        {
            Scope = services.CreateScope();
            TypedDbContext = Scope.ServiceProvider.GetRequiredService<TDbContext>();
        }

        public void Dispose()
        {
            Scope.Dispose();
        }
    }
}