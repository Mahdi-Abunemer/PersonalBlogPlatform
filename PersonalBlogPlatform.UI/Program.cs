using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Infrastructure.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.Run();
