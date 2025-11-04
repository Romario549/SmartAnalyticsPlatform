using Microsoft.EntityFrameworkCore;
using SAP.UserService.Infrastructure.Configurations;

namespace SAP.UserService.Infrastructure;
public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new OAuthProviderConfiguration());
	}
}
