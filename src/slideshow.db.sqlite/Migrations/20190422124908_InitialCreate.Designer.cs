﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using slideshow.db;

namespace slideshow.db.sqlite.Migrations
{
    [DbContext(typeof(SqliteSlideshowContext))]
    [Migration("20190422124908_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085");

            modelBuilder.Entity("slideshow.data.models.Section", b =>
                {
                    b.Property<int>("SectionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Class");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("Order");

                    b.HasKey("SectionId");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("slideshow.data.models.Slide", b =>
                {
                    b.Property<int>("SlideId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("Header")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("Order");

                    b.Property<int>("SectionId");

                    b.Property<string>("Template");

                    b.HasKey("SlideId");

                    b.HasIndex("SectionId");

                    b.ToTable("Slides");
                });

            modelBuilder.Entity("slideshow.data.models.Slide", b =>
                {
                    b.HasOne("slideshow.data.models.Section", "Section")
                        .WithMany("Slides")
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
