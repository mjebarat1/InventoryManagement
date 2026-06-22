# Client API

## Objectif

Ce document décrit les conventions d’appel à l’API backend depuis le client Vite.js.

---

## Principe

Les appels API doivent être isolés autant que possible dans une couche dédiée.

Éviter d’éparpiller des appels `fetch` ou `axios` directement dans tous les composants.

---

## Organisation possible

Exemple d’organisation :

```txt
src
  api
    httpClient.ts
    articlesApi.ts
    stockApi.ts
  models
    article.ts
    stockMovement.ts
```

Adapter selon la structure réelle du projet.

---

## Configuration

L’URL de base de l’API doit être configurable.

Exemple avec Vite :

```txt
VITE_API_BASE_URL=https://localhost:7280
```

Ne pas coder en dur une URL de production dans les composants.

---

## Gestion des erreurs

Afficher des messages compréhensibles côté utilisateur.

Ne pas exposer directement des détails techniques inutiles.

Conserver les détails utiles pour le debug en développement si nécessaire.

---

## Synchronisation avec le backend

Quand un endpoint change, mettre à jour :

- le client API ;
- les types côté client ;
- la documentation `docs/backend/api.md` ;
- éventuellement le README si les commandes ou URLs changent.

---

## Règle pour les agents IA

Avant d’ajouter un appel API :

1. vérifier l’endpoint backend ;
2. vérifier le DTO attendu ;
3. ajouter ou réutiliser une fonction dans la couche API client ;
4. éviter de mettre la logique d’appel directement dans le composant ;
5. documenter si l’intégration est significative.
