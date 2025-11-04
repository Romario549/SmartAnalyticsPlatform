using SAP.UserService.Domain.Common;
using static System.Text.RegularExpressions.Regex;

namespace SAP.UserService.Domain.ValueObjects;

public class PersonName: ValueObject
{
	public string FirstName	{ get; }
	public string LastName { get; }
	public string? Patronymic { get; }

	public string FullName => Patronymic == null
		? $"{FirstName} {LastName}"
		: $"{FirstName} {Patronymic} {LastName}";

	public string ShortName => Patronymic == null
		? $"{FirstName} {LastName}"
		: $"{LastName} {FirstName.First()}.{Patronymic.First()}.";

	private PersonName()
	{
		FirstName = string.Empty;
		LastName = string.Empty;
	}
	public PersonName(string firstName, string lastName, string? patronymic = null)
	{

		if (string.IsNullOrWhiteSpace(firstName))
			throw new ArgumentException("First name cannot be empty", nameof(firstName));

		if (string.IsNullOrWhiteSpace(lastName))
			throw new ArgumentException("Last name cannot be empty", nameof(lastName));

		if (firstName.Length < 2 || firstName.Length > 50)
			throw new ArgumentException("First name must be between 2 and 50 characters", nameof(firstName));

		if (lastName.Length < 2 || lastName.Length > 50)
			throw new ArgumentException("Last name must be between 2 and 50 characters", nameof(lastName));

		if (patronymic != null && (patronymic.Length < 2 || patronymic.Length > 50))
			throw new ArgumentException("Patronymic must be between 2 and 50 characters", nameof(patronymic));

		if (!IsValidName(firstName))
			throw new ArgumentException("First name contains invalid characters", nameof(firstName));

		if (!IsValidName(lastName))
			throw new ArgumentException("Last name contains invalid characters", nameof(lastName));

		if (patronymic != null && !IsValidName(patronymic))
			throw new ArgumentException("Patronymic contains invalid characters", nameof(patronymic));

		FirstName = Capitalize(firstName.Trim());
		LastName = Capitalize(lastName.Trim());
		Patronymic = patronymic?.Trim() == string.Empty ? null : Capitalize(patronymic?.Trim());
	}

	private static bool IsValidName(string name) => IsMatch(name, @"^[\p{L}\s'-]+$");

	private static string Capitalize(string? name)
	{
		if (string.IsNullOrEmpty(name)) return string.Empty;

		return string.Join(" ", name.Split(' ')
			.Select(part =>
				part.Length > 0
					? char.ToUpper(part[0]) + part.Substring(1).ToLower()
					: part));
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return FirstName;
		yield return LastName;
		yield return Patronymic ?? string.Empty;
	}
	public override string ToString() => FullName;

	public (string FirstName, string LastName, string? Patronymic) Deconstruct()
	{
		return (FirstName, LastName, Patronymic);
	}
}
