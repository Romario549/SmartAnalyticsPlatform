using SAP.UserService.Domain.Enitities;
using SAP.UserService.Domain.Interfaces.Repositories;
using SAP.UserService.Domain.Interfaces.Services;
using SAP.UserService.Domain.ValueObjects;

namespace SAP.UserService.Application.Services;
public class OAuthService : IOAuthService
{
	private readonly IGitHubOAuthProvider _gitHubOAuthProvider;
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher _passwordHasher;

	public OAuthService(
		IGitHubOAuthProvider gitHubOAuthProvider, 
		IUserRepository userRepository, 
		IPasswordHasher passwordHasher)
	{
		_gitHubOAuthProvider = gitHubOAuthProvider;
		_userRepository = userRepository;
		_passwordHasher = passwordHasher;
	}

	public async Task<string> GetOAuthLoginUrlAsync(string provider)
	{
		if (provider != "GitHub")
			throw new ArgumentException("Only GitHub provider is supported");
		return await _gitHubOAuthProvider.GetAuthorizationUrlAsync();
	}

	public async Task<User> HandleOAuthCallbackAsync(string provider, string code)
	{
		if (provider != "GitHub")
			throw new ArgumentException("Only GitHub provider is supported");

		// 1. Получаем информацию о пользователе от GitHub
		var userInfo = await _gitHubOAuthProvider.ExchangeCodeForUserInfoAsync(code);

		// 2. Ищем пользователя по email (GitHub дает email)
		var user = await FindOrCreateUserAsync(userInfo);

		return user;
	}
	private async Task<User> FindOrCreateUserAsync(GitHubUserInfo userInfo)
	{
		var existingUser = await _userRepository.GetByEmailAsync(userInfo.Email) ?? throw new ApplicationException(
			"User not found. Please register first with email and password, then link GitHub account.");
		var oauthAdd = OAuthProvider.Create(userInfo.Login, userInfo.ProviderUserId, existingUser);
		await _userRepository.AddOauthAsync(oauthAdd);
		return existingUser;
	}

}
