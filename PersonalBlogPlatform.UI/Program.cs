using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Core.AutoMapperProfiles;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.Service;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Infrastructure.DbContext;
using PersonalBlogPlatform.Infrastructure.Repositories;
using PersonalBlogPlatform.UI.Authorization;
using PersonalBlogPlatform.UI.Filters;
using PersonalBlogPlatform.UI.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders()
                 .AddUserStore<UserStore<ApplicationUser, ApplicationRole,
                   ApplicationDbContext, Guid>>()
                 .AddRoleStore<RoleStore<ApplicationRole,
                   ApplicationDbContext, Guid>>(); 

builder.Services.AddSingleton<IAuthorizationHandler, AdminRequirementHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p =>
    {
        p.RequireRole("Admin");
        p.Requirements.Add(new AdminRequirement());
    });
});

builder.Services.AddScoped<IPostsRepository , PostsRepository>();

builder.Services.AddScoped<IPostsService, PostsService>();

builder.Services.AddAutoMapper(typeof(PostAddRequestProfile).Assembly);

builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();

builder.Services.AddScoped<IProfileService , ProfileService>();

builder.Services.AddAutoMapper(typeof(PostResponseProfile).Assembly);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<LoggingActionFilter>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHadlingMiddleware();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
