namespace SmartFlow.Infrastructure.Identity;

public static class AppRoles
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