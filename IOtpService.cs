public interface IOtpService 
{
    public Task<string> SendOtp(string phoneNumber);
    public Task<string> CheckOtp(string phoneNumber, string code);
}