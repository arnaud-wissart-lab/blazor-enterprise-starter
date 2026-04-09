# Audit technique ciblé

Date : 02/04/2026

Périmètre audité :
- `BlazorEnterpriseStarter.AppHost`
- `BlazorEnterpriseStarter.ServiceDefaults`
- `BlazorEnterpriseStarter.App`
- `BlazorEnterpriseStarter.Server`
- `BlazorEnterpriseStarter.Shared`
- `BlazorEnterpriseStarter.Components`
- `BlazorEnterpriseStarter.DesignSystem`
- `BlazorEnterpriseStarter.Tests`

Méthode :
- lecture de la structure de solution et des points d’entrée
- inspection ciblée des pages, composants, styles, services, endpoints, règles métier et tests
- exécution de `dotnet build BlazorEnterpriseStarter.sln`
- exécution de `dotnet test BlazorEnterpriseStarter.sln`
- vérification de `docker compose config`

## Synthèse

Le socle est sain. Le projet présente une intention d’architecture claire, un design system séparé, un cas d’usage métier lisible et une base de tests utile.

Le dépôt reste aujourd’hui au niveau d’un bon starter avancé, avec encore quelques limites structurantes :
- un positionnement Blazor encore trop implicite
- une expérience UI sérieuse mais inégale dans les finitions
- un seul module métier réel
- une persistance mémoire qui reste acceptable pour le démarrage mais affaiblit le discours "enterprise"
- une couverture de tests presque exclusivement métier, sans tests de composants ni d’intégration HTTP
- une présentation GitHub longtemps limitée par l’absence de CI visible, de captures stabilisées et d’un plan d’évolution formalisé

Décision recommandée :
- conserver le socle actuel
- éviter toute refonte massive
- renforcer progressivement la crédibilité du dépôt par petites itérations à fort impact

## Diagnostic projet par projet

### `BlazorEnterpriseStarter.AppHost`

Constats :
- L’orchestration Aspire est simple, lisible et cohérente avec le périmètre du dépôt.
- Le couplage avec `server` est explicite via `WithReference`, `WaitFor` et les health checks.
- Le fichier `AppHost.cs` reste volontairement minimal, ce qui facilite la lecture rapide.

Risques :
- La partie Aspire reste basique : aucun service additionnel, aucune ressource externe, aucun scénario de résilience visible.
- Elle montre l’intention, sans encore couvrir un écosystème distribué plus riche.

Actions recommandées :
- Conserver la simplicité actuelle.
- Ajouter à moyen terme une ressource technique légère et utile au périmètre, par exemple une base relationnelle pour le backlog.
- Documenter explicitement ce que démontre Aspire ici et ce qui est volontairement hors périmètre.

### `BlazorEnterpriseStarter.ServiceDefaults`

Constats :
- Le projet centralise proprement la télémétrie, les health checks, la découverte de services et la résilience HTTP.
- Les conventions Aspire sont regroupées dans `Extensions.cs`, ce qui évite la duplication.
- L’exclusion des endpoints de santé du tracing est un bon détail de maturité.

Risques :
- La couche est bonne techniquement, mais peu visible depuis GitHub si elle n’est pas illustrée dans la documentation ou la CI.
- Aucun test ne verrouille ces conventions transverses.

Actions recommandées :
- Garder cette structure.
- Ajouter au moins un test d’intégration léger ou une vérification documentaire sur les endpoints `/health` et `/alive`.
- Montrer dans la documentation la valeur réelle de cette couche.

### `BlazorEnterpriseStarter.App`

Constats :
- Le positionnement réel est celui d’une `Blazor Web App` en `InteractiveServer`, pas d’une application WebAssembly.
- La séparation entre pages, services HTTP et état local est lisible.
- Le module backlog évite les appels API directs depuis le Razor, ce qui améliore la maintenabilité.
- La page d’accueil et la page composants jouent bien leur rôle de pages de référence.

