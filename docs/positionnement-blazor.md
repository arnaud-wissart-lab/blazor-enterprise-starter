# Positionnement Blazor du projet

## Mode de rendu utilisé aujourd’hui

Le projet utilise une `Blazor Web App` avec rendu interactif côté serveur.

Concrètement :
- les composants Razor interactifs sont activés via `AddInteractiveServerComponents()`
- l’application expose le mode interactif via `AddInteractiveServerRenderMode()`
- le routeur principal est rendu avec `@rendermode="InteractiveServer"`
- la modale de reconnexion est elle aussi rendue en `InteractiveServer`

Ce choix signifie que :
- le HTML initial est produit côté serveur
- l’interactivité des composants est ensuite assurée par le circuit Blazor côté serveur
- l’état de session et les événements d’interface restent pilotés par Blazor, sans construire un client JavaScript riche sur mesure

## Ce que le projet démontre réellement du point de vue Blazor

Le dépôt ne cherche pas à démontrer "tout Blazor". Il cherche à démontrer un usage crédible, lisible et maintenable de Blazor dans une application métier.

Aujourd’hui, le projet met en avant :
- une composition Razor claire entre layout, pages, composants applicatifs et design system
- une séparation explicite entre composants visuels, état local et appels HTTP
- des interactions utilisateur gérées proprement côté composants Razor
- une bibliothèque de composants réutilisables pensée pour des usages métier réels
- un cas d’usage front démonstratif avec recherche, filtres, pagination, modales et feedback utilisateur

Autrement dit, le projet montre surtout :
- la capacité à structurer un front Blazor sérieux
- la maîtrise de composants Razor réutilisables
- la gestion d’état locale sans sur-ingénierie
- l’articulation front Blazor / API ASP.NET Core dans une solution multi-projets

## Ce que le projet ne cherche pas à démontrer

Le projet n’a pas pour objectif principal de démontrer :
- une application Blazor WebAssembly autonome ou offline-first
- une architecture front massivement orientée client
- un store global complexe de type Redux
- une démonstration exhaustive des modes de rendu hybrides Blazor
- une couverture complète de toutes les primitives avancées de l’écosystème Blazor

Ce n’est pas une faiblesse en soi. C’est un cadrage volontaire pour préserver la lisibilité du dépôt.

## Pourquoi ce choix est pertinent ici

Le mode `InteractiveServer` est cohérent avec la vocation vitrine du projet :
- il réduit le bruit technique inutile pour un dépôt de démonstration
- il permet de garder une architecture front très lisible
- il met l’accent sur Razor, les composants, la composition d’interface et les flux métier
- il reste parfaitement crédible pour une application interne ou un back-office

Dans ce contexte, changer de mode de rendu uniquement pour “cocher plus de cases Blazor” serait contre-productif.

## Compromis assumés

Les compromis actuels sont les suivants :
- la démonstration privilégie la lisibilité à la variété des modes de rendu
- l’état local reste simple et ciblé sur le module backlog
- la persistance métier est encore mémoire, ce qui limite le discours "enterprise" mais ne remet pas en cause la démonstration Blazor elle-même

Le bon levier d’amélioration n’est donc pas une refonte du mode de rendu. Le bon levier est de mieux rendre visibles les choix Blazor déjà présents et d’étoffer progressivement les parcours front les plus démonstratifs.

## Suite recommandée

Pour renforcer la valeur démonstrative côté Blazor sans casser le socle :
- assumer explicitement dans la documentation que le projet est une `Blazor Web App` en `InteractiveServer`
- rendre ce choix visible dans l’interface elle-même
- compléter la page composants avec une lecture architecturale front
- ajouter ensuite des tests de composants bUnit sur les briques clés

Ce plan améliore la perception recruteur sans transformer le projet en vitrine artificiellement complexe.
