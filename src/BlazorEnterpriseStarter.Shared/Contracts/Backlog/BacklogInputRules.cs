namespace BlazorEnterpriseStarter.Shared.Contracts.Backlog;

/// <summary>
/// Centralise les contraintes de saisie et la normalisation simple du module backlog.
/// </summary>
public static class BacklogInputRules
{
    public const int RechercheLongueurMaximale = 120;
    public const int TitreLongueurMaximale = 120;
    public const int DescriptionLongueurMaximale = 2000;
    public const int TaillePageMinimale = 1;
    public const int TaillePageMaximale = 50;

    public static string? NormaliserRecherche(string? valeur)
    {
        var rechercheNormalisee = valeur?.Trim();
        return string.IsNullOrWhiteSpace(rechercheNormalisee) ? null : rechercheNormalisee;
    }

    public static string NormaliserTitre(string? valeur) =>
        NormaliserTexte(valeur, autoriserRetoursLigne: false);

    public static string NormaliserDescription(string? valeur) =>
        NormaliserTexte(valeur, autoriserRetoursLigne: true);

    public static bool ContientDesCaracteresInterditsPourRecherche(string? valeur) =>
        ContientDesCaracteresInterdits(valeur, autoriserRetoursLigne: false);

    public static bool ContientDesCaracteresInterditsPourTitre(string? valeur) =>
        ContientDesCaracteresInterdits(valeur, autoriserRetoursLigne: false);

    public static bool ContientDesCaracteresInterditsPourDescription(string? valeur) =>
        ContientDesCaracteresInterdits(valeur, autoriserRetoursLigne: true);

    private static string NormaliserTexte(string? valeur, bool autoriserRetoursLigne)
    {
        if (string.IsNullOrWhiteSpace(valeur))
        {
            return string.Empty;
        }

        var valeurNormalisee = autoriserRetoursLigne
            ? valeur.ReplaceLineEndings("\n")
            : valeur.ReplaceLineEndings(" ");

        return valeurNormalisee.Trim();
    }

    private static bool ContientDesCaracteresInterdits(string? valeur, bool autoriserRetoursLigne)
    {
        if (string.IsNullOrEmpty(valeur))
        {
            return false;
        }

        foreach (var caractere in valeur)
        {
            if (!char.IsControl(caractere))
            {
                continue;
            }

            if (autoriserRetoursLigne && caractere is '\r' or '\n' or '\t')
            {
                continue;
            }

            return true;
        }

        return false;
    }
}
