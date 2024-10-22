using System.Security.Claims;
using HealthAppAPI.Entities;
using HealthAppAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HealthAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserController> _logger;
    private readonly IAuthenticator _authenticator;
    private readonly IOtpService _otpService;
    public UserController(IUserRepository userRepositiry, ILogger<UserController> logger, IAuthenticator authenticator, IOtpService otpService)
    {
        _userRepository = userRepositiry;
        _logger = logger;
        _authenticator = authenticator;
        _otpService = otpService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] UserLoginDto userDto)
    {
        try {
            if (userDto.Email is null && userDto.PhoneNumber is null) throw new Exception("No phone number nor email was provided."); 
            string? credentials = userDto.Email is not null && userDto.Email.Length > 0 ? userDto.Email : userDto.PhoneNumber;
    
            var token = await _authenticator.Authenticate(credentials, userDto.Password);
            if (token is null)
                return Unauthorized();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };
            HttpContext.Response.Cookies.Append("access_token", token, cookieOptions);
            return Ok(new {token});
        }
        catch(Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register(UserRegisterDto userDto)
    {
        if (userDto.Email is null) throw new Exception("No email was provided.");
        if (userDto.PhoneNumber is null) throw new Exception("No phone number was provided.");
        if (!Validator.IsValidEmail(userDto.Email))
        {
            throw new Exception("Invalid email.");
        }
        if (userDto.Email != null) 
        {
            var existingUser = await _userRepository.GetUserByEmail(userDto.Email);
            if (existingUser is not null) throw new Exception("This email has been registered.");
        }
        if (userDto.PhoneNumber != null) 
        {
            var existingUser = await _userRepository.GetUserByPhoneNumber(userDto.PhoneNumber);
            if (existingUser is not null) throw new Exception("This phone number has been registered.");
        }
        Security.CheckValidPassword(userDto.Password); 
        Patient user = new Patient(new Guid(), userDto.Email, userDto.PhoneNumber, Security.HashPassword(userDto.Password));
        await _userRepository.CreateUser(user);
        _logger.LogInformation("New user registered.");
        return Ok();
        
    }

    [HttpGet]
    [Route("me", Name = "GetCurrentUser")]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        string token = HttpContext.Request.Cookies["access_token"];
        if (token is null)
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                var headerValue = authorizationHeader.FirstOrDefault();
                if (headerValue is not null && headerValue.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
                {
                    token = headerValue.Split(' ')[1];
                }
            }
            else 
            {
                return Unauthorized();
            }
        }
        var principal = _authenticator.GetClaimsFromToken(token);
        
        if (principal != null)
        {
            var phoneNumber = principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (phoneNumber is null && email is null) return BadRequest();
            var existingUser = email is null ? await _userRepository.GetUserByPhoneNumber(phoneNumber) : await _userRepository.GetUserByEmail(email);
            if (existingUser is null) return NotFound();
            return existingUser.AsDto();
        }
        return Unauthorized();
    }

    [HttpGet]
    [Route("logout")]
    public async Task<ActionResult> LogOut() {
        var cookieOptions = new CookieOptions 
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            // Domain = "localhost", 
            Expires = DateTime.UtcNow.AddDays(-1)
        };
        Response.Cookies.Append("access_token", "", cookieOptions);
        return Ok(new {message = "User logged out"});
    }

    [HttpGet("/api/verify-otp", Name = "GetOtpCode")]
    public async Task<ActionResult> GetOtpCode([FromQuery]GetOtpDto otpDto)
    {
        var verificationStatus = await _otpService.SendOtp(otpDto.PhoneNumber);
        if (verificationStatus == "pending") return Ok();
        return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Verification service is currently unavailable. Please try again later." });

    }

    [HttpPost("/api/verify-otp", Name = "GetOtpCode")]
    public async Task<ActionResult> VerifyOtpCode(CheckOtpDto otpDto)
    {
        var verificationStatus = await _otpService.CheckOtp(otpDto.PhoneNumber, otpDto.Code);
        if (verificationStatus == "approved") return Ok();
        return BadRequest(new { message = "Invalid or incorrect OTP code." });
    }
}
