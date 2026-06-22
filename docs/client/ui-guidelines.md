# Guidelines UI client

## Objectif

Ce document décrit les règles générales pour le client basé sur Vite.js.

---

## Principes

Le client doit rester simple et lisible.

Il sert à démontrer les fonctionnalités principales du back-office de gestion de stocks.

Ne pas complexifier inutilement l’interface.

Ne pas remplacer le template Vite.js sans validation.

---

## Organisation recommandée

Adapter cette organisation à la structure réelle du projet :

```txt
client
  src
    components
    pages
    api
    models
    hooks
    App.*
```

---

## Règles UI

Privilégier :

- des composants simples ;
- des formulaires lisibles ;
- des messages d’erreur compréhensibles ;
- une séparation entre composants UI et appels API ;
- une navigation simple.

---

## Règles métier

Le client peut faire des validations de confort :

- champ obligatoire ;
- format visuel ;
- valeur numérique positive.

Mais les règles métier importantes doivent rester validées côté backend.

Le backend est la source de vérité.

---

## Pages possibles

Pages envisageables :

- liste des articles ;
- création d’article ;
- détail d’article ;
- saisie d’approvisionnement ;
- saisie d’inventaire ;
- historique des mouvements.

Tout n’est pas obligatoire pour l’exercice.

## Parcours Articles implémenté

La page Articles appelle la recherche avec une pagination configurable, limitée à 100 lignes, des filtres sur la référence, le nom et le type, ainsi qu'un tri contrôlé.

Le formulaire de création est unique et adapte les modes de vente au type sélectionné. La fiche article est une route autonome utilisée après création et depuis la grille. Les actions d'approvisionnement, vente et inventaire sont visibles mais désactivées jusqu'à l'implémentation de leurs scénarios.

Les stocks vendable et non vendable sont affichés comme indisponibles tant que leur calcul backend n'est pas implémenté.

## Identité visuelle

Le client conserve le thème Minimal existant avec une palette bleu indigo, des accents turquoise et des couleurs sémantiques. Les bandeaux de pages utilisent un dégradé, la navigation active reprend la couleur primaire et les cards de synthèse distinguent visuellement stock, prix et TVA. Les types d'article et états sont présentés avec des badges colorés, sans modifier les règles métier.

---

## Règle pour les agents IA

Avant de modifier le client :

1. inspecter la structure existante ;
2. identifier les conventions déjà utilisées ;
3. proposer un plan ;
4. attendre validation ;
5. modifier uniquement les fichiers nécessaires.

---

## Internationalisation

Le client est multilingue. Le français est la langue par défaut et l'anglais est également pris en charge.

Toute chaîne visible dans l'interface doit :

- être définie dans `InventoryManagement.Client/src/locales/translations/fr.json` et `en.json` ;
- être affichée avec le hook `useTranslate` ;
- utiliser une clé organisée par domaine fonctionnel, par exemple `navigation.articles` ou `articles.reference`.

Ne pas coder de texte métier directement dans un composant React. Lorsqu'une nouvelle clé est ajoutée, fournir sa traduction dans toutes les langues supportées.

Le provider i18n est initialisé à la racine du client. Le sélecteur de langue existant doit rester connecté à ce provider et la préférence utilisateur doit être conservée dans le navigateur.

Les noms de langues, libellés d'accessibilité, titres de pages, états vides, erreurs et infobulles font partie des chaînes à traduire.

---

## Disponibilité de l'API

L'application est enveloppée par `ApiAvailabilityGate`. Au démarrage, l'interface métier ne doit être affichée qu'après une réponse réussie de `GET /api/ping`.

Le message d'attente doit rester traduit. Les composants métier ne doivent pas implémenter leur propre boucle de ping.
