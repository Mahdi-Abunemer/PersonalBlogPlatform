using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.Service;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Infrastructure.DbContext;
using PersonalBlogPlatform.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IPostsRepository , PostsRepository>();

builder.Services.AddScoped<IPostsService, PostsService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.Run();
