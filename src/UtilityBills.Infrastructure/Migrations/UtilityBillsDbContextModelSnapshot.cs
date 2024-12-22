﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UtilityBills.Infrastructure;

#nullable disable

namespace UtilityBills.Infrastructure.Migrations
{
    [DbContext(typeof(UtilityBillsDbContext))]
    partial class UtilityBillsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities.UtilityPaymentPlatformCredential", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UtilityPaymentPlatformId")
                        .HasColumnType("TEXT");

                    b.ComplexProperty<Dictionary<string, object>>("Email", "UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities.UtilityPaymentPlatformCredential.Email#Email", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(512)
                                .HasColumnType("TEXT")
                                .HasColumnName("Email");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Password", "UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities.UtilityPaymentPlatformCredential.Password#Password", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("Password");
                        });

                    b.HasKey("Id");

                    b.HasIndex("UtilityPaymentPlatformId");

                    b.ToTable("UtilityPaymentPlatformCredentials");
                });

            modelBuilder.Entity("UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.UtilityPaymentPlatform", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<int>("PlatformType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("UtilityPaymentPlatforms");
                });

            modelBuilder.Entity("UtilityBills.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities.UtilityPaymentPlatformCredential", b =>
                {
                    b.HasOne("UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.UtilityPaymentPlatform", null)
                        .WithMany("Credentials")
                        .HasForeignKey("UtilityPaymentPlatformId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.UtilityPaymentPlatform", b =>
                {
                    b.Navigation("Credentials");
                });
#pragma warning restore 612, 618
        }
    }
}
