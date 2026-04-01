# BlazorEnterpriseStarter

Socle de démonstration pour une application Blazor moderne, structurée pour un contexte professionnel avec une API ASP.NET Core, un design system réutilisable et une orchestration .NET Aspire.

## Objectif de ce premier incrément

Cette première étape pose une base minimale viable, démontrable localement et prête à être enrichie :

- solution .NET complète avec séparation claire des responsabilités
- application `Blazor Web App` pour la vitrine front-end
- backend `ASP.NET Core Web API` pour les capacités métier et la supervision
- design system et composants réutilisables isolés dans des bibliothèques dédiées
- orchestration locale via `.NET Aspire`
- endpoint de vérification de santé et client front-end pour l’interroger

## Structure de la solution

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
```

## Rôle des projets

- `BlazorEnterpriseStarter.AppHost` : orchestration locale avec Aspire, composition des services et dépendances d’exécution.
- `BlazorEnterpriseStarter.ServiceDefaults` : configuration transversale Aspire pour la découverte de services, la résilience HTTP, la télémétrie et les endpoints de santé.
- `BlazorEnterpriseStarter.App` : interface Blazor Web App, point d’entrée de la démonstration utilisateur.
- `BlazorEnterpriseStarter.Server` : API ASP.NET Core destinée aux fonctionnalités métier et à la supervision technique.
- `BlazorEnterpriseStarter.Shared` : contrats partagés entre le front-end, l’API et les tests.
- `BlazorEnterpriseStarter.Components` : composants réutilisables orientés usage métier ou présentation d’écrans.
- `BlazorEnterpriseStarter.DesignSystem` : primitives d’interface, styles, tokens visuels et layout réutilisable.
- `BlazorEnterpriseStarter.Tests` : tests unitaires ciblant d’abord les contrats et règles simples du socle.

## Structure interne proposée

### `BlazorEnterpriseStarter.App`

- `Components/Layout` : layout global, gestion de la reconnexion et chrome applicatif
- `Components/Pages` : pages routées de la démonstration
- `Services` : clients HTTP et orchestration front-end
- `wwwroot` : styles et ressources propres à l’application

### `BlazorEnterpriseStarter.Server`

- `Endpoints` : endpoints minimaux ou groupes d’endpoints
- `Program.cs` : composition de l’application et pipeline HTTP
- `appsettings*.json` : configuration d’environnement

### `BlazorEnterpriseStarter.Shared`

- `Contracts` : DTO, constantes de routes et modèles partagés

### `BlazorEnterpriseStarter.Components`

- `Monitoring` : composants réutilisables de supervision et de tableau de bord

### `BlazorEnterpriseStarter.DesignSystem`

- `Layout` : gabarits de page et structure visuelle
- `Elements` : briques UI atomiques et réutilisables
- `wwwroot/styles` : tokens, variables CSS et styles transverses

### `BlazorEnterpriseStarter.Tests`

- `Shared` : tests des contrats et conventions du socle

## Choix d’architecture

### 1. Séparation nette entre UI, API et contrats

Le front-end Blazor et l’API sont déployables séparément. Cette séparation améliore la lisibilité de l’architecture, permet de faire évoluer la sécurité ou la scalabilité indépendamment et évite de mélanger responsabilités visuelles et responsabilités métier.

### 2. Design system dédié

Le design system n’est pas mélangé avec l’application finale. Les primitives visuelles restent réutilisables pour d’autres écrans ou d’autres projets, ce qui correspond mieux à une démarche produit ou plate-forme.

### 3. Composants métier réutilisables

Les composants orientés usage, comme une carte de supervision, sont isolés du design system. Cela permet de garder une frontière claire entre style générique et composition fonctionnelle.

### 4. Aspire dès le socle

L’AppHost et les Service Defaults sont introduits immédiatement pour rendre la démonstration crédible dans un contexte moderne : découverte de services, santé, résilience et observabilité sont prévues dès l’initialisation du projet.

## Hypothèses techniques

- La solution cible `.NET 10`, cohérent avec le SDK et les templates Aspire installés localement.
- Le premier incrément privilégie un socle démontrable plutôt qu’une logique métier complète.
- La conteneurisation Docker n’est pas encore détaillée dans cette étape, mais la séparation `App` / `Server` / `AppHost` la prépare proprement.

## Démarrage local

### Avec Aspire

```bash
dotnet run --project src/BlazorEnterpriseStarter.AppHost
```

### Sans Aspire

```bash
dotnet run --project src/BlazorEnterpriseStarter.Server
dotnet run --project src/BlazorEnterpriseStarter.App
```

## Validation

```bash
dotnet restore
dotnet build BlazorEnterpriseStarter.sln
dotnet test BlazorEnterpriseStarter.sln
```

## Prochaines étapes naturelles

- ajouter une vraie couche application et domaine si l’on introduit des cas métier
- brancher une persistance ou un fournisseur externe
- enrichir le design system avec formulaires, tableaux et navigation
- préparer les Dockerfiles et la stratégie de déploiement
