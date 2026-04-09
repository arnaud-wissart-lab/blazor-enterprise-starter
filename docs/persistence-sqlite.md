# Persistence SQLite du backlog

Le projet utilise une persistence locale basée sur `SQLite` et `Entity Framework Core` pour le module backlog.

Le choix retenu reste simple :

- lancement local sans infrastructure externe
- fichier SQLite local
- `DbContext` EF Core dédié au backlog
- dépôt `SqliteBacklogRepository`
- initialisation automatique au démarrage
- seed minimal si la base est vide

## Choix d’implémentation

Le projet n’utilise pas encore de migrations versionnées. À ce stade, le choix retenu est `EnsureCreated` afin de :

- limiter le bruit d’infrastructure
- éviter une dépendance supplémentaire pour démarrer
- garder un socle facile à lire

## Emplacement de la base

Par défaut, le backend stocke la base dans :

- `src/BlazorEnterpriseStarter.Server/data/blazor-enterprise-starter.db` en lancement local

Chaîne de connexion par défaut :

```json
"ConnectionStrings": {
  "BacklogDatabase": "Data Source=data/blazor-enterprise-starter.db"
}
```

Le chemin relatif est résolu depuis le répertoire racine du projet serveur.

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

Dans les deux cas, aucune création manuelle de base n’est nécessaire.

## Lancement via Docker

Le `docker-compose.yml` monte un volume nommé sur `/app/data` pour conserver la base entre deux redémarrages.

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

## Limites actuelles

Cette persistence reste volontairement sobre :

- pas de migrations versionnées
- pas de concurrence avancée
- pas de découpage multi-contexte
- pas de base externe
