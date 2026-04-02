# BlazorEnterpriseStarter

BlazorEnterpriseStarter est une solution de démonstration construite pour illustrer une approche professionnelle d’une application métier moderne en .NET.

Le projet met en avant :

- une interface Blazor Web App structurée et crédible pour un contexte professionnel
- une API ASP.NET Core dédiée aux cas d’usage métier
- un design system et une bibliothèque de composants réutilisables
- une orchestration locale lisible avec .NET Aspire
- une conteneurisation simple avec Docker
- une persistence SQLite légère via EF Core sur le module backlog
- une base de tests unitaires, bUnit et E2E ciblée sur les comportements à forte valeur
- une CI GitHub minimale pour vérifier la build, les tests et la configuration Docker

L’objectif n’est pas de livrer un produit complet, mais un socle vitrine lisible, maintenable et démontrable localement ou sur GitHub.

## Objectifs techniques

Le projet a été conçu pour démontrer de manière concrète :

- une séparation claire entre UI, logique applicative, contrats partagés et backend
- une organisation cohérente d’une solution .NET multi-projets
- une approche sobre et réutilisable du design system et des composants Blazor
- un module métier simple mais crédible autour d’un backlog produit
- une exécution locale propre, à la fois via Aspire et via Docker
- une base saine pour évoluer ensuite vers des migrations versionnées, de l’authentification ou d’autres modules métier

## Architecture de la solution

```text
/src
  BlazorEnterpriseStarter.AppHost
  BlazorEnterpriseStarter.ServiceDefaults
  BlazorEnterpriseStarter.App
  BlazorEnterpriseStarter.Server
  BlazorEnterpriseStarter.Shared
  BlazorEnterpriseStarter.Components
  BlazorEnterpriseStarter.DesignSystem

/tests
  BlazorEnterpriseStarter.Tests
  BlazorEnterpriseStarter.E2ETests
```

### Rôle des projets

- `BlazorEnterpriseStarter.AppHost`
  Orchestration locale avec .NET Aspire, composition des services et supervision de l’ensemble.

- `BlazorEnterpriseStarter.ServiceDefaults`
  Configuration transverse Aspire pour la découverte de services, la résilience HTTP, la télémétrie et les endpoints de santé.

- `BlazorEnterpriseStarter.App`
  Application Blazor Web App, point d’entrée front-end de la démonstration.

- `BlazorEnterpriseStarter.Server`
  API ASP.NET Core pour les fonctionnalités métier, la santé applicative et l’exposition du module backlog.

- `BlazorEnterpriseStarter.Shared`
  Contrats partagés entre le front, l’API et les tests.

- `BlazorEnterpriseStarter.Components`
  Composants Blazor réutilisables orientés usages d’interface.

- `BlazorEnterpriseStarter.DesignSystem`
  Fondations visuelles, tokens CSS, layout, primitives UI et conventions de composition.

- `BlazorEnterpriseStarter.Tests`
  Tests unitaires xUnit ciblant en priorité les services applicatifs, la gestion d’état et les comportements à forte valeur démonstrative.

- `BlazorEnterpriseStarter.E2ETests`
  Tests end-to-end Playwright ciblés sur un flux utilisateur visible, volontairement séparés de la solution principale pour préserver une boucle locale rapide.

- `.github/workflows/ci.yml`
  Pipeline GitHub Actions minimal pour compiler la solution, exécuter les tests du dépôt et valider la configuration Docker.

### Vue d’ensemble

```text
BlazorEnterpriseStarter.App
  -> consomme BlazorEnterpriseStarter.Components
  -> consomme BlazorEnterpriseStarter.DesignSystem
  -> consomme BlazorEnterpriseStarter.Shared
  -> appelle BlazorEnterpriseStarter.Server via client API dédié

BlazorEnterpriseStarter.Server
  -> consomme BlazorEnterpriseStarter.Shared
  -> expose les endpoints métier et de santé

BlazorEnterpriseStarter.AppHost
  -> orchestre App et Server via .NET Aspire
```

## Rôle des technologies

### Blazor

Blazor porte l’interface utilisateur, la structure applicative, les pages de démonstration et le module backlog. Il permet ici de montrer :

- une `Blazor Web App` assumée en mode `InteractiveServer`
- une UI modulaire
- des composants réutilisables
- une gestion d’état locale claire
- une expérience cohérente entre vitrine UI et module métier

### Positionnement Blazor

Le projet ne cherche pas à montrer tous les modes de rendu Blazor à la fois.

