using ExtJwtTokenValidation.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtJwtTokenValidation.Util.Auth
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtValidationSettings _jwtSettings;

        public JwtValidationMiddleware(RequestDelegate next, IOptions<JwtValidationSettings> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userDto = new UserDto
                {
                    Id = long.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value),
                    Email = jwtToken.Claims.First(x => x.Type == "Email").Value,
                    Name = jwtToken.Claims.First(x => x.Type == "Name").Value,
                    Lastname = jwtToken.Claims.First(x => x.Type == "Lastname").Value,
                    Groups = jwtToken.Claims.First(x => x.Type == "Groups").Value.Split(",", StringSplitOptions.RemoveEmptyEntries),
                    EmployeeId = jwtToken.Claims.First(x => x.Type == "EmployeeId").Value,
                };
                context.Items["User"] = userDto;
            }
            catch (Exception ex)
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
                //Log error
            }
        }
    }
}