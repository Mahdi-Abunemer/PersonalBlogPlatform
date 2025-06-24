using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PersonalBlogPlatform.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>().ToTable("Posts");
            builder.Entity<Category>().ToTable("Categories");
            builder.Entity<Comment>().ToTable("Comments");

            // seed data
            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Id = Guid.Parse("172A5A39-14F4-4B06-8C1F-5CD22028253D"),
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = Guid.Parse("58ABD188-A359-4443-9644-A926B699B305"),
                UserName = "admin@blog.com",
                NormalizedUserName = "ADMIN@BLOG.COM",
                Email = "admin@blog.com",
                NormalizedEmail = "ADMIN@BLOG.COM",
                EmailConfirmed = true,
                DisplayName = "Site Admin",
                PasswordHash = "AQAAAAIAAYagAAAAEM0WcE+XS0GA7sTtQ4NZZVi8gQsw/kiazmMcN3WRtdHVvpyyZ6ZBhtIFFQNbMKOjkw=="
            });

            builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = Guid.Parse("172A5A39-14F4-4B06-8C1F-5CD22028253D"),
                UserId = Guid.Parse("58ABD188-A359-4443-9644-A926B699B305")
            });

            var seedFolder = Path.Combine(AppContext.BaseDirectory, "SeedData");
            var categoriesJson = File.ReadAllText(Path.Combine(seedFolder, "seed-categories.json"));
            var categories = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(categoriesJson);
            foreach (var category in categories)
                builder.Entity<Category>().HasData(category);

            var postsJson = File.ReadAllText(Path.Combine(seedFolder, "seed-posts.json"));
            var posts = System.Text.Json.JsonSerializer.Deserialize<List<Post>>(postsJson);

            var joinData = posts
                .SelectMany(p => p.Categories.Select(c => new
                {
                    PostsId = p.Id,
                    CategoriesId = c.Id
                }))
                .ToArray();

            foreach (var post in posts)
            {
                post.Categories = null;
                builder.Entity<Post>().HasData(post);
            }

            builder.Entity<Category>()
                .HasMany(c => c.Posts)
                .WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryPost",
                    right => right.HasOne<Post>().WithMany().HasForeignKey("PostsId"),
                    left => left.HasOne<Category>().WithMany().HasForeignKey("CategoriesId")
                )
                .HasData(joinData);

            builder.Entity<Post>()
             .HasOne(p => p.Author)
             .WithMany(u => u.Posts)    
             .HasForeignKey(p => p.AuthorId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