Il assume un choix simple et lisible :

- application `Blazor Web App`
- rendu interactif côté serveur
- composants Razor réutilisables
- état local ciblé sur les cas d’usage métier

Ce choix est volontaire :

- il maximise la lisibilité du dépôt
- il met en avant la composition Razor et l’architecture front
- il évite d’ajouter de la complexité démonstrative peu rentable à ce stade

Une note dédiée détaille ce positionnement et ses compromis :

- `docs/positionnement-blazor.md`

### ASP.NET Core

ASP.NET Core est utilisé pour l’API backend. Son rôle est de :

- exposer un backend proprement séparé du front
- porter les cas d’usage du backlog
- gérer la recherche, le filtrage, le tri et la pagination côté serveur
- exposer les endpoints de santé

### Docker

Docker fournit un mode de lancement simple et démontrable hors Aspire. Il permet de :

- lancer rapidement le front et l’API dans des conteneurs distincts
- valider la cohérence des URLs inter-services
- conserver la base SQLite du backlog dans un volume nommé
- préparer un futur déploiement avec une base conteneurisée propre

### .NET Aspire

.NET Aspire est le mode principal d’orchestration locale. Il apporte :

- une composition claire des services
- la découverte de services
- des health checks intégrés
- une expérience de développement plus lisible pour une solution distribuée, même légère

## Choix de conception principaux

### 1. Front-end, backend et contrats explicitement séparés

Le front Blazor, l’API ASP.NET Core et les contrats partagés vivent dans des projets distincts. Cette séparation améliore la lisibilité, limite le couplage et prépare mieux les évolutions d’un vrai produit.

### 2. Design system distinct de la bibliothèque de composants

Le design system porte les fondations visuelles. Les composants applicatifs les composent ensuite pour répondre à des usages réels. Ce découpage est plus crédible qu’un mélange de styles et de composants dans l’application elle-même.

### 3. Gestion d’état pragmatique sur le module backlog

Le module backlog évite les appels HTTP directs dans les composants Razor. Il s’appuie sur :

- un client API dédié
- une classe d’état locale
- des notifications explicites de changement
- une gestion claire des états `chargement`, `succès` et `erreur`

L’objectif est d’obtenir une architecture compréhensible rapidement, sans introduire une solution de type Redux qui ne serait pas justifiée ici.

### 4. Persistence SQLite légère et crédible

Le backlog repose désormais sur une persistence `SQLite` locale via `Entity Framework Core`.

Le choix reste volontairement sobre :

- aucun service externe n’est requis
- la base est créée automatiquement au démarrage
- un jeu initial est injecté uniquement si la base est vide
- l’architecture reste lisible grâce à un dépôt dédié et un `DbContext` ciblé

La solution n’ajoute pas encore de migrations versionnées, afin de conserver un coût d’entrée faible pour la vitrine.

### 5. Optimisations limitées aux gains utiles

Le projet applique quelques optimisations à vrai bénéfice :

- pagination serveur
- debounce sur la recherche
- conservation des résultats déjà chargés lors d’un rafraîchissement échoué
- retours visuels de chargement sobres

La virtualisation, par exemple, n’a pas été ajoutée sur le backlog car la pagination serveur actuelle la rend peu rentable à ce stade.

### 6. Garde-fous de saisie et sécurité de base

Le projet ne prétend pas couvrir toute la profondeur d’une application exposée à Internet, mais il applique les garde-fous attendus sur une vitrine sérieuse :

- validation côté interface pour raccourcir la boucle de feedback utilisateur
- validation côté API pour ne jamais faire confiance au client
- normalisation des saisies métier avant persistence
- rejet des caractères de contrôle non pris en charge dans les champs backlog
- rendu des contenus via Razor sans HTML brut injecté dans l’interface
- antiforgery activé côté application Blazor

## Points forts UI/UX

Le projet cherche un rendu sérieux, moderne et exploitable dans un contexte métier.

Les principaux partis pris sont :

- un layout principal clair, avec navigation, en-tête et zone de contenu stable
- un design system sobre, premium et maintenable
- une hiérarchie visuelle nette, utile pour des captures d’écran GitHub
- une page d’accueil pensée comme une vitrine technique et non comme un simple placeholder
- une page dédiée au design system et aux composants
- un module backlog démonstratif, avec recherche, filtres, tri, pagination et CRUD

Pages actuellement mises en avant :

- `/`
  Accueil vitrine du projet

- `/composants`
  Démonstration du design system et des composants réutilisables

