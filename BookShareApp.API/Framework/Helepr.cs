using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BookShareApp.API.Framework
{
    public static class Helepr
    {
        public static string GenerateJwtToken(string identifier, string userName, string appSettingsConfig) {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, identifier),
                new Claim(ClaimTypes.Name, userName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettingsConfig));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler =  new JwtSecurityTokenHandler();
            var tokenResult = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokenResult);
        }
    }
}