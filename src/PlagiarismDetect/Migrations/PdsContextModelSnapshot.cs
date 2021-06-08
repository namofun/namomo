﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SatelliteSite;

namespace SatelliteSite.Migrations
{
    [DbContext(typeof(PdsContext))]
    partial class PdsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Plag.Backend.Entities.PlagiarismSet<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int?>("ContestId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("ReportCount")
                        .HasColumnType("integer");

                    b.Property<int>("ReportPending")
                        .HasColumnType("integer");

                    b.Property<int>("SubmissionCount")
                        .HasColumnType("integer");

                    b.Property<int>("SubmissionFailed")
                        .HasColumnType("integer");

                    b.Property<int>("SubmissionSucceeded")
                        .HasColumnType("integer");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.HasIndex("UserId");

                    b.ToTable("PlagiarismSets");
                });

            modelBuilder.Entity("Plag.Backend.Entities.Report<System.Guid>", b =>
                {
                    b.Property<Guid>("SetId")
                        .HasColumnType("uuid");

                    b.Property<int>("SubmissionA")
                        .HasColumnType("integer");

                    b.Property<int>("SubmissionB")
                        .HasColumnType("integer");

                    b.Property<int>("BiggestMatch")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uuid");

                    b.Property<bool?>("Finished")
                        .HasColumnType("boolean");

                    b.Property<bool?>("Justification")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("Matches")
                        .HasColumnType("bytea");

                    b.Property<double>("Percent")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("double precision")
                        .HasDefaultValue(0.0);

                    b.Property<double>("PercentA")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("double precision")
                        .HasDefaultValue(0.0);

                    b.Property<double>("PercentB")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("double precision")
                        .HasDefaultValue(0.0);

                    b.Property<int>("TokensMatched")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("SetId", "SubmissionA", "SubmissionB");

                    b.HasAlternateKey("ExternalId");

                    b.HasIndex("SetId", "SubmissionB");

                    b.ToTable("PlagiarismReports");
                });

            modelBuilder.Entity("Plag.Backend.Entities.Submission<System.Guid>", b =>
                {
                    b.Property<Guid>("SetId")
                        .HasColumnType("uuid");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<int>("ExclusiveCategory")
                        .HasColumnType("integer");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uuid");

                    b.Property<int>("InclusiveCategory")
                        .HasColumnType("integer");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<double>("MaxPercent")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool?>("TokenProduced")
                        .HasColumnType("boolean");

                    b.Property<byte[]>("Tokens")
                        .HasColumnType("bytea");

                    b.Property<DateTimeOffset>("UploadTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("SetId", "Id");

                    b.ToTable("PlagiarismSubmissions");
                });

            modelBuilder.Entity("Plag.Backend.Entities.SubmissionFile<System.Guid>", b =>
                {
                    b.Property<Guid>("SubmissionId")
                        .HasColumnType("uuid");

                    b.Property<int>("FileId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<string>("FilePath")
                        .HasColumnType("text");

                    b.HasKey("SubmissionId", "FileId");

                    b.ToTable("PlagiarismFiles");
                });

            modelBuilder.Entity("Plag.Backend.Entities.Report<System.Guid>", b =>
                {
                    b.HasOne("Plag.Backend.Entities.PlagiarismSet<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Plag.Backend.Entities.Submission<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("SetId", "SubmissionA")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Plag.Backend.Entities.Submission<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("SetId", "SubmissionB")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Plag.Backend.Entities.Submission<System.Guid>", b =>
                {
                    b.HasOne("Plag.Backend.Entities.PlagiarismSet<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Plag.Backend.Entities.SubmissionFile<System.Guid>", b =>
                {
                    b.HasOne("Plag.Backend.Entities.Submission<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("SubmissionId")
                        .HasPrincipalKey("ExternalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
