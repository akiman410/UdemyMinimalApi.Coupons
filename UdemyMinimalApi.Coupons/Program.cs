using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Xml.Linq;
using UdemyMinimalApi.Coupons;
using UdemyMinimalApi.Coupons.Data;
using UdemyMinimalApi.Coupons.Models;
using UdemyMinimalApi.Coupons.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.MapGet("/api/coupon", (ApplicationDbContext _db, ILogger<Program> _logger) =>
{
    APIResponse response = new();

    _logger.Log(LogLevel.Information, "Getting All Coupons");

    response.Result = _db.Coupons;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("Get Coupons").Produces<APIResponse>(200);

app.MapGet("/api/coupon/{Id:int}", async (ApplicationDbContext _db, ILogger<Program> _logger, int Id) =>
{
    APIResponse response = new();

    _logger.Log(LogLevel.Information, $"Get coupon for {Id}");

    response.Result = await _db.Coupons.FirstOrDefaultAsync(x => x.Id == Id);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("Get Coupon").Produces<APIResponse>(200);

app.MapPost("/api/coupon", async (ApplicationDbContext _db, IMapper _mapper, ILogger<Program> _logger, IValidator<CouponCreateDTO> _validation, [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    APIResponse response = new()
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest,
        ErrorMessages = new List<string>()
    };

    
    var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
    if (!validationResult.IsValid)
    {
        //validationResult.Errors.FirstOrDefault()?.ToString() ?? string.Empty
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault()?.ToString() ?? string.Empty);
        return Results.BadRequest(response);
    }
    if (await _db.Coupons.FirstOrDefaultAsync(x => x.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon Name already Exists : " + coupon_C_DTO.Name.ToUpper());
        return Results.BadRequest(response);
    }
    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

    //coupon.Id = _db.Coupons.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
    _db.Coupons.Add(coupon);
    await _db.SaveChangesAsync();

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    _logger.Log(LogLevel.Information, $"Create New Coupon.");

    response.Result = couponDTO;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;
    return Results.Ok(response);

    //return Results.CreatedAtRoute("Get Coupon", new { id = coupon.Id }, couponDTO);
    //return Results.Created($"/api/coupon/{ coupon.Id}",coupon);
}
).WithName("Create Coupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);



app.MapPut("/api/coupon", async (ApplicationDbContext _db, IMapper _mapper, IValidator<CouponUpdateDTO> _validation, [FromBody] CouponUpdateDTO coupon_U_DTO) =>
{
    APIResponse response = new()
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest,
        ErrorMessages = new List<string>()
    };

    var validationResult = await _validation.ValidateAsync(coupon_U_DTO);
    if (!validationResult.IsValid)
    {
        //validationResult.Errors.FirstOrDefault()?.ToString() ?? string.Empty
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault()?.ToString() ?? string.Empty);
        return Results.BadRequest(response);
    }
    if (await _db.Coupons.FirstOrDefaultAsync(x => x.Name.ToLower() == coupon_U_DTO.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon Name already Exists : " + coupon_U_DTO.Name.ToUpper());
        return Results.BadRequest(response);
    }

    Coupon coupenFromStore = await _db.Coupons.FirstOrDefaultAsync(u => u.Id == coupon_U_DTO.Id);
    coupenFromStore.IsActive = coupon_U_DTO.IsActive;
    coupenFromStore.Name = coupon_U_DTO.Name;
    coupenFromStore.Percent = coupon_U_DTO.Percent;
    coupenFromStore.LastUpdated = DateTime.Now;

    await _db.SaveChangesAsync();

    response.Result = _mapper.Map<CouponDTO>(coupenFromStore);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}
).WithName("Update Coupon")
.Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);


app.MapDelete("/api/coupon/{Id:int}", async (ApplicationDbContext _db, int Id) =>
{
    APIResponse response = new()
    {
        IsSuccess = false,
        StatusCode = HttpStatusCode.BadRequest,
        ErrorMessages = new List<string>()
    };

    Coupon coupenFromStore = await _db.Coupons.FirstOrDefaultAsync(u => u.Id == Id);
    if (coupenFromStore != null)
    {
        _db.Coupons.Remove(coupenFromStore);
        await _db.SaveChangesAsync();

        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.NoContent;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessages.Add($"Invalid Id : {Id}");
        return Results.BadRequest(response);
    }
}
);

app.UseHttpsRedirection();



app.Run();
