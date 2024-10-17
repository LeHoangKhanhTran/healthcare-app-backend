using System.Security.Claims;

public interface IAuthenticator 
{
    public Task<string> Authenticate(string firstCredentials, string password);
    public ClaimsPrincipal GetClaimsFromToken(string token);
}