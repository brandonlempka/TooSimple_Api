using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Net;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_Poco.Models.Shared;
using System.Text;
using System.ComponentModel.DataAnnotations;
using TooSimple_Poco.Models.Dtos.Auth;
using TooSimple_Poco.Models.DataModels;

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

        /// <summary>
        /// Creates a new user account and returns json web token for auth.
        /// </summary>
        /// <param name="userDto">
        /// Username and password for new user.
        /// </param>
        /// <returns>
        /// <see cref="JwtDto"/> containg bearer token.
        /// </returns>
        public async Task<JwtDto> RegisterUserAsync(UserRequestDto userDto)
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

            string normalizedEmail = userDto.UserName.ToUpper();

            CreatePasswordHash(
                userDto.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            if (passwordHash is null || passwordSalt is null)
            {
                jwtDto.Success = false;
                jwtDto.Status = HttpStatusCode.InternalServerError;
                jwtDto.ErrorMessage = "Something went wrong while registering user.";
                return jwtDto;
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
                jwtDto.Success = false;
                jwtDto.Status = HttpStatusCode.InternalServerError;
                jwtDto.ErrorMessage = dbResponse.ErrorMessage;
                return jwtDto;
            }

            string token = CreateToken(newUser);
            jwtDto.BearerToken = token;
            jwtDto.Success = true;
            jwtDto.Status = HttpStatusCode.Created;
            return jwtDto;
        }

        /// <summary>
        /// Verifies username & password are correct and returns jwt for auth.
        /// </summary>
        /// <param name="userDto">
        /// Username and password for new user.
        /// </param>
        /// <returns>
        /// <see cref="JwtDto"/> containg bearer token.
        /// </returns>
        public async Task<JwtDto> LoginUserAsync(UserRequestDto userDto)
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

            UserAccountDataModel? user = await _userAccountAccessor
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

        /// <summary>
        /// Validates the user is sending good data.
        /// </summary>
        /// <param name="userDto">
        /// Username & password.
        /// </param>
        /// <returns>
        /// BaseHttpResponse of errors.
        /// </returns>
        private static BaseHttpResponse ValidateUserDto(UserRequestDto userDto)
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
            else
            {
                userDto.UserName = userDto.UserName.Trim();
                if(!new EmailAddressAttribute()
                    .IsValid(userDto.UserName))
                {
                    errors.Add($"{userDto.UserName} is not a valid email address.");
                }
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

        /// <summary>
        /// Creates password hash & salt byte[] for password storage.
        /// </summary>
        /// <param name="password">
        /// User provided password to salt & hash.
        /// </param>
        /// <param name="passwordHash">
        /// Password hash byte array.
        /// Returned via out for immediate access without assignment.
        /// </param>
        /// <param name="passwordSalt">
        /// Password salt byte array.
        /// Returned via out for immediate access without assignment.
        /// </param>
        private static void CreatePasswordHash(
            string password,
            out byte[] passwordHash,
            out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        /// <summary>
        /// Rehashes password with salt to see if password matches user submission.
        /// </summary>
        /// <param name="password">
        /// User provided password to verify matches password hash.
        /// </param>
        /// <param name="passwordHash">
        /// Password hash byte array from database.
        /// </param>
        /// <param name="passwordSalt">
        /// Password salt byte array from database.
        /// </param>
        /// <returns>
        /// Boolean indicating password is correct or incorrect.
        /// </returns>
        private static bool VerifyPasswordHash(
            string password,
            byte[] passwordHash,
            byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(passwordHash);
        }

        /// <summary>
        /// Creates Json web token with user account id & username claims.
        /// </summary>
        /// <param name="user">
        /// User to create token for.
        /// </param>
        /// <returns>
        /// String of Json web token.
        /// </returns>
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

            SymmetricSecurityKey key = new(
                Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials credentials = new(
                key,
                SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials
                );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
