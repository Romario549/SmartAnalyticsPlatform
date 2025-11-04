using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAP.UserService.Domain.Enitities;

namespace SAP.UserService.Infrastructure.Configurations;
public class OAuthProviderConfiguration : IEntityTypeConfiguration<OAuthProvider>
{
	public void Configure(EntityTypeBuilder<OAuthProvider> builder)
	{
		builder.ToTable("OAuthProviders");

		builder.HasKey(o => o.Id);

		builder.Property(o => o.ProviderName)
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(o => o.ProviderUserId)
			.HasMaxLength(255)
			.IsRequired();

		builder.HasOne(o => o.User)
		   .WithMany(u => u.OAuthProviders)
		   .HasForeignKey("UserId") // Shadow property - EF создаст автоматически
		   .IsRequired()
		   .OnDelete(DeleteBehavior.Cascade);

		builder.Property(o => o.LinkedAt)
			.IsRequired();

		builder.HasIndex(o => new { o.ProviderName, o.ProviderUserId })
			 .IsUnique();
	}
}
