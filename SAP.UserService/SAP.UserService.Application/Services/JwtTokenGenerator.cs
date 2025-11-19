using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SAP.UserService.Application.Configurations;
using SAP.UserService.Domain.Enitities;
using SAP.UserService.Domain.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SAP.UserService.Application.Services;
public class JwtTokenGenerator : IJWTTokenGenerator
{
	private readonly JwtSettings _jwtSettings;
	private readonly SigningCredentials _signingCredentials;

	public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
	{
		_jwtSettings = jwtSettings.Value;

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
		_signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
	}

	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

	public string GenerateToken(User user)
	{

		var claims = new[]
		{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
				new Claim(JwtRegisteredClaimNames.GivenName, user.FullName.FirstName),
				new Claim(JwtRegisteredClaimNames.FamilyName, user.FullName.LastName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("is_active", user.IsActive.ToString())
		};

		var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtSettings.ExpiryMinutes)),
				signingCredentials: _signingCredentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);

	}
}
