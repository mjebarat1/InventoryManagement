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
| À compléter | À compléter | À compléter | À compléter |

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
