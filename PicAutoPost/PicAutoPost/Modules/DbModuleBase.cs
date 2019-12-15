using System;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PicAutoPost.Modules
{
    public class DbModuleBase<TDbContext> : ModuleBase<SocketCommandContext> where TDbContext : DbContext, IDisposable
    {
        protected TDbContext TypedDbContext
        {
            get;
        }

        private readonly IServiceScope _scope;

        public DbModuleBase(IServiceProvider services)
        {
            _scope = services.CreateScope();
            TypedDbContext = _scope.ServiceProvider.GetRequiredService<TDbContext>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}