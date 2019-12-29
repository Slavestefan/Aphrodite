using System;
using Microsoft.EntityFrameworkCore;

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotConfiguration>().HasIndex(b => new {b.ChannelId, b.Key}).IsUnique();
            modelBuilder.Entity<Picture>().HasIndex("Hash", "UserIdUser").IsUnique();
        }
    }
} 