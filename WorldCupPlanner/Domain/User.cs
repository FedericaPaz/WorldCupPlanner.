namespace Domain;

public class User
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string Password { get; private set; }

    public User(string firstName, string lastName, string email, DateTime birthDate, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        BirthDate = birthDate;
        Password = password;
    }
}