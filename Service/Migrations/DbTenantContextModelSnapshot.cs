﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Service.Migrations
{
    [DbContext(typeof(DbTenantContext))]
    partial class DbTenantContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.2");

            modelBuilder.Entity("AliasPointer", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Alias")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "Alias");

                    b.ToTable("Aliases");
                });

            modelBuilder.Entity("EFStoredCredential", b =>
                {
                    b.Property<byte[]>("DescriptorId")
                        .HasColumnType("BLOB");

                    b.Property<Guid>("AaGuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("CredType")
                        .HasColumnType("TEXT");

                    b.Property<string>("DescriptorTransports")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DescriptorType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Device")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUsedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nickname")
                        .HasColumnType("TEXT");

                    b.Property<string>("Origin")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PublicKey")
                        .HasColumnType("BLOB");

                    b.Property<string>("RPID")
                        .HasColumnType("TEXT");

                    b.Property<uint>("SignatureCounter")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("UserHandle")
                        .HasColumnType("BLOB");

                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("DescriptorId");

                    b.ToTable("Credentials");
                });

            modelBuilder.Entity("Service.Models.AccountMetaInformation", b =>
                {
                    b.Property<string>("AcountName")
                        .HasColumnType("TEXT");

                    b.Property<string>("AdminEmailsSerialized")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("SubscriptionTier")
                        .HasColumnType("TEXT");

                    b.HasKey("AcountName");

                    b.ToTable("AccountInfo");
                });

            modelBuilder.Entity("Service.Models.TokenKey", b =>
                {
                    b.Property<int>("KeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("KeyMaterial")
                        .HasColumnType("TEXT");

                    b.HasKey("KeyId");

                    b.ToTable("TokenKeys");
                });

            modelBuilder.Entity("Service.Storage.ApiKeyDesc", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiKey")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Scopes")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ApiKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
