using Microsoft.Extensions.Options;
using SAP.UserService.Application.Configurations;
using SAP.UserService.Domain.Enitities;
using SAP.UserService.Domain.Interfaces.Repositories;
using SAP.UserService.Domain.Interfaces.Services;
using SAP.UserService.Domain.ValueObjects;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace SAP.UserService.Infrastructure.Services;
public class GitHubOAuthProvider : IGitHubOAuthProvider
{
	private readonly HttpClient _httpClient;
	private readonly GitHubOAuthSettings _settings;

	public GitHubOAuthProvider(HttpClient httpClient, IOptions<GitHubOAuthSettings> settings)
	{
		_httpClient = httpClient;
		_settings = settings.Value;
		_httpClient.DefaultRequestHeaders.Add("User-Agent", "SAP.UserService");

	}

	public async Task<GitHubUserInfo> ExchangeCodeForUserInfoAsync(string code)
	{
		var accessToken = await ExchangeCodeForTokenAsync(code);
		return await GetUserInfoAsync(accessToken);
	}

	public Task<string> GetAuthorizationUrlAsync()
	{
		var parameters = new Dictionary<string, string>() 
		{
			["client_id"] = _settings.ClientId,
			["redirect_uri"] = _settings.RedirectUri,
			["scope"] = "user:email",
			["state"] = Guid.NewGuid().ToString()
		};
		var url = "https://github.com/login/oauth/authorize?" +
		string.Join("&", parameters.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));
		return Task.FromResult(url);
	}



	private async Task<string> ExchangeCodeForTokenAsync(string code)
	{
		var request = new
		{
			client_id = _settings.ClientId,
			client_secret = _settings.ClientSecret,
			code = code,
			redirect_uri = _settings.RedirectUri
		};

		var response = await _httpClient.PostAsJsonAsync(
			"https://github.com/login/oauth/access_token",
			request);

		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();
		var formData = HttpUtility.ParseQueryString(content);

		return formData["access_token"] ??
			   throw new InvalidOperationException("Access token not found in GitHub response");
	}

	private async Task<GitHubUserInfo> GetUserInfoAsync(string accessToken)
	{
		_httpClient.DefaultRequestHeaders.Authorization =
			new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

		// Get user profile
		var userResponse = await _httpClient.GetAsync("https://api.github.com/user");
		userResponse.EnsureSuccessStatusCode();

		var userJson = await userResponse.Content.ReadAsStringAsync();
		using var userDoc = JsonDocument.Parse(userJson);
		var userRoot = userDoc.RootElement;

		var userId = userRoot.GetProperty("id").GetInt64().ToString();
		var login = userRoot.GetProperty("login").GetString() ?? "";
		var name = userRoot.GetProperty("name").GetString() ?? login;

		// Get email
		var emailResponse = await _httpClient.GetAsync("https://api.github.com/user/emails");
		emailResponse.EnsureSuccessStatusCode();

		var emailsJson = await emailResponse.Content.ReadAsStringAsync();
		var emails = JsonSerializer.Deserialize<List<GitHubEmail>>(emailsJson)
					?? new List<GitHubEmail>();

		var primaryEmail = emails.FirstOrDefault(e => e.primary && e.verified)?.email
						?? emails.FirstOrDefault(e => e.verified)?.email
						?? $"{login}@users.noreply.github.com";

		// Parse name
		var nameParts = name.Split(' ');
		var firstName = nameParts.Length > 0 ? nameParts[0] : login;
		var lastName = nameParts.Length > 1 ? nameParts[^1] : "";

		return new GitHubUserInfo(primaryEmail, firstName, lastName, userId);
	}
}

public record GitHubEmail(string email, bool primary, bool verified, string visibility);
