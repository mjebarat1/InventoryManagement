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

---

## Règle pour les agents IA

Avant de modifier le client :

1. inspecter la structure existante ;
2. identifier les conventions déjà utilisées ;
3. proposer un plan ;
4. attendre validation ;
5. modifier uniquement les fichiers nécessaires.
