namespace SAP.UserService.Domain.ValueObjects;

public record GitHubUserInfo(
	string Email,
	string FirstName,
	string LastName,
	string ProviderUserId
);