using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalBlogPlatform.Core.AutoMapperProfiles;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.Service;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Infrastructure.DbContext;
using PersonalBlogPlatform.Infrastructure.Repositories;
using PersonalBlogPlatform.Infrastructure.Service;
using PersonalBlogPlatform.API.Authorization;
using PersonalBlogPlatform.API.Filters;
using PersonalBlogPlatform.API.Middleware;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using PersonalBlogPlatform.Core.ServiceContracts; 

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

builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();

builder.Services.AddAutoMapper(typeof(PostAddRequestProfile).Assembly);

builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();

builder.Services.AddScoped<ICategoriesService, CategoriesService>(); 

builder.Services.AddScoped<IProfileService , ProfileService>();

builder.Services.AddAutoMapper(typeof(PostResponseProfile).Assembly);

builder.Services.AddAutoMapper(typeof(CommentAddProfileRequest).Assembly);

builder.Services.AddAutoMapper(typeof(CommentProfileResponse).Assembly);

builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

builder.Services.AddAutoMapper(typeof(CategoryAddProfileRequest).Assembly);

builder.Services.AddAutoMapper(typeof(CategoryProfileResponse).Assembly);

builder.Services.AddScoped<RegisterUseCase>();

builder.Services.AddScoped<LoginUseCase>();

builder.Services.AddScoped<RefreshTokenUseCase>();

builder.Services.AddScoped<PostDomainService>();

builder.Services.AddScoped<IPostCategoryService, PostCategoryService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<LoggingActionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); 

builder.Services.AddScoped<ITokenService , TokenService>();

//Jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters()
      {
          ValidateAudience = true,
          ValidateIssuer = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidAudience = builder.Configuration["Jwt:Audience"],
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
      };
  });

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHadlingMiddleware();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
