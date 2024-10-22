using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class Authenticator : IAuthenticator
{
    private readonly IUserRepository userRepository;
    private readonly IConfiguration config;
    IWebHostEnvironment env;
    public Authenticator(IUserRepository userRepository, IConfiguration config, IWebHostEnvironment env)
    {
        this.userRepository = userRepository;
        this.config = config;
        this.env = env;
    }
    public async Task<string> Authenticate(string firstCredentials, string password)
    {
        var user =  Validator.IsValidEmail(firstCredentials) ? await userRepository.GetUserByEmail(firstCredentials) : await userRepository.GetUserByPhoneNumber(firstCredentials);
        Console.WriteLine(Validator.IsValidEmail(firstCredentials));
        if (user is null) 
            throw new Exception("Can't find user with the provided phoneNumber/email.");
        
        if (!Security.VerifyPassword(password, user.Password))
            throw new Exception("Incorrect password.");
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(env.IsDevelopment() ? config["Authentication:JwtKey"].ToString() : Environment.GetEnvironmentVariable("JwtKey").ToString());
        string type = Validator.IsValidEmail(firstCredentials) ? ClaimTypes.Email : ClaimTypes.MobilePhone;
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[] {
                new Claim(type, firstCredentials),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(72),
            SigningCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
       
    }

    public ClaimsPrincipal GetClaimsFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(env.IsDevelopment() ? config["Authentication:JwtKey"].ToString() : Environment.GetEnvironmentVariable("JwtKey").ToString());
        var validationParameters = new TokenValidationParameters 
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        try 
        {
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        
    }
}