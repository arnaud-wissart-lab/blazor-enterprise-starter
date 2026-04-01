namespace BlazorEnterpriseStarter.Components.Common;

/// <summary>
/// Représente une option de sélection simple pour les listes déroulantes.
/// </summary>
/// <param name="Value">Valeur transmise lors de la sélection.</param>
/// <param name="Label">Texte affiché à l’utilisateur.</param>
/// <param name="IsDisabled">Indique si l’option est indisponible.</param>
public sealed record AppSelectOption(string Value, string Label, bool IsDisabled = false);
