using SAP.UserService.Domain.Enitities;

namespace SAP.UserService.Domain.Interfaces.Services;

public interface IOAuthService
{
	Task<string> GetOAuthLoginUrlAsync(string provider);
	Task<User> HandleOAuthCallbackAsync(string provider, string code);

}