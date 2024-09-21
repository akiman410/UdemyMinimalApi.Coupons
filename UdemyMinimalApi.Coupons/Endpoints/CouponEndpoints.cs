using AutoMapper;
using FluentValidation;
using UdemyMinimalApi.Coupons.Models.DTOs;
using UdemyMinimalApi.Coupons.Models;
using UdemyMinimalApi.Coupons.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace UdemyMinimalApi.Coupons.Endpoints
{
    public static class CouponEndpoints
    {
        public static void ConfigureCouponEndpoints(this WebApplication app)
        {
            app.MapGet("/api/coupon", GetAllCoupon).WithName("Get Coupons").Produces<APIResponse>(200);

            app.MapGet("/api/coupon/{Id:int}", GetCoupon).WithName("Get Coupon").Produces<APIResponse>(200);

            app.MapPost("/api/coupon", CreateCoupon).WithName("Create Coupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

            app.MapPut("/api/coupon", UpdateCoupon).WithName("Update Coupon")
            .Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

            app.MapDelete("/api/coupon/{Id:int}", DeleteCoupon);
        }

        private static async Task<IResult> GetAllCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger)
        {
            APIResponse response = new();

            _logger.Log(LogLevel.Information, "Getting All Coupons");

            response.Result = await _couponRepo.GetAllAsync();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        private static async Task<IResult> DeleteCoupon(ICouponRepository _couponRepo, int Id)
        {

            APIResponse response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessages = new List<string>()
            };

            Coupon couponFromStore = await _couponRepo.GetAsync(Id);
            if (couponFromStore != null)
            {
                await _couponRepo.RemoveAsync(couponFromStore);
                await _couponRepo.SaveAsync();

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

        private static async Task<IResult> UpdateCoupon(ICouponRepository _couponRepo, IMapper _mapper, IValidator<CouponUpdateDTO> _validation, [FromBody] CouponUpdateDTO coupon_U_DTO)
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
            if (await _couponRepo.GetAsync(coupon_U_DTO.Name.ToLower()) != null)
            {
                response.ErrorMessages.Add("Coupon Name already Exists : " + coupon_U_DTO.Name.ToUpper());
                return Results.BadRequest(response);
            }

            await _couponRepo.UpdateAsync(_mapper.Map<Coupon>(coupon_U_DTO));
            await _couponRepo.SaveAsync();

            response.Result = _mapper.Map<CouponDTO>(await _couponRepo.GetAsync(coupon_U_DTO.Id));
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        private static async Task<IResult> GetCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger, int Id)
        {
            APIResponse response = new();

            _logger.Log(LogLevel.Information, $"Get coupon for {Id}");

            response.Result = await _couponRepo.GetAsync(Id);
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);

        }

        private static async Task<IResult> CreateCoupon(ICouponRepository _couponRepo, IMapper _mapper, ILogger<Program> _logger, IValidator<CouponCreateDTO> _validation, [FromBody] CouponCreateDTO coupon_C_DTO)
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
            if (await _couponRepo.GetAsync(coupon_C_DTO.Name.ToLower()) != null)
            {
                response.ErrorMessages.Add("Coupon Name already Exists : " + coupon_C_DTO.Name.ToUpper());
                return Results.BadRequest(response);
            }
            Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

            //coupon.Id = _db.Coupons.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            await _couponRepo.CreateAsync(coupon);
            await _couponRepo.SaveAsync();

            CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

            _logger.Log(LogLevel.Information, $"Create New Coupon.");

            response.Result = couponDTO;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Created;
            return Results.Ok(response);

            //return Results.CreatedAtRoute("Get Coupon", new { id = coupon.Id }, couponDTO);
            //return Results.Created($"/api/coupon/{ coupon.Id}",coupon);

        }
    }
}
