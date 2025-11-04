using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAP.UserService.Domain.Enitities;

namespace SAP.UserService.Infrastructure.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{

		builder.ToTable("Users"); 

		builder.HasKey(x => x.Id);

		builder.OwnsOne(u => u.Email, emailBuilder =>
		{
			emailBuilder.Property(e => e.Value)
				.HasColumnName("Email") 
				.IsRequired()
				.HasMaxLength(255);
		});
		builder.OwnsOne(u => u.Password, passwordBuilder =>
		{
			passwordBuilder.Property(p => p.Hash)
				.HasColumnName("PasswordHash")
				.IsRequired()
				.HasMaxLength(500);
		});

		builder.OwnsOne(u => u.FullName, nameBuilder =>
		{
			nameBuilder.Property(n => n.FirstName)
				.HasColumnName("FirstName")
				.IsRequired()
				.HasMaxLength(100);

			nameBuilder.Property(n => n.LastName)
				.HasColumnName("LastName")
				.IsRequired()
				.HasMaxLength(100);

			nameBuilder.Property(n => n.Patronymic)
				.HasColumnName("Patronymic")
				.IsRequired(false)
				.HasMaxLength(100);
		});

		builder.Property(u => u.CreatedAt)
			.IsRequired();

		builder.Property(u => u.IsActive)
			.IsRequired();

	}

}