Faiblesses :
- Le code de page backlog concentre encore beaucoup de responsabilités dans un seul composant code-behind.
- Les `CancellationToken` ne sont pas propagés de bout en bout côté UI métier ; beaucoup d’appels partent avec `CancellationToken.None`.
- Les filtres ne semblent pas synchronisés avec l’URL, ce qui limite les liens profonds, le partage d’état et la lisibilité de la navigation applicative.
- Le discours Blazor réel n’est pas assez explicite dans le dépôt : rendu interactif côté serveur, pas de SSR pur, pas de WASM.

Risques :
- Si le backlog grossit, la page deviendra vite difficile à faire évoluer.
- L’absence de synchronisation avec l’URL réduit la crédibilité d’un module métier "prêt à évoluer".
- Un lecteur peut surestimer ou mal comprendre la nature exacte du choix Blazor.

Actions recommandées :
- Extraire progressivement la logique backlog la plus dense vers des classes ou services d’orchestration ciblés.
- Introduire la synchronisation des filtres principaux dans la query string.
- Clarifier dans la documentation que le projet démontre surtout Blazor `InteractiveServer`.

### `BlazorEnterpriseStarter.Server`

Constats :
- Le backend est proprement séparé.
- Le couple `Endpoints` / `Application` / `Infrastructure` / `Domain` est simple mais crédible.
- Les validations sont explicites et cohérentes avec les règles partagées.
- Les endpoints minimal API restent lisibles et bien nommés.

Faiblesses :
- La persistance mémoire singleton limite fortement le discours "enterprise".
- Les opérations de liste chargent tout en mémoire puis filtrent et paginent côté service.
- Il n’y a ni authentification, ni autorisation, ni versionnement API, ni OpenAPI visible.

Risques :
- Le dépôt peut être perçu comme un front plus avancé que son backend.
- Le passage à une base réelle demandera de verrouiller plus clairement les frontières de repository et les scénarios d’intégration.

Actions recommandées :
- Garder les frontières actuelles.
- Prioriser ensuite un passage EF Core avec une base relationnelle légère.
- Ajouter OpenAPI si l’objectif est de renforcer la lisibilité backend pour GitHub.

### `BlazorEnterpriseStarter.Shared`

Constats :
- Les contrats partagés sont simples et propres.
- `BacklogInputRules` centralise utilement les contraintes de saisie.
- `PagedResultDto<T>` et `ApplicationStatusDto` sont suffisamment sobres pour ce périmètre.

Faiblesses :
- Le projet mélange contrats d’API, routes HTTP et règles de saisie, ce qui reste acceptable ici mais pourrait devenir une zone de couplage.
- La centralisation des règles partagées est utile, mais elle peut devenir ambiguë si le domaine serveur se complexifie.

Risques :
- À mesure que le projet grandit, ce projet peut devenir un fourre-tout transverse.

Actions recommandées :
- Conserver le périmètre actuel tant que le backlog reste simple.
- Prévoir, à moyen terme seulement, une séparation plus stricte si plusieurs modules métier apparaissent.

### `BlazorEnterpriseStarter.Components`

Constats :
- Le dépôt contient un vrai début de bibliothèque de composants réutilisables.
- Les API des composants sont cohérentes, nommées proprement et documentées en français.
- Les usages métiers sont bien représentés par `AppCard`, `AppPageHeader`, `AppButton`, `AppModal`, `AppInput`, `AppSelect`, `AppTextarea`.

Faiblesses :
- Les composants de formulaire ne reposent pas sur `InputBase<T>` ni sur l’intégration standard `EditContext`, ce qui limite l’extensibilité Blazor native.
- `AppModal` n’implémente pas de vrai focus trap ni de restauration du focus à la fermeture.
- Aucune batterie de tests de composants n’existe aujourd’hui.

Risques :
- La bibliothèque peut devenir difficile à industrialiser si elle reste en dehors des conventions natives de validation Blazor.
- L’accessibilité reste correcte sur les bases, mais pas encore robuste sur les interactions complexes.

Actions recommandées :
- Prioriser les composants de formulaire et la modale avant d’ajouter de nouveaux composants.
- Introduire des tests bUnit sur les composants structurants.
- Ajouter une courte matrice d’états supportés pour les composants clés.

