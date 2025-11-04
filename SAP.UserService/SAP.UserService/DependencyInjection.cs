using SAP.UserService.Application.Configurations;
using SAP.UserService.Domain.Interfaces.Services;
using SAP.UserService.Infrastructure.Services;

namespace SAP.UserService.Api;

public static class DependencyInjection
{
	public static IServiceCollection AddServices(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddHttpClient<IGitHubOAuthProvider, GitHubOAuthProvider>();

		services.AddControllers();

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new() { Title = "User Service API", Version = "v1" });

			// Добавляем описание OAuth endpoints
			c.TagActionsBy(api => new[] { api.GroupName ?? "Default" });
		});

		return services;
	}
}
