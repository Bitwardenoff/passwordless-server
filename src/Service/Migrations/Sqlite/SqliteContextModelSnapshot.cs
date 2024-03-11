﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Passwordless.Service.Storage.Ef;

#nullable disable

namespace Passwordless.Service.Migrations.Sqlite
{
    [DbContext(typeof(DbGlobalSqliteContext))]
    partial class SqliteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("Passwordless.Service.EventLog.Models.ApplicationEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiKeyId")
                        .HasColumnType("TEXT");

                    b.Property<int>("EventType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PerformedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("PerformedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Severity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("ApplicationEvents");
                });

            modelBuilder.Entity("Passwordless.Service.Models.AccountMetaInformation", b =>
                {
                    b.Property<string>("AcountName")
                        .HasColumnType("TEXT");

                    b.Property<string>("AdminEmailsSerialized")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeleteAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tenant")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AcountName");

                    b.ToTable("AccountInfo");
                });

            modelBuilder.Entity("Passwordless.Service.Models.AliasPointer", b =>
                {
                    b.Property<string>("Tenant")
                        .HasColumnType("TEXT");

                    b.Property<string>("Alias")
                        .HasColumnType("TEXT");

                    b.Property<string>("Plaintext")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Tenant", "Alias");

                    b.ToTable("Aliases");
                });

            modelBuilder.Entity("Passwordless.Service.Models.ApiKeyDesc", b =>
                {
                    b.Property<string>("Tenant")
                        .HasColumnType("TEXT");

                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastLockedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastUnlockedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Scopes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Tenant", "Id");

                    b.ToTable("ApiKeys");
                });

            modelBuilder.Entity("Passwordless.Service.Models.AppFeature", b =>
                {
                    b.Property<string>("Tenant")
                        .HasColumnType("TEXT");

                    b.Property<bool>("AllowAttestation")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DeveloperLoggingEndsAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EventLoggingIsEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventLoggingRetentionPeriod")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsGenerateSignInTokenEndpointEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsMagicLinksEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(true);

                    b.Property<int>("MagicLinkEmailMonthlyQuota")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("MaxUsers")
                        .HasColumnType("INTEGER");

                    b.HasKey("Tenant");

                    b.ToTable("AppFeatures");
                });

            modelBuilder.Entity("Passwordless.Service.Models.Archive", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasMaxLength(104857600)
                        .HasColumnType("BLOB");

                    b.Property<string>("Entity")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("JobId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tenant")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.HasIndex("Tenant", "JobId", "Id");

                    b.ToTable("Archives");
                });

            modelBuilder.Entity("Passwordless.Service.Models.ArchiveJob", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<short>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tenant")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Tenant");

                    b.ToTable("ArchiveJobs");
                });

            modelBuilder.Entity("Passwordless.Service.Models.Authenticator", b =>
                {
                    b.Property<string>("Tenant")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AaGuid")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAllowed")
                        .HasColumnType("INTEGER");

                    b.HasKey("Tenant", "AaGuid");

                    b.ToTable("Authenticators");
                });

            modelBuilder.Entity("Passwordless.Service.Models.DispatchedEmail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LinkTemplate")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Tenant")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Tenant");

                    b.ToTable("DispatchedEmails");
                });

            modelBuilder.Entity("Passwordless.Service.Models.EFStoredCredential", b =>
                {
                    b.Property<string>("Tenant")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("DescriptorId")
                        .HasColumnType("BLOB");

                    b.Property<Guid?>("AaGuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("AttestationFmt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool?>("BackupState")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("DescriptorTransports")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DescriptorType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Device")
                        .HasColumnType("TEXT");

                    b.Property<bool?>("IsBackupEligible")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsDiscoverable")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUsedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nickname")
                        .HasColumnType("TEXT");

                    b.Property<string>("Origin")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PublicKey")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("RPID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<uint>("SignatureCounter")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("UserHandle")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Tenant", "DescriptorId");

                    b.ToTable("Credentials");
                });

            modelBuilder.Entity("Passwordless.Service.Models.PeriodicActiveUserReport", b =>
                {
                    b.Property<string>("Tenant")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("DailyActiveUsersCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalUsersCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WeeklyActiveUsersCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Tenant", "CreatedAt");

                    b.ToTable("PeriodicActiveUserReports");
                });

            modelBuilder.Entity("Passwordless.Service.Models.PeriodicCredentialReport", b =>
                {
                    b.Property<string>("Tenant")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("CredentialsCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UsersCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Tenant", "CreatedAt");

                    b.ToTable("PeriodicCredentialReports");
                });

            modelBuilder.Entity("Passwordless.Service.Models.TokenKey", b =>
                {
                    b.Property<string>("Tenant")
                        .HasColumnType("TEXT");

                    b.Property<int>("KeyId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("KeyMaterial")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Tenant", "KeyId");

                    b.ToTable("TokenKeys");
                });

            modelBuilder.Entity("Passwordless.Service.EventLog.Models.ApplicationEvent", b =>
                {
                    b.HasOne("Passwordless.Service.Models.AccountMetaInformation", "Application")
                        .WithMany("Events")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");
                });

            modelBuilder.Entity("Passwordless.Service.Models.AppFeature", b =>
                {
                    b.HasOne("Passwordless.Service.Models.AccountMetaInformation", "Application")
                        .WithOne("Features")
                        .HasForeignKey("Passwordless.Service.Models.AppFeature", "Tenant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");
                });

            modelBuilder.Entity("Passwordless.Service.Models.Archive", b =>
                {
                    b.HasOne("Passwordless.Service.Models.ArchiveJob", "Job")
                        .WithMany("Archives")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Passwordless.Service.Models.AccountMetaInformation", "Application")
                        .WithMany("Archives")
                        .HasForeignKey("Tenant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");

                    b.Navigation("Job");
                });

            modelBuilder.Entity("Passwordless.Service.Models.ArchiveJob", b =>
                {
                    b.HasOne("Passwordless.Service.Models.AccountMetaInformation", "Application")
                        .WithMany("ArchiveJobs")
                        .HasForeignKey("Tenant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");
                });

            modelBuilder.Entity("Passwordless.Service.Models.Authenticator", b =>
                {
                    b.HasOne("Passwordless.Service.Models.AppFeature", "AppFeature")
                        .WithMany("Authenticators")
                        .HasForeignKey("Tenant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppFeature");
                });

            modelBuilder.Entity("Passwordless.Service.Models.DispatchedEmail", b =>
                {
                    b.HasOne("Passwordless.Service.Models.AccountMetaInformation", "Application")
                        .WithMany("DispatchedEmails")
                        .HasForeignKey("Tenant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");
                });

            modelBuilder.Entity("Passwordless.Service.Models.PeriodicActiveUserReport", b =>
                {
                    b.HasOne("Passwordless.Service.Models.AccountMetaInformation", "Application")
                        .WithMany("PeriodicActiveUserReports")
                        .HasForeignKey("Tenant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");
                });

            modelBuilder.Entity("Passwordless.Service.Models.PeriodicCredentialReport", b =>
                {
                    b.HasOne("Passwordless.Service.Models.AccountMetaInformation", "Application")
                        .WithMany("PeriodicCredentialReports")
                        .HasForeignKey("Tenant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");
                });

            modelBuilder.Entity("Passwordless.Service.Models.AccountMetaInformation", b =>
                {
                    b.Navigation("ArchiveJobs");

                    b.Navigation("Archives");

                    b.Navigation("DispatchedEmails");

                    b.Navigation("Events");

                    b.Navigation("Features");

                    b.Navigation("PeriodicActiveUserReports");

                    b.Navigation("PeriodicCredentialReports");
                });

            modelBuilder.Entity("Passwordless.Service.Models.AppFeature", b =>
                {
                    b.Navigation("Authenticators");
                });

            modelBuilder.Entity("Passwordless.Service.Models.ArchiveJob", b =>
                {
                    b.Navigation("Archives");
                });
#pragma warning restore 612, 618
        }
    }
}
