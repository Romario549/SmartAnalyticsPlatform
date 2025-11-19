namespace SAP.UserService.Domain.ValueObjects;

public record GitHubUserInfo(
	string Email,
	string Login,
	string ProviderUserId
);