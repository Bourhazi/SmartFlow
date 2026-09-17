namespace SmartFlow.Application.Common.Security;

public static class Roles
{
    public const string Collaborateur = "Collaborateur";
    public const string Manager = "Manager";
    public const string Administrateur = "Administrateur";

    public static readonly IReadOnlyCollection<string> All =
    [
        Collaborateur,
        Manager,
        Administrateur
    ];
}