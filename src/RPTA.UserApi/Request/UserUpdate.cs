namespace RPTA.UserApi.Request;

public record UserUpdate
{
    public required string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
