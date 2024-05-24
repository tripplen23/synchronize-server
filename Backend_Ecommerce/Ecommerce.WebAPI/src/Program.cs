using System.Text;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.Service;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.Service.src.Shared;
using Ecommerce.WebAPI.src.AuthorizationPolicy;
using Ecommerce.WebAPI.src.Database;
using Ecommerce.WebAPI.src.Middleware;
using Ecommerce.WebAPI.src.Repo;
using Ecommerce.WebAPI.src.ExternalService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(
  options =>
  {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
      Description = "Bearer token authentication",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Scheme = "Bearer"
    }
    );

    // swagger would add the token to the request header of routes with [Authorize] attribute
    options.OperationFilter<SecurityRequirementsOperationFilter>();
  }
);

// Add CORS services
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAllOrigins",
    builder =>
    {
      builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// add all controllers
builder.Services.AddControllers();

// add database service
var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Remote")); // Change to "Localhost" to use the local postgre database
dataSourceBuilder.MapEnum<UserRole>();
dataSourceBuilder.MapEnum<OrderStatus>();
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<AppDbContext>
(
  options =>
  options
    .UseNpgsql(dataSource)
    .UseSnakeCaseNamingConvention()
    .AddInterceptors(new TimeStampInterceptor())
    .EnableSensitiveDataLogging() // For Development
    .EnableDetailedErrors() // For Development
);


// add automapper service
builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

// add DI services
builder.Services
  .AddScoped<IProductImageService, ProductImageService>()
  .AddScoped<IUserService, UserService>()
  .AddScoped<IProductService, ProductService>()
  .AddScoped<ICategoryService, CategoryService>()
  .AddScoped<IOrderService, OrderService>()
  .AddScoped<ICartService, CartService>()
  .AddScoped<ICartItemService, CartItemService>()
  .AddScoped<IReviewService, ReviewService>()
  .AddScoped<IAuthService, AuthService>()
  .AddScoped<ITokenService, TokenService>()
  .AddScoped<IPasswordService, PasswordService>()
;

builder.Services
  .AddScoped<IProductImageRepo, ProductImageRepo>()
  .AddScoped<IUserRepo, UserRepo>()
  .AddScoped<IProductRepo, ProductRepo>()
  .AddScoped<ICategoryRepo, CategoryRepo>()
  .AddScoped<IOrderRepo, OrderRepo>()
  .AddScoped<ICartRepo, CartRepo>()
  .AddScoped<ICartItemRepo, CartItemRepo>()
  .AddScoped<IReviewRepo, ReviewRepo>()
;

// add DI custom authorization services
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrOwnerAccountHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrOwnerOrderHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrOwnerCartHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrOwnerReviewHandler>();

builder.Services.AddScoped<ExceptionHandlerMiddleware>(serviceProvider =>
{
  var logger = serviceProvider.GetRequiredService<ILogger<ExceptionHandlerMiddleware>>();
  return new ExceptionHandlerMiddleware(next =>
  {
    var requestDelegate = serviceProvider.GetRequiredService<RequestDelegate>();
    return Task.CompletedTask;
  }, logger);
}); // Catching database exception

// add authentication instructions
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidIssuer = builder.Configuration["Secrets:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Secrets:JwtKey"]!)),
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true, // make sure it's not expired
        ValidateIssuerSigningKey = true,
      };
    }
);

// Add authorization instructions
builder.Services.AddAuthorization(
    policy =>
    {
      policy.AddPolicy("ResourceOwner", policy => policy.Requirements.Add(new VerifyResourceOwnerRequirement()));
      policy.AddPolicy("AdminOrOwnerAccount", policy => policy.Requirements.Add(new AdminOrOwnerAccountRequirement()));
      policy.AddPolicy("AdminOrOwnerOrder", policy => policy.Requirements.Add(new AdminOrOwnerOrderRequirement()));
      policy.AddPolicy("AdminOrOwnerCart", policy => policy.Requirements.Add(new AdminOrOwnerCartRequirement()));
      policy.AddPolicy("AdminOrOwnerReview", policy => policy.Requirements.Add(new AdminOrOwnerReviewRequirement()));
    }
);

// build app
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
  options.RoutePrefix = string.Empty;
});

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();