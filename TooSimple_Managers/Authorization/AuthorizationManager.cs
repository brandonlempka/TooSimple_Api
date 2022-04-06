using System.Security.Claims;
using System.Security.Cryptography;
using TooSimple_Poco.Models.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Net;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Authorization
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private readonly IConfiguration _configuration;
        private readonly IUserAccountAccessor _userAccountAccessor;

        public AuthorizationManager(
            IConfiguration configuration,
            IUserAccountAccessor userAccountAccessor)
        {
            _configuration = configuration;
            _userAccountAccessor = userAccountAccessor;
        }

        public async Task<BaseHttpResponse> RegisterUserAsync(UserDto userDto)
        {
            BaseHttpResponse response = ValidateUserDto(userDto);
            if (!response.Success)
            {
                return response;
            }

            string normalizedEmail = userDto.UserName.ToUpper();

            CreatePasswordHash(
                userDto.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            if (passwordHash is null || passwordSalt is null)
            {
                response.Success = false;
                response.Status = HttpStatusCode.InternalServerError;
                response.ErrorMessage = "Something went wrong while registering user.";
                return response;
            }

            UserAccountDataModel newUser = new()
            {
                UserAccountId = Guid.NewGuid().ToString(),
                UserName = userDto.UserName,
                NormalizedUserName = normalizedEmail,
                Email = userDto.UserName,
                NormalizedEmail = normalizedEmail,
                IsEmailConfirmed = false,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = Convert.ToBase64String(passwordSalt),
                IsTwoFactorEnabled = false,
                FailedLoginCount = 0
            };

            DatabaseResponseModel dbResponse = await _userAccountAccessor.RegisterUserAsync(newUser);
            
            if (!dbResponse.Success)
            {
                response.Success = false;
                response.Status = HttpStatusCode.InternalServerError;
                response.ErrorMessage = dbResponse.ErrorMessage;
                return response;
            }

            response.Success = true;
            response.Status = HttpStatusCode.Created;
            return response;
        }

        public async Task<JwtDto> LoginUserAsync(UserDto userDto)
        {
            JwtDto jwtDto = new();

            BaseHttpResponse validationResult = ValidateUserDto(userDto);
            if (!validationResult.Success)
            {
                jwtDto.Success = false;
                jwtDto.ErrorMessage = validationResult.ErrorMessage;
                jwtDto.Status = validationResult.Status;
                return jwtDto;
            }

            UserAccountDataModel user = await _userAccountAccessor
                .GetUserAccountAsync(userDto.UserName.ToUpper());

            if (user is null)
            {
                jwtDto.ErrorMessage = "No user was found with this email address.";
                jwtDto.Status = HttpStatusCode.NotFound;
                return jwtDto;
            }
            
            bool isVerified = VerifyPasswordHash(
                userDto.Password,
                Convert.FromBase64String(user.PasswordHash),
                Convert.FromBase64String(user.PasswordSalt));

            if (!isVerified)
            {
                jwtDto.Status = HttpStatusCode.BadRequest;
                jwtDto.ErrorMessage = "Invalid password.";
                return jwtDto;
            }

            string token = CreateToken(user);
            jwtDto.BearerToken = token;
            jwtDto.Status = HttpStatusCode.OK;
            jwtDto.Success = true;
            return jwtDto;
        }

        private static BaseHttpResponse ValidateUserDto(UserDto userDto)
        {
            List<string> errors = new();
            BaseHttpResponse response = new()
            {
                Status = HttpStatusCode.BadRequest,
            };

            if (userDto is null)
            {
                response.ErrorMessage = "Request arrived null.";
                return response;
            }

            if (string.IsNullOrWhiteSpace(userDto.UserName))
            {
                errors.Add("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(userDto.Password))
            {
                errors.Add("Password is required.");
            }

            if (errors.Count == 0)
            {
                response.Success = true;
                return response;
            }

            response.ErrorMessage = string.Join(", ", errors);

            return response;
        }

        private static void CreatePasswordHash(
            string password,
            out byte[] passwordHash,
            out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(
            string password,
            byte[] passwordHash,
            byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(passwordHash);
        }

        private string CreateToken(UserAccountDataModel user)
        {
            List<Claim> claims = new()
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserAccountId),
                new Claim(
                    ClaimTypes.Name,
                    user.UserName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