### `BlazorEnterpriseStarter.DesignSystem`

Constats :
- Le design system est séparé de l’application, ce qui est un vrai point fort.
- Les tokens CSS, la gestion des thèmes et le shell principal donnent une base crédible.
- Le système est suffisamment sobre pour rester maintenable.

Faiblesses :
- La cohérence visuelle est bonne sans être encore irréprochable sur toutes les finitions.
- Il manque une documentation dédiée aux tokens, aux règles d’usage et aux décisions de design.
- Aucun outillage visuel type Storybook, page de guidelines plus avancée ou snapshots visuels.

Risques :
- La qualité perçue de l’UI peut stagner si les composants évoluent sans garde-fous visuels.
- Le design system peut être perçu comme "CSS bien organisé" plutôt que comme un vrai actif de conception.

Actions recommandées :
- Consolider la cohérence visuelle avant d’élargir le périmètre fonctionnel.
- Ajouter une documentation des tokens et conventions dans le dépôt.
- Compléter la page composants avec davantage de cas d’état réel et de contraintes métier.

### `BlazorEnterpriseStarter.Tests`

Constats :
- Les tests couvrent utilement les règles backlog, le service applicatif, l’état client et un DTO partagé.
- Les scénarios testés sont concrets et lisibles.
- La suite est rapide et utile comme garde-fou de régression.

Faiblesses :
- La couverture reste presque exclusivement unitaire.
- Il n’y a pas de tests d’intégration API.
- Il n’y a pas de tests de composants Blazor.
- Il n’y a pas de tests end-to-end.

Risques :
- Les régressions UI, binding, accessibilité et contrat HTTP passeront facilement sous le radar.
- Le dépôt paraît plus fort côté logique métier que côté expérience utilisateur réelle.

Actions recommandées :
- Ajouter d’abord des tests d’intégration API sur le backlog.
- Introduire ensuite bUnit sur `AppButton`, `AppModal`, `AppInput` et `AppPageHeader`.
- En dernier, ajouter un petit parcours end-to-end critique sur backlog.

## Analyse par axe

### 1. Positionnement Blazor réel du projet

Constats :
- Le projet montre une vraie application Blazor, pas seulement une page statique.
- Le choix réel est `Blazor Web App` avec `InteractiveServer`.
- La gestion d’état locale et les composants réutilisables renforcent la crédibilité Blazor.

Risques :
- Le dépôt peut être mal lu si le lecteur pense voir du WebAssembly ou un front complètement autonome.

Priorité :
- Haute.

Actions recommandées :
- Rendre ce positionnement explicite dans le README et dans l’audit.
- Expliquer ce que démontre ce choix et ce qu’il ne cherche pas à démontrer.

### 2. Qualité perçue de l’UI

Constats :
- L’interface est plus soignée qu’un starter standard.
- La hiérarchie visuelle et les composants sont globalement cohérents.
- Le mode clair/sombre et le shell latéral donnent une vraie identité.

Risques :
- Certaines finitions peuvent encore casser la qualité perçue.
- Sans revue visuelle régulière, la cohérence dérivera vite.

Priorité :
- Haute.

Actions recommandées :
- Continuer les passes ciblées de cohérence graphique.
- Stabiliser un jeu de captures d’écran GitHub.
- Ajouter des tests ou snapshots visuels plus tard.

### 3. Couverture front

Constats :
- Le backlog est un bon cas d’usage principal.
- La page composants montre des briques utiles, pas seulement décoratives.

Risques :
- Un seul module métier réel reste un peu court pour un dépôt plus complet.
- Les interactions sont propres, mais encore concentrées sur CRUD + filtres + pagination.

Priorité :
- Haute.

Actions recommandées :
- Ajouter une deuxième profondeur fonctionnelle au backlog avant de créer un autre module complet.
- Exemples possibles : vue détail, historique, commentaires, filtres sauvegardés ou affectation.

### 4. Robustesse de l’architecture actuelle

