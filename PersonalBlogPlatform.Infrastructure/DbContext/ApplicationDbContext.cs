using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser , ApplicationRole ,Guid>
    {
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Post>().ToTable("Posts");
            builder.Entity<Category>().ToTable("Categories");
            builder.Entity<Comment>().ToTable("Comments");

            //Seed Data 
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
            List<Category>? categories = System.Text.Json.JsonSerializer.Deserialize<List<Category>>
                ((categoriesJson));   
                foreach (var category in categories)
                    builder.Entity<Category>().HasData(category);
                
            var postsJson = File.ReadAllText(Path.Combine(seedFolder, "seed-posts.json"));
            List<Post>? posts = System.Text.Json.JsonSerializer.Deserialize<List<Post>>
                ((postsJson));
            foreach (var post in posts)
                builder.Entity<Post>().HasData(post);

        }
    }
}
