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

Une seconde catégorie de scénarios existe désormais pour générer les captures d’écran de vitrine sans les mélanger aux tests de validation métier.

## Périmètre couvert

Les scénarios sont séparés en deux familles :

1. `Validation`
   - ouverture de la page backlog et vérification des éléments de synthèse principaux
   - création d’un élément de backlog puis recherche de cet élément

2. `Screenshots`
   - génération de `docs/screenshots/home-overview.png`
   - génération de `docs/screenshots/components-library.png`
   - génération de `docs/screenshots/backlog-module.png`

Ce découpage est volontaire :

- il couvre un vrai écran métier
- il montre un flux lisible
- il évite de mélanger validation fonctionnelle et production d’assets de vitrine

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

Génération des captures de vitrine :

```powershell
pwsh ./scripts/capture-screenshots.ps1
```

Génération avec navigateur visible :

```powershell
pwsh ./scripts/capture-screenshots.ps1 -Headed
```

Les captures sont écrites directement dans `docs/screenshots` avec des noms fixes et sont écrasées à chaque exécution.

## Convention de capture

Les captures de vitrine appliquent une stratégie volontairement sobre :

- viewport fixe `1600 x 1700`
- thème forcé en mode clair pour éviter une variation selon la machine locale
- animation et transitions neutralisées uniquement pendant la capture
- barre de thème flottante masquée pendant la capture pour ne pas masquer le contenu
- scrollbars masquées pendant la capture pour garder un cadrage propre
- capture de viewport, pas de `full page`, afin de conserver un rendu homogène dans le README

Chaque scénario attend explicitement les zones clés de la page avant la prise de vue pour éviter les états transitoires.

## AppHost et dashboard Aspire

La capture `docs/screenshots/apphost-dashboard.png` reste manuelle à ce stade.

La raison est pragmatique :

- le dashboard Aspire vit dans un processus distinct du front et de l’API déjà pilotés par les fixtures E2E
- l’URL de base peut être stabilisée via le profil `http`, mais l’accès complet au dashboard dépend d’une URL de connexion avec jeton éphémère fournie au démarrage
- automatiser cette capture imposerait donc de parser un log Aspire spécifique pour récupérer ce jeton, ce qui couple la génération d’assets à un détail d’implémentation peu rentable à maintenir

Le choix retenu est donc de garder les trois captures principales totalement automatisées et de documenter clairement que le dashboard AppHost reste une capture manuelle.

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

Dans ce cas, l’AppHost annonce une URL de connexion de type `http://localhost:15061/login?t=...`, dont le jeton reste éphémère et n’est pas exploité par l’automatisation du repo.

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
