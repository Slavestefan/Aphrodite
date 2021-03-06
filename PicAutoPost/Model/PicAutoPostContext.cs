﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Slavestefan.Aphrodite.Model.Tasks;
using Slavestefan.Aphrodite.Model.Tracker;
using Slavestefan.Aphrodite.Model.Users;
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
        public DbSet<TaskSet> TaskSets { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<MultiSet> MultiSet { get; set; }
        public DbSet<UserAlias> UserAliases { get; set; }
        public DbSet<OwnerSlaveRelationship> OwnerSlaveRelationships { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<CounterHistory> CounterHistory {get;set;}
        //public DbSet<UserConfiguration> UserConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotConfiguration>().HasIndex(b => new {b.ChannelId, b.Key}).IsUnique();
            modelBuilder.Entity<Picture>().HasIndex("Hash", "UserIdUser").IsUnique();

            modelBuilder.Entity<MultiSetTaskSet>()
                .HasOne(msts => msts.MultiSet)
                .WithMany(ms => ms.MultiSetTaskSets)
                .HasForeignKey(mst => mst.IdMultiSet);

            modelBuilder.Entity<MultiSetTaskSet>()
                .HasOne(msts => msts.TaskSet)
                .WithMany(ts => ts.MultiSetTaskSets)
                .HasForeignKey(msts => msts.IdTaskSet);

            modelBuilder.Entity<OwnerSlaveRelationship>()
                .HasIndex("SlaveIdUser", "OwnerIdUser").IsUnique();

            modelBuilder.Entity<OwnerSlaveRelationship>()
                .Property("OwnerIdUser")
                .IsRequired();

            modelBuilder.Entity<OwnerSlaveRelationship>()
                .Property("SlaveIdUser")
                .IsRequired();
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

        public async Task<User> GetOwnerAsync(User user)
            => (await this.OwnerSlaveRelationships.Include(x => x.Slave).Include(x => x.Owner).FirstOrDefaultAsync(x => x.Slave.IdUser == user.IdUser)).Owner;

    }
} 