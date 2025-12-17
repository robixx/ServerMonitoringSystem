using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SysMonitor.Application;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SysMonitor.Infrastructure
{
    public class JwtConfig
    {
        private readonly IConfiguration _config;
        
        public JwtConfig(IConfiguration config)
        {
            _config = config;
        }

        public string Generate(JwtUser auth)
        {
            var jti = Guid.NewGuid().ToString();
            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            try
            {
                double expirationTime = Convert.ToDouble(_config["Expiration:minutes"]);

                auth.TokenExpired = DateTime.UtcNow.AddMinutes(expirationTime);
                var claims = new[] {
                    new Claim(type: "JWTId", jti),
                    new Claim(type: "UserId", value: auth.UserId.ToString() ?? ""),
                    new Claim(type: "DispalyName",value: auth.DispalyName ?? ""),
                    new Claim(type: "EmployeeId",value: auth.EmployeeId.ToString() ??"0"),
                    new Claim(type: "RoleName",value: auth.RoleName?.ToString() ?? string.Empty),
                    new Claim(type: "RoleId",value: auth.RoleId.ToString()?? "0"),                    
                    new Claim(type: "TokenExpired", value: DateTime.Now.AddMinutes(expirationTime).ToString("yyyy-MM-dd HH:mm:ss")??"")

                };


                var token = new JwtSecurityToken(
                        _config["Jwt:Issuer"],
                        _config["Jwt:Audience"],
                        claims,
                        expires: DateTime.Now.AddMinutes(expirationTime),
                        signingCredentials: credentials);

                //_jwtStore.StoreJti(jti, DateTime.UtcNow.AddHours(expirationTime));

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {

                throw new Exception("Error generating token", ex);
            }

        }

        public static JwtUser? ReadToken(ClaimsIdentity httpContextIdentity)
        {

            try
            {
                var jtiClaim = httpContextIdentity.FindFirst("JWTId")?.Value;
                if (string.IsNullOrEmpty(jtiClaim))
                {

                    throw new SecurityTokenException("JWT ID (jti) not found in token.");
                }

                var identity = httpContextIdentity ?? throw new SecurityTokenException("JWT Token Invalid."); //
                var claims = identity.Claims;
                var auth = new JwtUser
                {
                    JWTId = jtiClaim,
                    UserId = Convert.ToInt32(claims.FirstOrDefault(c => c.Type == "UserId")?.Value),
                    EmployeeId = Convert.ToInt32(claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value),
                    RoleId = Convert.ToInt32(claims.FirstOrDefault(c => c.Type == "RoleId")?.Value),
                    DispalyName = claims.FirstOrDefault(c => c.Type == "DispalyName")?.Value,
                    RoleName = claims.FirstOrDefault(c => c.Type == "RoleName")?.Value,
                    TokenExpired = Convert.ToDateTime(claims.FirstOrDefault(c => c.Type == "TokenExpired")?.Value.ToString()),

                };

                return auth;
            }
            catch (Exception ex)
            {

                throw new Exception("Error reading token", ex);
            }
        }
    }
}
