﻿using Microsoft.EntityFrameworkCore;

namespace ASP_1.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entity.User> Users { get; set; }
        public DbSet<Entity.EmailConfirmToken> EmailConfirmTokens { get; set; }
        public DbSet<Entity.Section> Sections { get; set; }
        public DbSet<Entity.Theme> Themes { get; set; }
        public DbSet<Entity.Topic> Topics { get; set; }
        public DbSet<Entity.Rate> Rates { get; set; }
        public DbSet<Entity.Post> Posts { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity.Rate>()
                .HasKey(
                nameof(Entity.Rate.ItemId), 
                nameof(Entity.Rate.UserId));

            modelBuilder.Entity<Entity.Section>()
                .HasOne(s => s.Author)
                .WithMany()
                .HasForeignKey(s => s.AuthorId);
            modelBuilder.Entity<Entity.Section>()
                .HasMany(s => s.RateList)
                .WithOne()
                .HasForeignKey(r => r.ItemId);


            modelBuilder.Entity<Entity.Theme>()
               .HasOne(s => s.Author)
               .WithMany()
               .HasForeignKey(s => s.AuthorId);
            modelBuilder.Entity<Entity.Post>()
              .HasOne(p => p.Author)
              .WithMany()
              .HasForeignKey(s => s.AuthorId);
            modelBuilder.Entity<Entity.Post>()
              .HasOne(p => p.Reply)
              .WithMany()
              .HasForeignKey(s => s.ReplyId);
        }
    }
}
