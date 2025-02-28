namespace RPTA.UserApi.Request;

public record UserCreate
{
    public required string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
}
