using UdemyMinimalApi.Coupons.Models.DTOs;
using UdemyMinimalApi.Coupons.Models;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UdemyMinimalApi.Coupons.Repository.IRepository;
using Microsoft.Win32;
using Microsoft.IdentityModel.Tokens;

namespace UdemyMinimalApi.Coupons.Endpoints
{
    public static class AuthEndpoints
    {
        public static void ConfigureAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/Login", Login).WithName("Login").Accepts<LoginRequestDTO>("application/json").Produces<APIResponse>(200).Produces(400);
            app.MapPost("/api/Register", Register).WithName("Register").Accepts<RegistrationRequestDTO>("application/json").Produces<APIResponse>(200).Produces(400);
        }
        private static async Task<IResult> Login(IAuthRepository _authRepo, ILogger<Program> _logger, [FromBody] LoginRequestDTO model)
        {
            APIResponse response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessages = new List<string>()
            };

            var loginResponse = await _authRepo.login(model);

            if(loginResponse == null)
            {
                response.ErrorMessages.Add("User or Password is incorrect");
                return Results.BadRequest(response);
            }

            _logger.Log(LogLevel.Information, $"login successful for {loginResponse.User}");

            response.Result = loginResponse;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }
        private static async Task<IResult> Register(IAuthRepository _authRepo, ILogger<Program> _logger, [FromBody] RegistrationRequestDTO model)
        {
            APIResponse response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessages = new List<string>()
            };

            bool ifUserNameIsUnique = _authRepo.IsUserUnique(model.Username);

            if(!ifUserNameIsUnique)
            {
                response.ErrorMessages.Add("Username Already Exists");
                return Results.BadRequest(response);
            }
            var registerResponse = await _authRepo.Register(model);
            if (registerResponse == null||  string.IsNullOrEmpty(registerResponse.Username))
            {
                response.ErrorMessages.Add("User or Password is incorrect");
                return Results.BadRequest(response);
            }

            _logger.Log(LogLevel.Information, $"Register new User successful for {registerResponse.Username}");

            //response.Result = registerResponse;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }
    }
}
