# BlazorEnterpriseStarter

Socle de démonstration pour une application Blazor moderne, structurée pour un contexte professionnel avec une API ASP.NET Core, un design system réutilisable, une conteneurisation Docker simple et une orchestration .NET Aspire.

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
- Les conteneurs exécutent l’application en HTTP interne sur le port `8080`. En environnement déployé, la terminaison TLS a vocation à être assurée par le reverse proxy ou la plate-forme d’hébergement.

## Démarrage local

### Avec Aspire

```bash
dotnet run --project src/BlazorEnterpriseStarter.AppHost
```

Ports attendus en développement local :

- application Blazor : `https://localhost:7196` et `http://localhost:5184`
- API ASP.NET Core : `https://localhost:7005` et `http://localhost:5036`

Ce mode est le chemin principal pour le développement local. `AppHost` orchestre `App` et `Server`, injecte les informations de service discovery et permet de visualiser l’ensemble via le tableau de bord Aspire.

### Via Docker

```bash
docker compose up --build
```

Ports exposés :

- application Blazor : `http://localhost:8080`
- API ASP.NET Core : `http://localhost:8081`

Dans ce mode, l’application front contacte l’API via l’URL interne `http://server:8080` grâce à la variable d’environnement `PlatformApi__BaseUrl` définie dans [docker-compose.yml](C:/Users/ArnaudW/source/repos/blazor-enterprise-starter/docker-compose.yml).

Pour arrêter et nettoyer les conteneurs :

```bash
docker compose down
```

### Sans Aspire ni Docker

```bash
dotnet run --project src/BlazorEnterpriseStarter.Server
dotnet run --project src/BlazorEnterpriseStarter.App
```

Ce mode reste utile pour un débogage ciblé, mais il est moins confortable que l’orchestration Aspire.

## Conteneurisation

Les fichiers suivants constituent la base Docker :

- [src/BlazorEnterpriseStarter.App/Dockerfile](C:/Users/ArnaudW/source/repos/blazor-enterprise-starter/src/BlazorEnterpriseStarter.App/Dockerfile)
- [src/BlazorEnterpriseStarter.Server/Dockerfile](C:/Users/ArnaudW/source/repos/blazor-enterprise-starter/src/BlazorEnterpriseStarter.Server/Dockerfile)
- [docker-compose.yml](C:/Users/ArnaudW/source/repos/blazor-enterprise-starter/docker-compose.yml)
- [.dockerignore](C:/Users/ArnaudW/source/repos/blazor-enterprise-starter/.dockerignore)

Choix pragmatiques retenus :

- Dockerfiles multi-étapes pour produire des images propres et compactes
- pas de HTTPS à l’intérieur des conteneurs, afin d’éviter une complexité locale inutile
- ports internes unifiés à `8080` pour simplifier le raisonnement
- `docker-compose` minimal, limité au front et à l’API
- conservation d’Aspire comme orchestrateur local principal

## Cohérence des communications

- En mode Aspire, `App` résout `server` via la découverte de services et l’instruction `.WithReference(...)` de l’AppHost.
- En mode Docker, `App` bascule automatiquement sur la configuration `PlatformApi:BaseUrl`, injectée par `docker-compose`.
- Les endpoints `/health` et `/alive` sont exposés en développement et en conteneur pour faciliter la supervision locale et préparer un futur déploiement.

## Validation

```bash
dotnet restore
dotnet build BlazorEnterpriseStarter.sln
dotnet test BlazorEnterpriseStarter.sln
docker compose config
```

## Prochaines étapes naturelles

- ajouter une vraie couche application et domaine si l’on introduit des cas métier
- brancher une persistance ou un fournisseur externe
- enrichir le design system avec formulaires, tableaux et navigation
- préparer un déploiement derrière un reverse proxy ou une plate-forme managée
