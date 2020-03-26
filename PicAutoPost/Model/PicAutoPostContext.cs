using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Slavestefan.Aphrodite.Model.Tasks;
using Task = Slavestefan.Aphrodite.Model.Tasks.Task;

namespace Slavestefan.Aphrodite.Model
{
    public class PicAutoPostContext : DbContext
    {
        public PicAutoPostContext() : base()
        {

        }

        public PicAutoPostContext(DbContextOptions<PicAutoPostContext> options) : base(options)
        {

        }

        public DbSet<PostConfiguration> Configurations { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BotConfiguration> BotConfigurations { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskConfiguration> TaskConfigurations { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotConfiguration>().HasIndex(b => new {b.ChannelId, b.Key}).IsUnique();
            modelBuilder.Entity<Picture>().HasIndex("Hash", "UserIdUser").IsUnique();
        }

        public async Task<User> GetOrCreateUserAsync(ulong snowflake, string name)
        {
            var user = await this.Users.FirstOrDefaultAsync(x => x.DiscordId == snowflake);
            if (user == null)
            {
                user = new User()
                {
                    DiscordId = snowflake,
                    IdUser = Guid.NewGuid(),
                    Username = name
                };
                this.Users.Add(user);
                this.SaveChanges();
            }

            return user;
        }
    }
} 