using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Xml.Linq;
using UdemyMinimalApi.Coupons;
using UdemyMinimalApi.Coupons.Data;
using UdemyMinimalApi.Coupons.Endpoints;
using UdemyMinimalApi.Coupons.Models;
using UdemyMinimalApi.Coupons.Models.DTOs;
using UdemyMinimalApi.Coupons.Repository;
using UdemyMinimalApi.Coupons.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthentication();
//app.UseAuthorization();

app.ConfigureCouponEndpoints();
app.ConfigureAuthEndpoints();
app.UseHttpsRedirection();



app.Run();
