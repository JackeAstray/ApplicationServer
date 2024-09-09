using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager
{
    public class TokenManager
    {
        private static TokenManager? instance;

        private string jwtSecret;

        public static TokenManager GetInstance()
        {
            if (instance == null)
            {
                Config config = ConfigManager.GetConfig();

                instance = new TokenManager(config.Secret);
            }

            return instance;
        }

        TokenManager(string secret)
        {
            jwtSecret = secret;
        }

        /// <summary>
        /// 生成 Token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GenerateToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("username", username) }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 验证 Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 刷新 Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public string? RefreshToken(string token)
        {
            // 验证 Token
            if (ValidateJwtToken(token))
            {
                string username = GetUsernameFromToken(token);
                Log.Debug($"刷新 Token, 用户名: {username}");
                // 验证刷新 Token 的有效性
                return GenerateToken(username);
            }

            return "Error";
        }

        /// <summary>
        /// 从令牌获取用户名
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GetUsernameFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var usernameClaim = jwtToken?.Claims.First(claim => claim.Type == "username");
            return usernameClaim?.Value;
        }
    }
}