- `/backlog`
  Module métier démonstratif de backlog produit

## Instructions de lancement

### Prérequis

- SDK .NET 10
- Docker Desktop si vous souhaitez lancer la solution en conteneurs
- workload Aspire disponible localement pour le lancement via AppHost

### Lancement via Aspire

```bash
dotnet run --project src/BlazorEnterpriseStarter.AppHost
```

Ce mode est le chemin principal pour le développement local.

La base SQLite du backlog est créée automatiquement dans :

- `src/BlazorEnterpriseStarter.Server/data/blazor-enterprise-starter.db`

Ports attendus en développement :

- application Blazor : `https://localhost:7196` et `http://localhost:5184`
- API ASP.NET Core : `https://localhost:7005` et `http://localhost:5036`

Ce lancement permet de bénéficier :

- de l’orchestration des services
- de la découverte de services entre `app` et `server`
- du tableau de bord Aspire
- des health checks intégrés

### Lancement via Docker

```bash
docker compose up --build
```

Ports exposés :

- application Blazor : `http://localhost:8080`
- API ASP.NET Core : `http://localhost:8081`

Dans ce mode, le front contacte l’API via l’URL interne `http://server:8080`.

Le backend monte un volume Docker nommé pour conserver la base SQLite entre deux redémarrages.

Pour arrêter les conteneurs :

```bash
docker compose down
```

### Vérifications utiles

- Front : `http://localhost:8080` en mode Docker
- API backlog : `http://localhost:8081/api/backlog-items` en mode Docker
- Santé : `http://localhost:8081/health` en mode Docker

### Validation locale

```bash
dotnet build BlazorEnterpriseStarter.sln
dotnet test BlazorEnterpriseStarter.sln
dotnet build tests/BlazorEnterpriseStarter.E2ETests/BlazorEnterpriseStarter.E2ETests.csproj
docker compose config
```

### Documentation persistence

La mise en place SQLite et les choix d’implémentation sont détaillés dans :

- `docs/persistence-sqlite.md`

### Tests end-to-end

Une couche E2E Playwright minimale est disponible dans :

- `docs/tests-e2e-playwright.md`

Installation du navigateur utilisé :

```powershell
pwsh ./scripts/install-playwright.ps1
```

Exécution des scénarios E2E :

```powershell
pwsh ./scripts/test-e2e.ps1
```

### Intégration continue

Le dépôt inclut une CI GitHub Actions volontairement sobre :

- compilation de la solution
- exécution des tests unitaires et composants
- compilation du projet E2E
- validation de `docker compose config`

## Captures d’écran

Le dossier `docs/screenshots` est prêt à recevoir des captures ou GIFs à mesure que la vitrine visuelle se stabilise.

Suggestions de captures :

- accueil du projet
- page de démonstration du design system et des composants
- module backlog produit
- vue Aspire locale

Exemple de structure à compléter :

```md
![Accueil](docs/screenshots/home.png)
![Composants](docs/screenshots/components.png)
![Backlog](docs/screenshots/backlog.png)
```

## Pistes d’évolution

Les évolutions naturelles du projet seraient :

- introduire des migrations EF Core versionnées si la vitrine gagne en profondeur métier
- ajouter une authentification et une gestion des rôles
- enrichir le backlog avec des commentaires, affectations ou workflows
- introduire des tests de composants Blazor avec bUnit si le périmètre UI s’élargit
- ajouter une stratégie de déploiement plus complète avec reverse proxy ou plate-forme managée
- préparer une CI plus poussée avec analyse statique et publication d’images

## Audit technique

Un audit ciblé du dépôt et un plan d’amélioration priorisé sont disponibles dans :

- `docs/audit-technique-2026-04.md`
- `docs/positionnement-blazor.md`
- `docs/persistence-sqlite.md`
- `docs/tests-e2e-playwright.md`

## Valeur démonstrative pour un recruteur technique

Ce dépôt permet d’évaluer rapidement plusieurs dimensions utiles dans un contexte de recrutement :

- capacité à structurer une solution .NET proprement
- compréhension des frontières entre front, API, contrats partagés et orchestration
- maîtrise de Blazor au-delà d’une simple page de démonstration
- capacité à construire un design system et des composants réutilisables
- sens de la sobriété UI/UX dans un cadre métier
- pragmatisme dans les arbitrages techniques
- qualité de base sur les tests unitaires et la lisibilité du code

Autrement dit, le projet ne cherche pas à impressionner par le volume, mais par la cohérence technique, la lisibilité et la crédibilité de ses choix.
