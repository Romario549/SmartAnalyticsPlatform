using Microsoft.EntityFrameworkCore;
using SAP.UserService.Domain.Enitities;
using SAP.UserService.Domain.Interfaces.Repositories;

namespace SAP.UserService.Infrastructure;
public class UserRepository(UserDbContext context) : IUserRepository
{
	private readonly UserDbContext _context = context;
	private readonly DbSet<User> _users = context.Set<User>();
	private readonly DbSet<OAuthProvider> _oauths = context.Set<OAuthProvider>();

	public async Task AddAsync(User user)
	{
		await _users.AddAsync(user);
		await _context.SaveChangesAsync();
	}
	public async Task AddOauthAsync(OAuthProvider user)
	{
		await _oauths.AddAsync(user);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Guid userId)
	{
		var user = await _users.FirstOrDefaultAsync(x => x.Id == userId);
		if (user != null)
		{
			_users.Remove(user);
			await _context.SaveChangesAsync();
		}
	}

	public async Task<bool> ExistsByEmailAsync(string email) =>
		await _users
			.AsNoTracking()
			.AnyAsync(x => x.Email.Value == email);

	public async Task<User?> GetByEmailAsync(string email) =>
		await _users.FirstOrDefaultAsync(x => x.Email.Value == email);

	public async Task<User?> GetByIdAsync(Guid userId) =>
		await _users.FirstOrDefaultAsync(x => x.Id == userId);

	public async Task<IEnumerable<User?>> GetAllUsersAsync() =>
		await _users.ToListAsync();

	public async Task UpdateAsync(User user)
	{
		context.Entry(user).State = EntityState.Modified;

		//В данном случае применяется отсележивание изменений сущности для практики
		context.ChangeTracker.DetectChanges();
		Console.WriteLine(context.ChangeTracker.DebugView.LongView);
		await _context.SaveChangesAsync();
	}
}