Constats :
- Le découpage de solution est bon.
- Les responsabilités sont globalement bien séparées.
- Le projet évite le surdesign.

Risques :
- Le backlog UI peut devenir trop massif.
- Les conventions transverses ne sont pas encore beaucoup testées.

Priorité :
- Moyenne à haute.

Actions recommandées :
- Consolider l’orchestration backlog côté App.
- Ajouter quelques tests d’intégration backend.
- Garder la structure générale.

### 5. Pertinence de la persistance actuelle

Constats :
- L’in-memory repository est acceptable pour démarrer et pour garder le dépôt simple.
- Il est bien encapsulé derrière une abstraction.

Risques :
- C’est aujourd’hui la principale faiblesse de crédibilité "enterprise".
- Toute couverture de concurrence, d’historique, de requêtes réelles ou de migrations est absente.

Priorité :
- Très haute.

Actions recommandées :
- Faire évoluer le backlog vers EF Core avec une base simple, idéalement SQLite ou PostgreSQL selon la cible retenue.
- Conserver le seed démonstratif, mais dans une vraie persistence.

### 6. Couverture et qualité des tests

Constats :
- Les tests actuels sont propres et utiles.
- La suite est rapide.

Risques :
- La couverture n’est pas équilibrée entre métier, API et UI.

Priorité :
- Très haute.

Actions recommandées :
- Ajouter des tests d’intégration HTTP.
- Ajouter bUnit sur quelques composants clés.
- Garder les tests unitaires existants comme base.

### 7. Présentation du dépôt sur GitHub

Constats :
- Le README est bien écrit et expose correctement l’intention.
- Le dépôt est lisible et ordonné.

Risques :
- L’absence de workflow CI visible est un manque important.
- L’absence de captures stabilisées laisse une partie de la valeur UI invisible.
- L’absence de plan d’amélioration visible pouvait donner une impression de dépôt "presque fini" sans trajectoire claire.

Priorité :
- Très haute.

Actions recommandées :
- Ajouter une GitHub Action minimale build + tests.
- Ajouter des captures d’écran réelles dans `docs/screenshots`.
- Conserver cet audit et le mettre à jour après les principales itérations.

## Priorités recommandées

### Priorité 1

Objectif :
- renforcer immédiatement la crédibilité GitHub et technique sans changer le socle

Actions :
- ajouter une CI GitHub `build + test`
- documenter explicitement le positionnement `Blazor Web App / InteractiveServer`
- stabiliser les captures d’écran principales
- garder les correctifs UI ciblés sur la cohérence visuelle

### Priorité 2

Objectif :
- renforcer le module backlog comme parcours d’application métier

Actions :
- synchroniser les filtres backlog avec l’URL
- ajouter une vue détail ou un sous-parcours métier visible
- améliorer l’accessibilité et le comportement de la modale
- introduire bUnit sur les composants structurants

### Priorité 3

Objectif :
- faire évoluer le dépôt d’un bon starter vers une base plus complète

Actions :
- remplacer la persistence mémoire par EF Core et une base réelle
- ajouter des tests d’intégration API
- exposer OpenAPI
- enrichir légèrement Aspire avec une ressource utile au périmètre

## Plan de travail pragmatique

Ordre recommandé :
1. Visibilité GitHub : CI, captures, clarification du positionnement Blazor
2. Qualité front : finitions UI, modale, query string backlog, tests bUnit ciblés
3. Crédibilité backend : EF Core, tests d’intégration, OpenAPI
4. Renforcement démonstratif : seconde profondeur fonctionnelle sur le backlog

## Conclusion

Le projet constitue déjà une base .NET / Blazor solide. Son principal mérite est de ne pas surjouer l’enterprise avec une architecture artificiellement complexe. Le travail à mener reste progressif, autour de quatre leviers :
- la persistence réelle
- les tests UI et d’intégration
- la visibilité GitHub
- la profondeur fonctionnelle du cas d’usage backlog

Le socle actuel mérite d’être conservé. L’enjeu n’est pas de le remplacer, mais de le renforcer sur ses limites les plus visibles.
