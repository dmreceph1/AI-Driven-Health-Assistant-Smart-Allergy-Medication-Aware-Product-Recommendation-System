using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using WebApi.Dtos.AllergyDto;
using WebApi.Models.PContext;
using WebApi.Repositories.AllergyRepository;
using WebApi.Repositories.MedicationRepository;
using WebApi.Repositories.ProductContentsRepository;
using WebApi.Repositories.ProductRepository;
using WebApi.Repositories.UserAllergyRepository;
using WebApi.Repositories.UserMedicationsRepository;
using WebApi.Repositories.UserPhysicalInfoRepository;
using WebApi.Repositories.UserRepository;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<Context>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IAllergyRepository, AllergyRepository>();
builder.Services.AddTransient<IMedicationRepository, MedicationRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IProductContentsRepository, ProductContentsRepository>();
builder.Services.AddTransient<IUserPhysicalInfoRepository, UserPhysicalInfoRepository>();
builder.Services.AddTransient<IUserAllergyRepository, UserAllergyRepository>();
builder.Services.AddTransient<IUserMedicationsRepository, UserMedicationsRepository>();
builder.Services.AddTransient<AuthService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
