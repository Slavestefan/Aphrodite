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

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Log", b =>
                {
                    b.Property<Guid>("IdLog")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("AuthorId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("ChannelName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("IdLog");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Picture", b =>
                {
                    b.Property<Guid>("IdPicture")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

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

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.User", b =>
                {
                    b.Property<Guid>("IdUser")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdUser");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.Picture", b =>
                {
                    b.HasOne("Slavestefan.Aphrodite.Model.PicturePool", null)
                        .WithMany("Pictures")
                        .HasForeignKey("PicturePoolIdPicturePool");

                    b.HasOne("Slavestefan.Aphrodite.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserIdUser");
                });

            modelBuilder.Entity("Slavestefan.Aphrodite.Model.PicturePool", b =>
                {
                    b.HasOne("Slavestefan.Aphrodite.Model.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerIdUser");

                    b.HasOne("Slavestefan.Aphrodite.Model.PostConfiguration", null)
                        .WithMany("Pool")
                        .HasForeignKey("PostConfigurationIdConfiguration");
                });
#pragma warning restore 612, 618
        }
    }
}
