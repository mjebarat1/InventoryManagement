# API backend

## Objectif

Ce document décrit les conventions API du backend.

Il doit être mis à jour lorsque des endpoints sont ajoutés, modifiés ou supprimés.

---

## Principes

Les controllers sont des adapters entrants.

Ils doivent rester fins.

Ils appellent les cas d’usage de la couche application.

Ils ne contiennent pas de logique métier.

---

## DTO

Les DTO représentent les contrats HTTP.

Ils ne doivent pas être confondus avec les entités du domaine.

Les DTO peuvent être utilisés pour :

- les requêtes de création ;
- les requêtes de modification ;
- les réponses de consultation ;
- les erreurs exposées à l’API.

---

## Validation

La validation simple de forme peut être faite à l’entrée API.

Exemples :

- champ obligatoire manquant ;
- format invalide ;
- payload incorrect.

Les règles métier doivent être validées dans le domaine ou dans les cas d’usage.

---

## Gestion des erreurs

À court terme, certaines erreurs peuvent être retournées en `BadRequest`.

Cette gestion pourra être raffinée ensuite avec des statuts plus précis :

- `400 Bad Request` pour une entrée invalide ;
- `404 Not Found` pour une ressource inexistante ;
- `409 Conflict` pour une référence déjà existante ;
- `422 Unprocessable Entity` pour une règle métier non respectée, si ce choix est retenu.

Tout changement de stratégie d’erreur doit être documenté.

---

## Endpoints à documenter

Ajouter ici les endpoints existants.

### Articles

| Méthode | Route | Description | Statut |
|---|---|---|---|
| POST | `/api/articles/food` | Crée un article alimentaire | Implémenté |
| POST | `/api/articles/non-food` | Crée un article non alimentaire | Implémenté |
| POST | `/api/articles/search` | Recherche paginée, filtrée et triée | Implémenté |
| GET | `/api/articles/{id}` | Consulte une fiche article et ses mouvements | Implémenté |
| POST | `/api/articles/{id}/supplies` | Enregistre un approvisionnement | Implémenté |
| POST | `/api/articles/{id}/sales` | Enregistre une vente | Implémenté |
| POST | `/api/articles/{id}/inventories` | Enregistre un inventaire partiel | Implémenté |
| POST | `/api/articles/{id}/stock-buckets/search` | Recherche lazy des lots de l'article | Implémenté |

### Création d'article

Les deux endpoints attendent `reference`, `name` et `priceExcludingTax`. La création alimentaire attend également `saleModes`. La création non alimentaire refuse les propriétés JSON inconnues afin d'empêcher l'envoi de données alimentaires, d'une DLC ou d'un packaging.

Une création réussie retourne `201 Created` avec `{ "id": "..." }`.

### POST /api/articles/search

```json
{
  "pageNumber": 1,
  "pageSize": 20,
  "sortBy": "Reference",
  "sortDirection": "Asc",
  "type": "Food",
  "reference": null,
  "name": null
}
```

`pageSize` doit être compris entre 1 et 100. Les filtres sont facultatifs. Les tris autorisés sont `Reference`, `Name`, `Type` et `PriceExcludingTax`. La réponse contient `items`, `pageNumber`, `pageSize`, `totalItems` et `totalPages`.

### GET /api/articles/{id}

Retourne les informations principales, les tarifs calculés par mode de vente, les quantités de synthèse, les buckets et les mouvements.

Les quantités sont calculées exclusivement depuis `StockMovementLine.QuantityDelta` :

- `totalStock` : somme des quantités physiques des buckets ;
- `sellableStock` : somme des buckets vendables à la date de consultation ;
- `nonSellableStock` : somme des buckets expirés ou invendables.

Chaque élément de `buckets` contient la DLC ou le packaging, la quantité physique, la quantité vendable et le statut `Empty`, `Sellable`, `Expired` ou `Unsellable`.

Chaque mouvement expose son `quantityDelta` agrégé et ses `lines`. Une ligne contient le bucket impacté, son delta et les quantités avant/après. La DLC et le packaging ne sont plus exposés au niveau du mouvement global.

