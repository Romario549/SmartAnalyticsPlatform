using SAP.UserService.Domain.Common;

namespace SAP.UserService.Domain.Enitities;
public class OAuthProvider: Entity
{
	public string ProviderName { get; private set; }
	public string ProviderUserId { get; private set; }
	public DateTime LinkedAt { get; private set; }
	public virtual User User { get; private set; }

	private OAuthProvider() { }

	public static OAuthProvider Create(string providerName, string providerUserId, User user)
	{
		return new OAuthProvider()
		{
			ProviderName = providerName,
			ProviderUserId = providerUserId,
			User = user,
			LinkedAt = DateTime.UtcNow
		};
	}
}
