using SAP.UserService.Domain.Common;
using SAP.UserService.Domain.Interfaces.Services;

namespace SAP.UserService.Domain.ValueObjects;
public class Password: ValueObject
{
	public string Hash { get; }
	public bool IsHashed => Hash.StartsWith("$2");
	private Password() { Hash = string.Empty; }

	public Password(string password, IPasswordHasher hasher, bool isHashed = false)
	{

		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Почта не может быть пустой", nameof(password));

		if (!isHashed)
		{
			if (password.Length < 6 || password.Length > 12)
				throw new ArgumentException("Пароль должен быть от 6 до 12 символов включительно!");

			if (!password.Any(char.IsDigit))
				throw new ArgumentException("Пароль должен содержать хотя бы одну цифру", nameof(password));

			if (!password.Any(char.IsLetter))
				throw new ArgumentException("Пароль должен содержать хотя бы одну букву", nameof(password));
		}

		Hash = isHashed ? password : hasher.HashPassword(password);

	}

	public bool Verify(string plainPassword, IPasswordHasher hasher) =>
		hasher.VerifyPassword(plainPassword, Hash);

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Hash;
	}

	public override string ToString() => "[PROTECTED]";


	public static implicit operator string(Password password) => password.Hash; 
}
