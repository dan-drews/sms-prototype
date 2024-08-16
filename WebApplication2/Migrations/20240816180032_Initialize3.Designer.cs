﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication2;

#nullable disable

namespace WebApplication2.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    [Migration("20240816180032_Initialize3")]
    partial class Initialize3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApplication2.SmsNotification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Metadata")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SmsNotifications");
                });

            modelBuilder.Entity("WebApplication2.SmsNotificationAction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ActionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SmsNotificationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SmsNotificationId");

                    b.ToTable("SmsNotificationActions");
                });

            modelBuilder.Entity("WebApplication2.SmsResponses", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SmsNotificationActionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SmsNotificationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TwilioPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SmsNotificationActionId");

                    b.HasIndex("SmsNotificationId");

                    b.ToTable("SmsResponses");
                });

            modelBuilder.Entity("WebApplication2.SmsNotificationAction", b =>
                {
                    b.HasOne("WebApplication2.SmsNotification", null)
                        .WithMany("Actions")
                        .HasForeignKey("SmsNotificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebApplication2.SmsResponses", b =>
                {
                    b.HasOne("WebApplication2.SmsNotificationAction", "SmsNotificationAction")
                        .WithMany()
                        .HasForeignKey("SmsNotificationActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication2.SmsNotification", "SmsNotification")
                        .WithMany()
                        .HasForeignKey("SmsNotificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SmsNotification");

                    b.Navigation("SmsNotificationAction");
                });

            modelBuilder.Entity("WebApplication2.SmsNotification", b =>
                {
                    b.Navigation("Actions");
                });
#pragma warning restore 612, 618
        }
    }
}
