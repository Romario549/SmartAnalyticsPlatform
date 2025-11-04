using SAP.UserService.Application.DTOs;
using SAP.UserService.Application.Interfaces;
using SAP.UserService.Domain.Enitities;
using SAP.UserService.Domain.Interfaces.Repositories;
using SAP.UserService.Domain.Interfaces.Services;
using SAP.UserService.Domain.ValueObjects;

namespace SAP.UserService.Application.Services;
public class AuthService(
	IJWTTokenGenerator jwtTokenGenerator, 
	IUserRepository userRepository, IPasswordHasher passwordHasher): IAuthService
{
	private readonly IJWTTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
	private readonly IUserRepository _userRepository = userRepository;
	private readonly IPasswordHasher _passwordHasher = passwordHasher;

	public async Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName, string? patronymic=null)
	{
		if (await _userRepository.ExistsByEmailAsync(email))
			throw new ApplicationException($"Пользователь с почтой {email} уже существует!");

		var emailVo = new Email(email);
		var passwordVo = new Password(password, _passwordHasher); 
		var nameVo = new PersonName(firstName, lastName, patronymic);

		var user = new User(emailVo, passwordVo, nameVo);

		await _userRepository.AddAsync(user);
		var token = _jwtTokenGenerator.GenerateToken(user);
		var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

		return new AuthResult(
			user.Id,
			user.Email.Value,
			user.FullName.FirstName,
			user.FullName.LastName,
			token,
			refreshToken);
	}

	public async Task<AuthResult> LoginAsync(string email, string password)
	{
		var user = await _userRepository.GetByEmailAsync(email);
		if (user == null || !user.IsActive || !_passwordHasher.VerifyPassword(password, user.Password.Hash))
			throw new ApplicationException("Данные не корректны!");

		var token = _jwtTokenGenerator.GenerateToken(user);
		var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

		return new AuthResult(
			user.Id, 
			user.Email.Value, 
			user.FullName.FirstName, 
			user.FullName.LastName, 
			token, 
			refreshToken);
	}
}
