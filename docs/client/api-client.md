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

Le client utilise la variable obligatoire suivante :

```txt
VITE_API_BASE_URL=https://localhost:7280
```

Les valeurs attendues sont documentées dans `InventoryManagement.Client/.env.example`. Le fichier `.env.development` fournit la valeur correspondant au profil HTTP local actuel du backend.

Le type de la variable est déclaré dans `src/vite-env.d.ts`.

---

## Client HTTP

Le module `InventoryManagement.Client/src/api/http-client.ts` :

- centralise et normalise l'URL de base ;
- construit les URL relatives à l'API ;
- ajoute `Content-Type: application/json` lorsqu'une requête contient un corps ;
- transforme une réponse HTTP en erreur `ApiError` lorsque son statut n'est pas un succès ;
- désérialise les réponses JSON.

Les futurs modules fonctionnels, par exemple `articles-api.ts`, devront réutiliser `apiRequest` au lieu d'appeler `fetch` depuis les composants.

Le module `src/api/articles-api.ts` centralise la création alimentaire et non alimentaire, la recherche paginée et la consultation d'une fiche par identifiant.

Après une création, le client utilise uniquement l'identifiant retourné puis recharge la fiche avec `GET /api/articles/{id}`. La fiche est ainsi identique qu'elle soit ouverte après création ou depuis la grille.

### CORS

Le backend applique la policy `ClientCors`. En développement local, elle autorise le client Vite servi depuis `http://localhost:3039` ou `http://127.0.0.1:3039`.

Les nouvelles origines doivent être ajoutées explicitement à `Cors:AllowedOrigins` dans la configuration backend concernée.

Le client appelle directement l'endpoint HTTPS publié par Docker sur le port `7280`. Le port `58995` reste l'exposition HTTP de l'API et n'est pas utilisé par le client, afin d'éviter une redirection `307` avant le traitement CORS.

---

## Vérification au démarrage

Le module `src/api/ping-api.ts` appelle `GET /api/ping` via le client HTTP commun.

Le composant `ApiAvailabilityGate` :

- effectue un ping dès le chargement du client ;
- bloque l'interface tant que l'API n'est pas joignable ;
- affiche un écran d'attente traduit ;
- attend 2 secondes après un échec avant une nouvelle tentative ;
- annule la requête et le timer lorsqu'il est démonté ;
- arrête définitivement le polling après le premier succès.

Ce mécanisme vérifie la disponibilité du processus HTTP, pas celle de la base de données.

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
