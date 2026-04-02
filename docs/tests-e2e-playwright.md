# Tests E2E Playwright

Le dépôt inclut un projet E2E minimal basé sur `Playwright for .NET` pour démontrer un vrai flux utilisateur visible sans alourdir la stratégie de tests.

## Pourquoi ce choix

Le dépôt couvre déjà :

- les services métier
- la validation
- la gestion d’état
- une couche bUnit ciblée sur les composants et comportements UI Blazor

Le rôle des tests E2E est donc plus limité :

- vérifier qu’un parcours backlog visible fonctionne réellement dans le navigateur
- montrer une démarche qualité crédible sur un flux utilisateur complet
- rester maintenable

Le projet E2E reste volontairement séparé de la solution principale pour ne pas ralentir `dotnet test BlazorEnterpriseStarter.sln` dans la boucle de développement courante.

## Périmètre couvert

Deux scénarios seulement sont fournis :

1. ouverture de la page backlog et vérification des éléments de synthèse principaux
2. création d’un élément de backlog puis recherche de cet élément

Ce choix est volontaire :

- il couvre un vrai écran métier
- il montre un flux lisible
- il évite d’ajouter des scénarios artificiels

## Structure

```text
tests/
  BlazorEnterpriseStarter.E2ETests/
    Backlog/
    Infrastructure/
```

Le fixture de test :

- démarre le backend ASP.NET Core
- démarre l’application Blazor
- injecte une base SQLite temporaire isolée pour chaque session E2E
- arrête proprement les processus à la fin

## Installer Playwright

Les navigateurs Playwright ne sont pas versionnés dans le dépôt.

Installation :

```powershell
pwsh ./scripts/install-playwright.ps1
```

Cette commande :

- compile le projet E2E
- génère le script `playwright.ps1`
- installe `Chromium`, seul navigateur utilisé par les scénarios

## Lancer les tests E2E

Exécution standard :

```powershell
pwsh ./scripts/test-e2e.ps1
```

Exécution avec navigateur visible :

```powershell
pwsh ./scripts/test-e2e.ps1 -Headed
```

## Notes d’implémentation

Les scénarios E2E ne s’appuient ni sur Docker ni sur Aspire pour rester simples à lancer.

Le fixture démarre directement :

- `src/BlazorEnterpriseStarter.Server`
- `src/BlazorEnterpriseStarter.App`

avec :

- des ports HTTP éphémères
- une variable `PlatformApi__BaseUrl` pour relier le front au backend
- une base SQLite temporaire pour éviter toute pollution de la base locale de démonstration

## Références officielles

- Documentation Playwright .NET : https://playwright.dev/dotnet/docs/intro
- Guide d’installation du script navigateur : https://playwright.dev/dotnet/docs/library
