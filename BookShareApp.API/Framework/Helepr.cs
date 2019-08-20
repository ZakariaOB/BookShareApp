using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using AutoMapper;

namespace BookShareApp.API.Framework
{
    public static class Helepr
    {
        public static string GenerateJwtToken(string identifier, string userName, string appSettingsConfig)
        {
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

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenResult = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokenResult);
        }


        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddJwtAuthenticationValidation(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
            {
                return;
            }

            var appSettingsConfig = config.GetSection(Constants.AppSettingsToken).Value;
            var bytesKey = Encoding.ASCII.GetBytes(appSettingsConfig);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(bytesKey),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });
        }

        public static int ToAge(this DateTime dateOfBirth)
        {
            var age = DateTime.UtcNow.Year - dateOfBirth.Year;
            var dateOfBirthWithAge = dateOfBirth.AddYears(age);
            if (dateOfBirthWithAge > DateTime.UtcNow)
            {
                age++;
            }

            return age;
        }
    }
}