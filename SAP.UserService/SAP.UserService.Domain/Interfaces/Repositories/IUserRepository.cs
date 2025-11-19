using SAP.UserService.Domain.Enitities;

namespace SAP.UserService.Domain.Interfaces.Repositories;

public interface IUserRepository
{
	Task<IEnumerable<User?>> GetAllUsersAsync();
	Task<User?> GetByIdAsync(Guid userId);
	Task<User?> GetByEmailAsync(string email);

	Task AddAsync(User user);
	Task AddOauthAsync(OAuthProvider user);
	Task UpdateAsync(User user); 
	Task DeleteAsync(Guid userId);
	Task<bool> ExistsByEmailAsync(string email);

}
