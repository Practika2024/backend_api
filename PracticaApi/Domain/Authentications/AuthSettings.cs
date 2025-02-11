namespace Domain.Authentications;

public static class AuthSettings
{
    public const string AdminRole = "Administrator";
    public const string OperatorRole = "Operator";

    public static readonly List<string> ListOfRoles = new()
    {
        AdminRole,
        OperatorRole
    };
}