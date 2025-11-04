using SAP.UserService.Domain.Common;
using System.Net.Mail;

namespace SAP.UserService.Domain.ValueObjects;

public class Email: ValueObject
{
	public string Value { get; }
	public string LocalPart => Value.Split('@')[0];
	public string Domain => Value.Split('@')[1];

	private Email() { Value = string.Empty; }

	public Email(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Почта не может быть пустой", nameof(value));

		value = value.Trim().ToLowerInvariant();

		if (!IsValidEmail(value))
			throw new ArgumentException("Invalid email format", nameof(value));

		Value = value;
	}

	private static bool IsValidEmail(string email)
	{
		try
		{
			var addr = new MailAddress(email);
			return addr.Address == email;
		}
		catch 
		{
			return false;
		}
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}
	public override string ToString() => Value;

	public static implicit operator string(Email email) => email.Value;

}
