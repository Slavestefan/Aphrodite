﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Slavestefan.Aphrodite.Model;

namespace Slavestefan.Aphrodite.Model.Migrations
{
    [DbContext(typeof(PicAutoPostContext))]
    partial class PicAutoPostContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.BotConfiguration", b =>
                {
                    b.Property<Guid>("IdBotConfiguration")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("ValueBool")
                        .HasColumnType("bit");

                    b.Property<int>("ValueInt")
                        .HasColumnType("int");

                    b.Property<string>("ValueString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ValueUlong")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("IdBotConfiguration");

                    b.HasIndex("ChannelId", "Key")
                        .IsUnique()
                        .HasFilter("[Key] IS NOT NULL");

                    b.ToTable("BotConfigurations");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Picture", b =>
                {
                    b.Property<Guid>("IdPicture")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Hash")
                        .IsRequired()
                        .HasColumnType("varbinary(900)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PicturePoolIdPicturePool")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Raw")
                        .HasColumnType("varbinary(max)");

                    b.Property<Guid?>("UserIdUser")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IdPicture");

                    b.HasIndex("PicturePoolIdPicturePool");

                    b.HasIndex("UserIdUser");

                    b.HasIndex("Hash", "UserIdUser")
                        .IsUnique()
                        .HasFilter("[UserIdUser] IS NOT NULL");

                    b.ToTable("Pictures");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.PicturePool", b =>
                {
                    b.Property<Guid>("IdPicturePool")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("OwnerIdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PostConfigurationIdConfiguration")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IdPicturePool");

                    b.HasIndex("OwnerIdUser");

                    b.HasIndex("PostConfigurationIdConfiguration");

                    b.ToTable("PicturePool");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.PostConfiguration", b =>
                {
                    b.Property<Guid>("IdConfiguration")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<bool>("IsInSithMode")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRunning")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastPost")
                        .HasColumnType("datetime2");

                    b.Property<int>("MaxPostPerInterval")
                        .HasColumnType("int");

                    b.Property<int>("MaxPostingIntervalInMinutes")
                        .HasColumnType("int");

                    b.Property<int>("MinPostPerInterval")
                        .HasColumnType("int");

                    b.Property<int>("MinPostingIntervalInMinutes")
                        .HasColumnType("int");

                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("IdConfiguration");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Tasks.Task", b =>
                {
                    b.Property<Guid>("IdTask")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("TaskSetIdTaskSet")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IdTask");

                    b.HasIndex("TaskSetIdTaskSet");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Tasks.TaskHistory", b =>
                {
                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("PickerIdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TaskIdTask")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Time");

                    b.HasIndex("PickerIdUser");

                    b.HasIndex("TaskIdTask");

                    b.ToTable("TaskHistories");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Tasks.TaskSet", b =>
                {
                    b.Property<Guid>("IdTaskSet")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("DoAllowMultiroll")
                        .HasColumnType("bit");

                    b.Property<bool>("DoesMultirollRepeat")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerIdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("RecipientIdUser")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IdTaskSet");

                    b.HasIndex("OwnerIdUser");

                    b.HasIndex("RecipientIdUser");

                    b.ToTable("TaskSets");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Users.User", b =>
                {
                    b.Property<Guid>("IdUser")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdUser");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Users.UserAlias", b =>
                {
                    b.Property<Guid>("IdUserAlias")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Alias")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserIdUser")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("IdUserAlias");

                    b.HasIndex("UserIdUser");

                    b.ToTable("UserAliases");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Picture", b =>
                {
                    b.HasOne("Slavestefan.Aphrodite.Model.PicturePool", null)
                        .WithMany("Pictures")
                        .HasForeignKey("PicturePoolIdPicturePool");

                    b.HasOne("Slavestefan.Aphrodite.Model.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserIdUser");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.PicturePool", b =>
                {
                    b.HasOne("Slavestefan.Aphrodite.Model.Users.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerIdUser");

                    b.HasOne("Slavestefan.Aphrodite.Model.PostConfiguration", null)
                        .WithMany("Pool")
                        .HasForeignKey("PostConfigurationIdConfiguration");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Tasks.Task", b =>
                {
                    b.HasOne("Slavestefan.Aphrodite.Model.Tasks.TaskSet", null)
                        .WithMany("Tasks")
                        .HasForeignKey("TaskSetIdTaskSet");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Tasks.TaskHistory", b =>
                {
                    b.HasOne("Slavestefan.Aphrodite.Model.Users.User", "Picker")
                        .WithMany()
                        .HasForeignKey("PickerIdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Slavestefan.Aphrodite.Model.Tasks.Task", "Task")
                        .WithMany()
                        .HasForeignKey("TaskIdTask")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Tasks.TaskSet", b =>
                {
                    b.HasOne("Slavestefan.Aphrodite.Model.Users.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerIdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Slavestefan.Aphrodite.Model.Users.User", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientIdUser");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Users.UserAlias", b =>
                {
                    b.HasOne("Slavestefan.Aphrodite.Model.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserIdUser");
                });
#pragma warning restore 612, 618
        }
    }
}
