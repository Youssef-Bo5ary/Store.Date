using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Store.Date.Entities.IdentityEntity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.TokenServices
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _Key;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"]));
        }
        public string GenerateToken(AppUser appUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,appUser.Email),
                new Claim(ClaimTypes.GivenName,appUser.DisplayName),
                new Claim("UserId",appUser.Id),
                new Claim("UserName",appUser.UserName)
            };

            var creds = new SigningCredentials(_Key,SecurityAlgorithms.HmacSha256);

            var tokenDescrptor = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                Issuer = _configuration["Token:Issuer"],
                IssuedAt=DateTime.Now,
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials=creds
            };

            var tokenHandeler = new JwtSecurityTokenHandler();

            var token= tokenHandeler.CreateToken(tokenDescrptor);

            return tokenHandeler.WriteToken(token);
        }
    }
}
