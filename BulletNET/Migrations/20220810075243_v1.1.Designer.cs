// <auto-generated />
using System;
using BulletNET.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BulletNET.Migrations
{
    [DbContext(typeof(DatabaseDB))]
    [Migration("20220810075243_v1.1")]
    partial class v11
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("BulletNET.EntityFramework.Entities.Radar.RadarBoard", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnOrder(0);

                    b.Property<string>("mainBoardID")
                        .IsRequired()
                        .HasColumnType("tinytext")
                        .HasColumnName("mainBoardID");

                    b.Property<string>("radarBoardID")
                        .IsRequired()
                        .HasColumnType("tinytext")
                        .HasColumnName("radarBoardID");

                    b.HasKey("ID");

                    b.ToTable("MainRadarBoardPairing");
                });

            modelBuilder.Entity("BulletNET.EntityFramework.Entities.Radar.TestAction", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnOrder(0);

                    b.Property<bool?>("IsPassed")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("pass");

                    b.Property<string>("LastComment")
                        .HasColumnType("longtext")
                        .HasColumnName("LastComment");

                    b.Property<double?>("Maximum")
                        .HasColumnType("double")
                        .HasColumnName("maximum");

                    b.Property<double?>("Measured")
                        .HasColumnType("double")
                        .HasColumnName("measured");

                    b.Property<double?>("Minimum")
                        .HasColumnType("double")
                        .HasColumnName("minimum");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("valueName");

                    b.Property<string>("Param1")
                        .HasColumnType("longtext")
                        .HasColumnName("parameter1");

                    b.Property<string>("Param2")
                        .HasColumnType("longtext")
                        .HasColumnName("parameter2");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("datetime");

                    b.Property<int>("group_id")
                        .HasColumnType("int(11)");

                    b.Property<int?>("userID")
                        .HasColumnType("int(11)");

                    b.HasKey("ID");

                    b.HasIndex("TimeStamp")
                        .IsUnique();

                    b.HasIndex("group_id");

                    b.HasIndex("userID");

                    b.ToTable("TestAction");
                });

            modelBuilder.Entity("BulletNET.EntityFramework.Entities.Radar.TestGroup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnOrder(0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("UserID")
                        .HasColumnType("int(11)");

                    b.Property<DateTime>("datetime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("datetime");

                    b.Property<int>("radarboardID")
                        .HasColumnType("int(11)");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.HasIndex("datetime")
                        .IsUnique();

                    b.HasIndex("radarboardID");

                    b.ToTable("TestGroup");
                });

            modelBuilder.Entity("BulletNET.EntityFramework.Entities.Users.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnOrder(0);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("USER_DESC");

                    b.Property<string>("Hashcode")
                        .IsRequired()
                        .HasColumnType("char(64)")
                        .HasColumnName("USER_HASH");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("USER_NAME")
                        .HasColumnOrder(1);

                    b.Property<int>("RoleNum")
                        .HasColumnType("int")
                        .HasColumnName("USER_ROLE");

                    b.HasKey("ID");

                    b.ToTable("PROG_USER");
                });

            modelBuilder.Entity("BulletNET.EntityFramework.Entities.Radar.TestAction", b =>
                {
                    b.HasOne("BulletNET.EntityFramework.Entities.Radar.TestGroup", "TestGroup")
                        .WithMany("TestActionsList")
                        .HasForeignKey("group_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BulletNET.EntityFramework.Entities.Users.User", "user")
                        .WithMany()
                        .HasForeignKey("userID");

                    b.Navigation("TestGroup");

                    b.Navigation("user");
                });

            modelBuilder.Entity("BulletNET.EntityFramework.Entities.Radar.TestGroup", b =>
                {
                    b.HasOne("BulletNET.EntityFramework.Entities.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BulletNET.EntityFramework.Entities.Radar.RadarBoard", "radarboard")
                        .WithMany()
                        .HasForeignKey("radarboardID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("radarboard");
                });

            modelBuilder.Entity("BulletNET.EntityFramework.Entities.Radar.TestGroup", b =>
                {
                    b.Navigation("TestActionsList");
                });
#pragma warning restore 612, 618
        }
    }
}
