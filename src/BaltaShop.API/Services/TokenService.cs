using BaltaShop.API.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace BaltaShop.API.Services
{
    public static class TokenService
    {
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            List<Claim> listClaims = new List<Claim>();

            string[] claims = user.Role.Split(',');

            foreach (var role in claims)
                listClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));

            listClaims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
            listClaims.Add(new Claim(ClaimTypes.Email, "caiopontalti@gmail.com"));

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(listClaims);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(Convert.ToUInt32(2)), //_appSettings.ExpiracaoHoras
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            return encodedToken;
        }
    }
}
