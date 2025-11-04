using SAP.UserService.Domain.Enitities;
using System.Security.Claims;

namespace SAP.UserService.Domain.Interfaces.Services;

public interface IJWTTokenGenerator
{
	string GenerateToken(User user);
	string GenerateRefreshToken();
}
