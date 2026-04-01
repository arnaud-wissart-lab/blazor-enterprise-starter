# BlazorEnterpriseStarter.DesignSystem

## Direction visuelle

Le design system adopte une direction visuelle claire, sobre et premium :

- base lumineuse et chaleureuse, adaptée à une application métier moderne
- accent principal vert grisé pour exprimer la stabilité et la maîtrise
- accent secondaire ambré pour apporter du relief sans effet démonstratif excessif
- contrastes lisibles, surfaces douces et ombres discrètes
- composants pensés pour rester crédibles sur des écrans de supervision, formulaires ou tableaux

Le mode sombre est prévu comme base compatible, sans multiplier les variantes de composants. L’objectif est d’avoir un socle prêt à évoluer, pas un thème parallèle complet dès maintenant.

## Organisation des styles

Les styles globaux du design system sont organisés dans `wwwroot/styles` :

- `index.css` : point d’entrée unique chargé par l’application
- `variables.css` : tokens de référence et échelles de design
- `theme.css` : mapping des tokens vers les rôles visuels du thème
- `base.css` : fondations HTML, typographie, focus, liens et comportements globaux
- `layout.css` : primitives de composition de page et de grilles
- `utilities.css` : utilitaires simples, limités et explicitement nommés

## Conventions de nommage

### Variables CSS

Préfixe systématique : `--bes-`

- `--bes-ref-*` : tokens de référence bruts, indépendants d’un thème
- `--bes-color-*` : rôles sémantiques consommés par les composants
- `--bes-font-*`, `--bes-space-*`, `--bes-radius-*`, `--bes-shadow-*` : échelles globales

### Classes globales

Préfixe systématique : `bes-`

Exemples :

- `.bes-page`
- `.bes-grid`
- `.bes-text-muted`

Les classes globales restent limitées aux primitives de layout et aux utilitaires réellement transverses.

### Styles de composants Razor

Les composants Razor utilisent une convention locale de type BEM léger :

- `.surface-card`
- `.surface-card__header`
- `.surface-card__description`

Cette approche garde les composants lisibles sans créer de dépendance à une nomenclature globale lourde.

## Responsive

Les breakpoints sont déclarés comme tokens documentaires dans `variables.css`.
Ils ne sont pas utilisés directement dans les media queries, car les variables CSS ne sont pas exploitables de manière fiable dans ces contextes aujourd’hui.

Valeurs de référence :

- `xs` : `30rem`
- `sm` : `48rem`
- `md` : `64rem`
- `lg` : `80rem`

## Principes de maintenance

- privilégier les tokens sémantiques plutôt que des couleurs codées en dur
- éviter les utilitaires redondants ou trop spécifiques
- limiter les effets visuels à ce qui améliore réellement la lisibilité et la hiérarchie
- faire porter les décisions globales par les fichiers de thème, pas par les composants
