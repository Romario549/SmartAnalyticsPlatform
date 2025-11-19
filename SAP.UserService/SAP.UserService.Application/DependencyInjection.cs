using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAP.UserService.Application.Interfaces;
using SAP.UserService.Application.Services;
using SAP.UserService.Domain.Interfaces.Services;

namespace SAP.UserService.Application;
public static class DependencyInjection
{
	public static IServiceCollection AddApplication(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IOAuthService, OAuthService>();
		services.AddScoped<IJWTTokenGenerator, JwtTokenGenerator>();


		return services;
	}
}
