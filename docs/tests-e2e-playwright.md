# Tests E2E Playwright

Le dépôt inclut un projet E2E basé sur `Playwright for .NET` pour valider un parcours backlog visible et générer les captures utilisées dans la présentation GitHub.

## Rôle du projet E2E

Le dépôt couvre déjà :

- les services métier
- la validation
- la gestion d’état
- une couche bUnit ciblée sur les composants et comportements UI Blazor

Le rôle des tests E2E est donc volontairement ciblé :

- vérifier qu’un parcours backlog fonctionne dans le navigateur
- garder un contrôle visible sur les interactions principales
- produire les captures de référence sans mélanger cette logique avec les tests métier

Le projet E2E reste séparé de la solution principale pour ne pas ralentir `dotnet test BlazorEnterpriseStarter.sln`.

## Périmètre couvert

Les scénarios sont séparés en deux familles :

1. `Validation`
   - ouverture de la page backlog et vérification des éléments de synthèse principaux
   - création d’un élément de backlog puis recherche de cet élément

2. `Screenshots`
   - génération de `docs/screenshots/home-overview.png`
   - génération de `docs/screenshots/components-library.png`
   - génération de `docs/screenshots/backlog-module.png`

## Structure

```text
tests/
  BlazorEnterpriseStarter.E2ETests/
    Backlog/
    Infrastructure/
    Screenshots/
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

Cette commande ne lance que les scénarios tagués `Validation`.

Exécution avec navigateur visible :

```powershell
pwsh ./scripts/test-e2e.ps1 -Headed
```

Génération des captures :

```powershell
pwsh ./scripts/capture-screenshots.ps1
```

Génération avec navigateur visible :

```powershell
pwsh ./scripts/capture-screenshots.ps1 -Headed
```

Les captures sont écrites directement dans `docs/screenshots` avec des noms fixes et sont écrasées à chaque exécution.

## Convention de capture

Les captures appliquent une stratégie simple :

- viewport fixe `1600 x 1700`
- thème forcé en mode clair
- animations et transitions neutralisées pendant la capture
- barre de thème flottante masquée pendant la capture
- scrollbars masquées pendant la capture
- capture de viewport, pas de `full page`

Chaque scénario attend explicitement les zones clés de la page avant la prise de vue pour éviter les états transitoires.

## AppHost et dashboard Aspire

La capture `docs/screenshots/apphost-dashboard.png` reste manuelle à ce stade.

La raison est pragmatique :

- le dashboard Aspire vit dans un processus distinct du front et de l’API pilotés par les fixtures E2E
- l’accès complet au dashboard dépend d’une URL de connexion avec jeton éphémère fournie au démarrage
- automatiser cette capture demanderait donc de récupérer ce jeton au lancement

## Capture manuelle du dashboard Aspire

Checklist :

1. lancer `dotnet run --project src/BlazorEnterpriseStarter.AppHost`
2. ouvrir l’URL de dashboard affichée par l’AppHost dans la console
3. attendre que les ressources `app` et `server` soient visibles dans l’écran de supervision
4. capturer la vue d’ensemble du dashboard
5. enregistrer l’image sous `docs/screenshots/apphost-dashboard.png`

Si vous souhaitez forcer une URL HTTP stable en local, vous pouvez utiliser :

```powershell
$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = "true"
dotnet run --project src/BlazorEnterpriseStarter.AppHost --launch-profile http
```

Dans ce cas, l’AppHost annonce une URL de connexion de type `http://localhost:15061/login?t=...`, dont le jeton reste éphémère.

## Notes d’implémentation

Les scénarios E2E ne s’appuient ni sur Docker ni sur Aspire pour rester simples à lancer.

Le fixture démarre directement :

- `src/BlazorEnterpriseStarter.Server`
- `src/BlazorEnterpriseStarter.App`

avec :

- des ports HTTP éphémères
- une variable `PlatformApi__BaseUrl` pour relier le front au backend
- une base SQLite temporaire pour éviter toute pollution de la base locale

## Références officielles

- Documentation Playwright .NET : https://playwright.dev/dotnet/docs/intro
- Guide d’installation du script navigateur : https://playwright.dev/dotnet/docs/library
