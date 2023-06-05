

// Add services to the container.

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Test2.context;
using Test2.Extensions.common.helpers;
using Test2.models;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DefaultDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        x=>x.MigrationsAssembly(typeof(DefaultDbContext).Assembly.FullName)
    ));
builder.Services.AddIdentity<User, Role>(options =>
    {
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<DefaultDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "https://localhost:5101/",

        ValidateAudience = true,
        ValidAudience = "https://localhost:5101/",
        ValidateLifetime = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuerSigningKey = true,
    };

});

builder.Services.AddSwaggerGen(options =>
{ 
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer authentication with JWT token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

using (var scope = app.Services.CreateScope())
{
    var roleManager = (RoleManager<Role>)scope.ServiceProvider.GetService(typeof(RoleManager<Role>));
    CreateUserRoles.CreateRoles(roleManager).GetAwaiter().GetResult();
}

app.MapControllers();

app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.Run();