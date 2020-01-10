using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ShortLink.sakila
{
    public partial class shortlinkContext : DbContext
    {
        public shortlinkContext(DbContextOptions<shortlinkContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Linktable> Linktable { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Fulltable> Fulltable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Linktable>(entity =>
            {
                entity.ToTable("linktable");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Longlink)
                    .HasName("longlink_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Shortlink)
                    .HasName("shortlink_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Deleted)
                    .HasColumnName("deleted")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Longlink)
                    .HasColumnName("longlink")
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Shortlink)
                    .HasColumnName("shortlink")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("logo");

                entity.HasIndex(e => e.IdLink)
                    .HasName("id_link_UNIQUE")
                    .IsUnique();


                entity.Property(e => e.Count)
                    .HasColumnName("count")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdLink)
                    .HasColumnName("id_link")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.IdLinkNavigation)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.IdLink)
                    .HasConstraintName("id_link");
            });

            modelBuilder.Entity<Fulltable>(entity =>
            {
                entity.ToTable("fulltable");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Longlink)
                    .HasName("longlink_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Shortlink)
                    .HasName("shortlink_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Deleted)
                    .HasColumnName("deleted")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Longlink)
                    .HasColumnName("longlink")
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Shortlink)
                    .HasColumnName("shortlink")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Count)
                   .HasColumnName("counter")
                   .HasColumnType("int(11)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
