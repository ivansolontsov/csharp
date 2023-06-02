namespace Test2.API.ViewModels.Requests;

public class CreateUserRequest
{
    public string email  { get; set; }
    public string password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}