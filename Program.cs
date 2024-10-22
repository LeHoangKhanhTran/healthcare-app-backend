using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
string connectionString = "";
string cloudinaryURL = "";
string jwtKey = "";
string[] allowedOrigins = new string[]{ env.IsDevelopment() ? "http://localhost:5173" : "https://healthcare-app-frontend-sd4b.vercel.app"};
if (env.IsDevelopment()) 
{
    connectionString = builder.Configuration["Database:ConnectionString"];
    cloudinaryURL = builder.Configuration["Cloudinary:CloudinaryURL"];
    jwtKey = builder.Configuration["Authentication:JwtKey"];
}
else 
{
    connectionString = Environment.GetEnvironmentVariable("ConnectionString");
    cloudinaryURL = Environment.GetEnvironmentVariable("CloudinaryURL");
    jwtKey = Environment.GetEnvironmentVariable("JwtKey");
}


IMongoClient mongoClient = new MongoClient(connectionString);
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider => 
{
    IMongoDatabase database = mongoClient.GetDatabase("Healthcare");
    return database;
});

builder.Services.AddSingleton<ICloudinaryUploader, CloudinaryUploader>(serviceProvider =>
{
    Cloudinary cloudinary = new(cloudinaryURL);
    cloudinary.Api.Secure = true;
    return new(cloudinary);
});

builder.Services.AddSingleton<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddSingleton<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddSingleton<IDoctorRepository, DoctorRepository>();
builder.Services.AddSingleton<IPatientProfileRepository, PatientProfileRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthenticator, Authenticator>();
builder.Services.AddScoped<IOtpService, OtpService>();
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow All", builder => 
    {
        builder.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/user/authenticate";
    options.Cookie.Name = "access_token";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();
// app.UseHttpsRedirection();
app.UseCors("Allow All");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
