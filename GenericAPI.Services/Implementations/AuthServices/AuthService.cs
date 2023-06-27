using GenericAPI.Domain.Models;
using GenericAPI.Repository.Abstractions;
using GenericAPI.Services.Abstractions.AuthServices;
using GenericAPI.Services.Models.AuthServiceModels.RequestModel;
using GenericAPI.Services.Models.AuthServiceModels.ResponseModel;
using Microsoft.AspNetCore.Http;

namespace GenericAPI.Services.Implementations.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher _hasher;
        private readonly IBaseRepository<Player> _repo;
        private readonly IJwtService _jwtService;
        public AuthService(IPasswordHasher hasher, IBaseRepository<Player> repo, IJwtService jwtService)
        {
            _hasher = hasher;
            _repo = repo;
            _jwtService = jwtService;
        }

        public async Task<Dictionary<byte[], byte[]>> GetHashandSalt(string mail)
        {
            byte[] passHash;
            byte[] passSalt;
            var result = new Dictionary<byte[], byte[]>();
            var user = (await _repo.FindAsync(p => p.Equals(mail))).FirstOrDefault();

            if (user != null)
            {
                passHash = user.PasswordHash;
                passSalt = user.PasswordSalt;
                result.Add(passHash, passSalt);
                return result;
            }

            return result;
        }
        public async Task<LoginResponse> LoginUser(LoginModel request)
        {
            var user = (await _repo.FindAsync(u => u.Mail == request.Mail))
                .FirstOrDefault();

            if (user == null)
            {
                return new LoginResponse { StatusCode = StatusCodes.Status400BadRequest, Token = null, Message = "Email or password is incorrect" };
            }

            if (!_hasher.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return new LoginResponse { StatusCode = StatusCodes.Status400BadRequest, Token = null, Message = "Email or password is incorrect" };
            }

            var token = _jwtService.GenerateToken(user.Id.ToString());

            return new LoginResponse
            {
                Token = token,
                Message = "Token created successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<RegisterResponse> RegisterUserAsync(UserDto request)
        {
            _hasher.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new Player()
            {
                Name = request.Name,
                Mail = request.Email,
                lastName = request.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _repo.AddAsync(user);
            return new RegisterResponse
            {
                Message = "Registration was Sucessfull",
                StatusCode = StatusCodes.Status200OK
            };
        }


    }
}