Retourne `404 Not Found` lorsque l'article n'existe pas.

### POST /api/articles/{id}/supplies

Crée un nouveau bucket, un `SupplyMovement` et une ligne positive dans une seule transaction.

```json
{
  "stockBucketReference": "ref-lot-0000000000042",
  "quantity": 10,
  "expirationDate": "2026-07-15",
  "packagingLevel": null
}
```

- `quantity` est obligatoire et strictement positive ;
- `stockBucketReference` est obligatoire, globalement unique et respecte `ref-lot-` suivi de 13 chiffres ;
- `expirationDate` est obligatoire uniquement pour un article Food ;
- `packagingLevel` est obligatoire uniquement pour un article NonFood ;
- chaque approvisionnement crée un nouveau bucket ;
- une DLC passée est acceptée, mais le bucket est immédiatement non vendable.

Retourne `201 Created` avec `movementId` et `bucketId`, `404 Not Found` si l'article n'existe pas et `400 Bad Request` lorsque les données sont incompatibles avec son type.

### POST /api/articles/{id}/sales

```json
{
  "quantity": 3,
  "saleMode": "TakeAway"
}
```

- `quantity` est obligatoire et strictement positive ;
- `saleMode` est obligatoire pour un article Food et doit faire partie de ses modes autorisés ;
- `saleMode` doit être absent ou `null` pour un article NonFood ;
- Food consomme les buckets vendables en FEFO ;
- NonFood consomme les buckets vendables par date de création et exclut `Unsellable` ;
- une ou plusieurs lignes négatives sont créées atomiquement ;
- un stock vendable insuffisant refuse toute la vente avec `400 Bad Request`.

Retourne `201 Created` avec `movementId` et `soldQuantity`, ou `404 Not Found` si l'article n'existe pas.

### POST /api/articles/{id}/inventories

```json
{
  "comment": "Inventaire mensuel",
  "existingBuckets": [
    { "stockBucketId": "00000000-0000-0000-0000-000000000000", "countedQuantity": 8 }
  ],
  "newBuckets": [
    {
      "reference": "ref-lot-0000000000043",
      "countedQuantity": 3,
      "expirationDate": "2026-08-15",
      "packagingLevel": null
    }
  ]
}
```

L'inventaire peut être partiel. Le backend recalcule les quantités système depuis les lignes de mouvements. Les lots existants ne produisent une ligne que si leur quantité constatée diffère. Les nouveaux lots produisent une ligne positive dans le même `InventoryMovement`. Les buckets inconnus, dupliqués, les références invalides ou existantes et un inventaire sans écart sont refusés avec `400 Bad Request`.

### POST /api/articles/{id}/stock-buckets/search

```json
{
  "referenceDigits": "000000000"
}
```

La recherche est limitée aux buckets de l'article courant, accepte entre 9 et 13 chiffres après le préfixe fixe `ref-lot-`, effectue une recherche par préfixe et retourne au maximum 20 résultats. Les quantités et statuts retournés sont calculés côté backend.

### Stock

| Méthode | Route | Description | Statut |
|---|---|---|---|
| À compléter | À compléter | À compléter | À compléter |

### Disponibilité de l'API

#### GET /api/ping

Vérifie que le processus API est démarré et capable de répondre à une requête HTTP.

Réponse `200 OK` :

```json
{
  "status": "ok"
}
```

Cet endpoint est technique. Il n'appelle aucun cas d'usage métier et n'accède pas à la base de données.

---

## CORS

La policy `ClientCors` autorise les origines déclarées dans `Cors:AllowedOrigins`.

Pour les environnements Development et Docker locaux :

- `http://localhost:3039` ;
- `http://127.0.0.1:3039`.

La policy autorise les méthodes et en-têtes nécessaires, mais n'utilise pas `AllowAnyOrigin` et n'autorise pas implicitement les credentials.

---

## Exemple de documentation d’endpoint

```md
### POST /api/articles

Crée un nouvel article.

#### Requête

...

#### Réponse succès

...

#### Erreurs possibles

...
```
