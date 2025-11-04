using SAP.UserService.Domain.ValueObjects;

namespace SAP.UserService.Domain.Interfaces.Services;

public interface IGitHubOAuthProvider
{
	Task<string> GetAuthorizationUrlAsync();
	Task<GitHubUserInfo> ExchangeCodeForUserInfoAsync(string code);
}