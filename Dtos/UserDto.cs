using HealthAppAPI.Enums;

public record UserLoginDto(string? Email, string? PhoneNumber, string Password);
public record UserRegisterDto(string Email, string PhoneNumber, string Password);
public record UserDto(Guid UserId, string Email , string PhoneNumber, string Role, Guid? ProfileId);
public record UpdateUserDto(string Email, string PhoneNumber, string Password);