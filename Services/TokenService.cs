using Blog.Extensions;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog.Services
{
	public class TokenService
	{
		public string GenerateToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
			var claims = user.GetClaim();
			var tokenDescription = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),

				Expires = DateTime.UtcNow.AddHours(12),

				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescription);

			return tokenHandler.WriteToken(token);
		}
	}
}


