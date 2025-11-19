using SAP.UserService.Domain.Common;
using SAP.UserService.Domain.Interfaces.Services;

namespace SAP.UserService.Domain.ValueObjects;
public class Password: ValueObject
{
	public string Hash { get; }
	public bool IsHashed => Hash.StartsWith("$2");
	private Password(string hash) { Hash = hash; }

	public static Password Create(string password, IPasswordHasher hasher)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Пароль не может быть пустой", nameof(password));

		if (password.Length < 6)
			throw new ArgumentException("Пароль должен быть от 6 символов включительно!");

		if (!password.Any(char.IsDigit))
			throw new ArgumentException("Пароль должен содержать хотя бы одну цифру", nameof(password));

		if (!password.Any(char.IsLetter))
			throw new ArgumentException("Пароль должен содержать хотя бы одну букву", nameof(password));

		var hash = hasher.HashPassword(password);
		return new Password(hash);
	}
	public static Password FromHash(string hash)
	{
		if (string.IsNullOrWhiteSpace(hash))
			throw new ArgumentException("Hash cannot be empty", nameof(hash));

		return new Password(hash);
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
