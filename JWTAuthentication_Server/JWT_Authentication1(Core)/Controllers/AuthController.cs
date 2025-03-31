using JWT_Authentication1_Core_.Contexts;
using JWT_Authentication1_Core_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// “AuthController.cs” which we will use to authorize the user and return the JWT Token.

namespace JWT_Authentication1_Core_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration Configuration;
        MyDbContext dbContext;
        public AuthController(IConfiguration configuration, MyDbContext myDb)
        {
            this.Configuration = configuration;
            dbContext = myDb;
        }
        // Authenticate the user from the database
        [NonAction]
        public User authenticateUser(User user)
        {
            User _user = null;
            if (user != null)
            {
                _user = dbContext.Users.Where(u =>
                            u.userName == user.userName &&
                            u.userPassword == user.userPassword).FirstOrDefault();
                dbContext.SaveChanges();
            }
            return _user;
        }

        // Generate token based on user's role
        [NonAction]
        public (string jwtToken, Refresh refreshToken) generateTokens(User user)
        {
            var issuer = Configuration["Jwt:Issuer"];
            var audience = Configuration["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            );
            //The code above retrieves the Issuer, Audience, and Key which we defined in “appsettings.json”. We will “signingCredentials” variable to store SigningCredentials with SymmetricSecurityKey created from our key and use the HmacSha512 security algorithm.
                    
            var roles = user.userRoles.Split(','); // Split roles if user has multiple roles
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Sub, user.userName),
                new Claim(JwtRegisteredClaimNames.Email, user.userEmail)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Trim())));// Add each role as a separate claim
            var subject = new ClaimsIdentity(claims);
            //The variable “subject” is used to store Claims. Here, we will generate the Claim of Sub and Email from the username.

            var expires = DateTime.UtcNow.AddMinutes(10);
            //Create a variable to contain the expiration date for the token.

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = subject,
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            //Finally, create a “tokenDescriptor” with the previously defined variable. Use JwtSecurityTokenHandler to create a token from the “tokenDescriptor”. Then use the “WriteToken” method to return the string value of the JWT.

            // Check if a valid refresh token already exists
            var existingRefreshToken = dbContext.Refresh
                .FirstOrDefault(rt => rt.UserId == user.userId && rt.RefreshTokenExpiryTime > DateTime.UtcNow);

            if (existingRefreshToken != null)
            {
                return (jwtToken, existingRefreshToken); // Reuse existing refresh token
            }

            //If no valid refresh token exists, Generate Refresh Token (random string) and Store the refresh token in the database and set expiration time
            var refreshToken = new Refresh
            {
                RefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7), // 7 days validity
                UserId = user.userId
            };
            // Save refresh token in DB
            dbContext.Refresh.Add(refreshToken);
            dbContext.SaveChanges();

            return (jwtToken, refreshToken);//access token and refresh token
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Auth([FromBody] User user)
        {
            IActionResult response = Unauthorized();
            var authenticatedUser = authenticateUser(user);
            if(authenticatedUser != null)
            {
                // Extract existing JWT from request headers
                var existingJwt = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                /*For the first req Request.Headers will not contain Authorization,
                 * on next req it ll have jwt. if ur using postman keep first requested jwt in Headers section Value place 
                 * by appending Bearer to it, and at Key place add Authorization.
                 * //Keep break points down and check. 
                */
                Console.WriteLine(existingJwt);
                foreach (var header in Request.Headers)
                {
                    Console.WriteLine($"{header.Key}: {header.Value}");
                }

                // Validate the existing JWT
                if (!string.IsNullOrEmpty(existingJwt)){
                    try{
                        var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var validationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = Configuration["Jwt:Audience"],
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero // Prevents expiration tolerance
                        };

                        tokenHandler.ValidateToken(existingJwt, validationParameters, out SecurityToken validatedToken);

                        // If valid, return the existing JWT and refresh token
                        var existingRefreshToken = dbContext.Refresh.FirstOrDefault(rt => rt.UserId == authenticatedUser.userId);
                        if (existingRefreshToken != null && existingRefreshToken.RefreshTokenExpiryTime > DateTime.UtcNow)
                        {
                            return Ok(new
                            {
                                token = existingJwt,
                                refreshToken = existingRefreshToken.RefreshToken,
                                refreshTokenExpiryTime = existingRefreshToken.RefreshTokenExpiryTime
                            });
                        }
                    }catch(SecurityTokenException){
                        // Token is expired or invalid, generate a new one
                    }
                }
                var (jwtToken, refreshToken) = generateTokens(authenticatedUser);
                response = Ok(new { token = jwtToken, refreshToken = refreshToken.RefreshToken, refreshTokenExpiryTime = refreshToken.RefreshTokenExpiryTime });
            }
            return response;
        }

        //Refresh the JWT token using the refresh token
        [AllowAnonymous]
        [HttpPost("RefreshJWTToken")]
        public IActionResult RefreshJWTToken([FromBody] Refresh refreshTokenRequest)
        {
            IActionResult response = Unauthorized();
            var refreshToken = dbContext.Refresh.Include(rt => rt.User)
                .FirstOrDefault(rt => rt.RefreshToken == refreshTokenRequest.RefreshToken);

            if(refreshToken != null && refreshToken.RefreshTokenExpiryTime > DateTime.UtcNow)
            {
                var (newJwtToken, newRefreshToken) = generateTokens(refreshToken.User);
                // Remove old refresh token
                dbContext.Refresh.Remove(refreshToken);
                dbContext.SaveChanges();
                response = Ok(new { token = newJwtToken, refreshToken = newRefreshToken.RefreshToken, refreshTokenExpiryTime = newRefreshToken.RefreshTokenExpiryTime });
            }else{
                response = Unauthorized("Invalid or expired refresh token.");
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("Logout")]
        public IActionResult Logout([FromBody] Refresh refreshTokenRequest)
        {
            if (refreshTokenRequest == null || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken))
            {
                return BadRequest("Invalid logout request.");
            }

            // Find the refresh token in the database
            var refreshToken = dbContext.Refresh
                .FirstOrDefault(rt => rt.RefreshToken == refreshTokenRequest.RefreshToken);

            if (refreshToken != null)
            {
                dbContext.Refresh.Remove(refreshToken); // Remove refresh token
                dbContext.SaveChanges();
                return Ok(new { message = "Logged out successfully." });
            }

            return Unauthorized("Invalid refresh token.");
        }
    }
}

/*
{
  "userId": 1,
  "userName": "srinivas",
  "userPassword": "12345",
  "userRoles": "Admin",
  "userEmail": "srinivas@gmail.com"
}
*/
/*
***For single User Role:
1.To make our endpoint requires authorization. Add the [Authorize] tag above the 
 Get Method in WeatherForecast Controller

2.Now hit the WeatherForecast endpoint again in (Postman).
 You will get an Error 401 Unauthorized message.

3.use the auth endpoint we created earlier to get the token(from Browser or Postman) and 
 use it to hit the WeatherForecast endpoint.
 
4.Copy the resulting token and use it in the Authorization Header with the Bearer token schema.

5.Authorization: Bearer <token> (or esle don't keep Bearer in postman)

6.If you are using postman, click the “Authorization” Tab and choose “Bearer Token”. 
 Paste the token inside the Token input field.

7.After setting up the Authorization Header, Hit the Weather Forecast endpoint again.
 
8.You'll get the WhetherForecast

*/
