# Positionnement Blazor du projet

## Mode de rendu utilisé

Le projet utilise une `Blazor Web App` avec rendu interactif côté serveur.

Concrètement :

- les composants Razor interactifs sont activés via `AddInteractiveServerComponents()`
- l’application expose ce mode via `AddInteractiveServerRenderMode()`
- le routeur principal est rendu avec `@rendermode="InteractiveServer"`
- la modale de reconnexion suit le même mode de rendu

Ce choix implique :

- un HTML initial produit côté serveur
- une interactivité portée ensuite par le circuit Blazor côté serveur
- un front Razor interactif sans client JavaScript applicatif dédié

## Ce que couvre le dépôt côté Blazor

Le dépôt se concentre sur un usage lisible et maintenable de Blazor dans une application métier.

Le périmètre actuel met en avant :

- une composition Razor claire entre layout, pages, composants applicatifs et design system
- une séparation explicite entre composants visuels, état local et appels HTTP
- des interactions utilisateur gérées directement côté composants Razor
- une bibliothèque de composants orientée usages réels
- un parcours backlog avec recherche, filtres, pagination, modales et feedback utilisateur

## Ce que le dépôt ne cherche pas à couvrir

Le projet n’a pas pour objectif principal de couvrir :

- une application Blazor WebAssembly autonome ou offline-first
- une architecture front massivement orientée client
- un store global complexe de type Redux
- une couverture exhaustive des modes de rendu hybrides Blazor
- l’ensemble des primitives avancées de l’écosystème Blazor

## Pourquoi ce choix ici

Le mode `InteractiveServer` reste cohérent avec le périmètre du dépôt :

- il limite le bruit technique inutile
- il garde le front très lisible
- il met l’accent sur Razor, les composants et les flux métier
- il reste adapté à une application interne ou un back-office

## Compromis actuels

Les compromis retenus sont les suivants :

- priorité donnée à la lisibilité plutôt qu’à la variété des modes de rendu
- état local simple et ciblé sur le module backlog
- périmètre fonctionnel volontairement concentré sur un seul parcours métier

## Suite possible

Les évolutions les plus naturelles seraient :

- expliciter davantage ce choix dans le README et dans l’interface
- enrichir la page composants avec une lecture plus architecturale du front
- compléter les composants structurants avec des tests bUnit ciblés
