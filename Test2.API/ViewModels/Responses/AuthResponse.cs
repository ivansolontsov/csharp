namespace Test2.API.ViewModels.Responses;

public class AuthResponse
{
    public string accessToken { get; set; }
    public string? refreshToken { get; set; }
}