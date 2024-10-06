using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UdemyMinimalApi.Coupons.Data;
using UdemyMinimalApi.Coupons.Models;
using UdemyMinimalApi.Coupons.Models.DTOs;
using UdemyMinimalApi.Coupons.Repository.IRepository;

namespace UdemyMinimalApi.Coupons.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private  readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private string secretKey;
        public AuthRepository(ApplicationDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUserUnique(string userName)
        {
            //go to DB and find whether the userName entered exists
            var user = _db.LocalUsers.SingleOrDefault(x => x.Username == userName);

            //If the user does not exist in the database return true because the user is unique
            if (user == null)
            {
                return true;
            }
            return false;

        }

        public async Task<LoginResponseDTO>login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.Username == loginRequestDTO.Username 
            && x.Password == loginRequestDTO.Password);
            if (user == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO = new()
            {
                User = _mapper.Map<UserDTO>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
            };
            return loginResponseDTO;
        }

        public async Task<UserDTO> Register(RegistrationRequestDTO requestDTO)
        {
            LocalUser userObj = new()
            {
                Username = requestDTO.Username,
                Password = requestDTO.Password,
                Name = requestDTO.Name,
                Role = "Admin"
            };

            _db.LocalUsers.Add(userObj);
            _db.SaveChanges();
            userObj.Password = "";
            return _mapper.Map<UserDTO>(userObj);
        }
    }
}
