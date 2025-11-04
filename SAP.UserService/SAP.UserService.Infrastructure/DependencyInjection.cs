using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAP.UserService.Application.Configurations;
using SAP.UserService.Domain.Interfaces.Repositories;
using SAP.UserService.Domain.Interfaces.Services;
using SAP.UserService.Infrastructure.Services;

namespace SAP.UserService.Infrastructure;
public static class DependencyInjection
{

	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services, 
		IConfiguration configuration)
	{
		services.AddDbContext<UserDbContext>(options =>
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

		services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.Configure<GitHubOAuthSettings>(configuration.GetSection("GitHubOAuth"));


		return services;
	}
}
