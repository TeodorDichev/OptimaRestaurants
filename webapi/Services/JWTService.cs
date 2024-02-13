using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapi.Models;

namespace webapi.Services
{
    /// <summary>
    /// The service creates and validates JWT tokens
    /// </summary>

    public class JWTService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _jwtKey;
        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
#pragma warning disable CS8604 // Possible null reference argument.
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
#pragma warning restore CS8604 // Possible null reference argument.
        }
        public string CreateJWT(ApplicationUser user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            };

            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256);

#pragma warning disable CS8604 // Possible null reference argument.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.Now.AddDays(int.Parse(_configuration["JWT:ExpiresInDays"])),
                SigningCredentials = credentials,
                Issuer = _configuration["JWT:Issuer"]
            };
#pragma warning restore CS8604 // Possible null reference argument.

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(jwt);
        }

        public string GenerateQrToken(string email)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };

            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.MaxValue,
                SigningCredentials = credentials,
                Issuer = _configuration["JWT:Issuer"],

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenHandler.WriteToken(jwt)));
            return token;
        }

        public bool ValidateQrToken(string token, string email)
        {
            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var tokenHandler = new JwtSecurityTokenHandler();
#pragma warning disable CS8604 // Possible null reference argument.
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
#pragma warning restore CS8604 // Possible null reference argument.

                tokenHandler.ValidateToken(decodedToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    //Audience is empty
                    ValidateAudience = false,
                    //ValidAudience = _configuration["Jwt:Issuer"],
                    //ValidateLifetime = true,
                    //ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken?.Claims?.Any(c => c.Value.Equals(email)) ?? false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
