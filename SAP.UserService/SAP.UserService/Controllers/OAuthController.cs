using Microsoft.AspNetCore.Mvc;
using SAP.UserService.Application.Services;
using SAP.UserService.Domain.Interfaces.Services;

namespace SAP.UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OAuthController:ControllerBase
{
	private readonly IOAuthService _oauthService;
	private readonly ILogger<OAuthController> _logger;
	private readonly IJWTTokenGenerator _jWTTokenGenerator;

	public OAuthController(IOAuthService oauthService, ILogger<OAuthController> logger, IJWTTokenGenerator jWTTokenGenerator)
	{
		_oauthService = oauthService;
		_logger = logger;
		_jWTTokenGenerator = jWTTokenGenerator;
	}

	[HttpGet("github")]
	public async Task<IActionResult> StartGitHubOAuth()
	{
		try
		{
			var redirectUri = $"{Request.Scheme}://{Request.Host}/api/oauth/github/callback";
			var authorizationUrl = await _oauthService.GetOAuthLoginUrlAsync("GitHub");

			return Ok(new
			{
				authorization_url = authorizationUrl,
				redirect_uri = redirectUri
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error starting GitHub OAuth");
			return BadRequest(new { error = ex.Message });
		}
	}

	[HttpGet("github/callback")]
	public async Task<IActionResult> GitHubCallback()
	{
		try
		{
			var code = Request.Query["code"].ToString();
			var state = Request.Query["state"].ToString();

			if (string.IsNullOrEmpty(code))
				return BadRequest(new { error = "Code parameter is required" });

			var user = await _oauthService.HandleOAuthCallbackAsync("GitHub", code);

			var token = _jWTTokenGenerator.GenerateToken(user);
			var refreshToken = _jWTTokenGenerator.GenerateRefreshToken();

			return Ok(new
			{
				user_id = user.Id,
				email = user.Email.Value,
				first_name = user.FullName.FirstName,
				last_name = user.FullName.LastName,
				access_token = token,
				refresh_token = refreshToken,
				message = "GitHub OAuth successful"
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in GitHub OAuth callback");
			return BadRequest(new { error = ex.Message });
		}
	}
}
