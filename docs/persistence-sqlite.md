# Persistence SQLite du backlog

Le projet utilise désormais une persistence locale basée sur `SQLite` et `Entity Framework Core` pour le module backlog.

L’objectif est volontairement pragmatique :

- conserver un lancement local très simple
- éviter une infrastructure externe
- rendre la démonstration plus crédible qu’un simple singleton mémoire
- garder une architecture lisible pour un recruteur technique

## Choix retenu

La solution met en place :

- un fichier SQLite local
- un `DbContext` EF Core dédié au backlog
- un dépôt `SqliteBacklogRepository`
- une initialisation automatique au démarrage
- un seed minimal si la base est vide

Le projet ne met pas encore en place de migrations versionnées. Pour cette vitrine, le choix retenu est `EnsureCreated` afin de :

- réduire le bruit d’infrastructure
- éviter de dépendre d’un outillage supplémentaire pour démarrer
- garder un socle simple à lire

Ce compromis est assumé : il améliore nettement la crédibilité technique par rapport à la mémoire en RAM, tout en restant léger.

## Emplacement de la base

Par défaut, le backend stocke la base dans :

- `src/BlazorEnterpriseStarter.Server/data/blazor-enterprise-starter.db` en lancement local

La chaîne de connexion par défaut est :

```json
"ConnectionStrings": {
  "BacklogDatabase": "Data Source=data/blazor-enterprise-starter.db"
}
```

Le chemin relatif est automatiquement résolu depuis le répertoire racine du projet serveur.

## Initialisation

Au démarrage du backend :

1. la base SQLite est créée si elle n’existe pas
2. la table `BacklogItems` est créée
3. un jeu initial d’éléments backlog est injecté uniquement si la base est vide

Le seed n’est donc pas rejoué à chaque lancement.

## Lancement local

### Via Aspire

```bash
dotnet run --project src/BlazorEnterpriseStarter.AppHost
```

### Via le backend seul

```bash
dotnet run --project src/BlazorEnterpriseStarter.Server
```

Dans les deux cas, aucune étape manuelle de création de base n’est nécessaire.

## Lancement via Docker

Le `docker-compose.yml` monte un volume nommé sur `/app/data` pour conserver la base entre deux redémarrages des conteneurs.

Commandes utiles :

```bash
docker compose up --build
docker compose down
```

Pour repartir d’une base vide :

```bash
docker compose down -v
```

## Réinitialiser la base locale

En local, il suffit de supprimer :

- `src/BlazorEnterpriseStarter.Server/data/blazor-enterprise-starter.db`

Puis de relancer le backend.

## Limites assumées

Cette persistence est volontairement sobre :

- pas de migrations versionnées pour le moment
- pas de concurrence avancée
- pas de découpage multi-contexte
- pas de base externe

Pour une vitrine GitHub, cela reste un bon compromis entre crédibilité, simplicité et coût de maintenance.
