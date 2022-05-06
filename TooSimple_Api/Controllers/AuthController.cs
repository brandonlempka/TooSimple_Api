using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.Authorization;
using TooSimple_Poco.Models.Dtos.Auth;

namespace TooSimple_Api.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthorizationManager _authorizationManager;

        public AuthController(IAuthorizationManager authorizationManager)
        {
            _authorizationManager = authorizationManager;
        }

        [HttpPost("register")]
        public async Task<JwtDto> Register(UserRequestDto request)
        {
            JwtDto response = await _authorizationManager.RegisterUserAsync(request);
            return response;
        }

        [HttpPost("login")]
        public async Task<JwtDto> Login(UserRequestDto request)
        {
            JwtDto response = await _authorizationManager.LoginUserAsync(request);
            return response;
        }
    }
}
