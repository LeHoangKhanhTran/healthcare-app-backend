using Twilio;
using Twilio.Rest.Verify.V2.Service;

public class OtpService : IOtpService
{
    private readonly IConfiguration config;
    IWebHostEnvironment env;

    public OtpService(IConfiguration config, IWebHostEnvironment env) 
    {
        this.config = config;
        this.env = env;
    }
    public async Task<string> SendOtp(string phoneNumber)
    {
        string accountSid = env.IsDevelopment() ? config["Twilio:AccountSid"] : Environment.GetEnvironmentVariable("AccountSid");
        string authToken = env.IsDevelopment() ? config["Twilio:AuthToken"] : Environment.GetEnvironmentVariable("AuthToken");

        TwilioClient.Init(accountSid, authToken);

        var verification = await VerificationResource.CreateAsync(
            to: "+84" + phoneNumber[1..],
            channel: "sms",
            pathServiceSid: "VAc4e0dbab904e0f2fe4afaaf452e3b81c");
        return verification.Status;
    }

    public async Task<string> CheckOtp(string phoneNumber, string code)
    {
        string accountSid = env.IsDevelopment() ? config["Twilio:AccountSid"] : Environment.GetEnvironmentVariable("AccountSid");
        string authToken = env.IsDevelopment() ? config["Twilio:AuthToken"] : Environment.GetEnvironmentVariable("AuthToken");

        TwilioClient.Init(accountSid, authToken);

        var verificationCheck = await VerificationCheckResource.CreateAsync(
            to: "+84" + phoneNumber[1..],
            code: code,
            pathServiceSid: "VAc4e0dbab904e0f2fe4afaaf452e3b81c");

        return verificationCheck.Status;
    }
}