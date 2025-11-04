using SAP.UserService.Domain.Common;
using SAP.UserService.Domain.Interfaces.Services;
using SAP.UserService.Domain.ValueObjects;

namespace SAP.UserService.Domain.Enitities;
public class User : Entity
{
	public Email Email { get; private set; }
	public Password Password { get; private set; }
	public PersonName FullName { get; private set; }
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
	public bool IsActive { get; private set; } = true;
	public virtual ICollection<OAuthProvider> OAuthProviders { get; private set; } = new List<OAuthProvider>();


	public void UpdateName(PersonName newName) => FullName = newName;

	public void ChangeEmail(Email newEmail) => Email = newEmail;

	public void ChangePassword(Password newPassword) => Password = newPassword;

	public bool VerifyPassword(string plainPassword, IPasswordHasher hasher) => 
		Password.Verify(plainPassword, hasher);

	public void Deactivate() => IsActive = false;

	public User(Email email, Password password, PersonName fullName)
	{
		Email = email;
		Password = password;
		FullName = fullName;
	}
	public void AddOAuthProvider(string providerName, string providerUserId)
	{
		var provider = OAuthProvider.Create(providerName, providerUserId, this);
		OAuthProviders.Add(provider);
	}
	private User() { }

}
