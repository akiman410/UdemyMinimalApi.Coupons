using UdemyMinimalApi.Coupons.Models.DTOs;

namespace UdemyMinimalApi.Coupons.Repository.IRepository
{
    public interface IAuthRepository
    {
        bool IsUserUnique(string userName);
        Task<LoginResponseDTO> login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegistrationRequestDTO requestDTO);
    }
}
