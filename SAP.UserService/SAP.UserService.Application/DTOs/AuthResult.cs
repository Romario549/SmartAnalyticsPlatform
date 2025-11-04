namespace SAP.UserService.Application.DTOs;

public record AuthResult(
	Guid UserId,
	string Email,
	string FirstName,
	string LastName,
	string Token,
	string RefreshToken
	);