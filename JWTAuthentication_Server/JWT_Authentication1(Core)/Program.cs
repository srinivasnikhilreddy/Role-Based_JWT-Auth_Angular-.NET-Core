using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using JWT_Authentication1_Core_.Contexts;

var builder = WebApplication.CreateBuilder(args);

//Register MyDbContext with the connection string from appsettings.json
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlconstr")));
/*builder.Services.AddDbContext<MyDbContext>(Options =>
    Options.UseSqlServer("data source=DESKTOP-90BV8QB\\SQLEXPRESS;database=tokenJWTDb;integrated security=true;Encrypt=True;TrustServerCertificate=True"));*/

// Add services to the container.
builder.Services.AddControllers();


//Do following 4-Steps for JWT Execution.
//1.Add Authentication:
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"], //value is fetched from appsettings.json file (Jwt:Issuer).
            ValidAudience = builder.Configuration["Jwt:Audience"], //value is fetched from appsettings.json file (Jwt:Audience).
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])), //This is derived from the Jwt:Key value in the configuration and encoded as a Symmetric Security Key.
            ValidateIssuer = true, //Ensures the token's issuer matches the configured ValidIssuer.
            ValidateAudience = true, //Ensures the token's audience matches the configured ValidAudience.
            ValidateIssuerSigningKey = true, //Validates that the signing key in the token matches the configured key.
            ValidateLifetime = true, // Reject expired tokens
            ClockSkew = TimeSpan.Zero // Remove default 5-minute expiration tolerance

        };
    });
//2.Add Authorization:
builder.Services.AddAuthorization(); //Adds authorization services, enabling the app to enforce policies or roles for accessing specific resources.
//3.Add configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
/*
The above code defines the Authentication Schema for JWT which we will use as an authentication method 
for our API. The code will use the JWT configuration we defined in “appsetting.json”
 */



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//If API and front-end language runs on different server then use CORS
builder.Services.AddCors((options) =>
{
    options.AddPolicy("myCors", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();//if required
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//add above used CORS name
app.UseCors("myCors");

app.UseHttpsRedirection();

app.UseAuthentication(); // 🔹 Ensure Authentication Middleware is used
app.UseAuthorization();

//4.additional setup:
IConfiguration configuration = app.Configuration; //Retrieves the application's configuration settings through the app.Configuration property.
IWebHostEnvironment environment = app.Environment; //Retrieves the hosting environment details through the app.Environment property.

app.MapControllers();

app.Run();
