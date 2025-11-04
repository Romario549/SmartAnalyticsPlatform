using SAP.UserService.Application.DTOs;

namespace SAP.UserService.Application.Interfaces;
public interface IAuthService
{
	Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName, string? patronymic = null);
	Task<AuthResult> LoginAsync(string email, string password);
}
