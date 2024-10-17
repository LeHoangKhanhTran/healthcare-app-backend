public record GetOtpDto(string PhoneNumber);
public record CheckOtpDto(string PhoneNumber, string Code);