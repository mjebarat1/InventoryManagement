# Architecture

## Choix architectural

Le projet utilise une architecture Hexagonale avec des principes DDD.

Ce choix permet de séparer :

- le métier ;
- les cas d’usage ;
- les adapters entrants comme les controllers ;
- les adapters sortants comme la base de données ;
- le client frontend.

L’objectif est de montrer une architecture claire, testable et évolutive, même sur un exercice court.

---

## Principes généraux

Les dépendances doivent aller vers le centre.

Le domaine ne dépend de rien de technique.

Les couches externes dépendent des couches internes, jamais l’inverse.

Le backend doit rester structuré autour du métier, et non autour de la base de données ou des controllers.

---

## Structure logique

La structure exacte du dépôt peut évoluer, mais la logique doit rester proche de celle-ci :

```txt
Domain
  Entités
  Value Objects
  Règles métier
  Services de domaine si nécessaires

Application
  Cas d’usage
  Ports entrants
  Ports sortants
  DTO applicatifs si nécessaires

Infrastructure
  Persistance
  Repositories
  Entity Framework / SQLite
  Migrations
  Implémentations techniques

Api
  Controllers
  DTO HTTP
  Mapping API
  Configuration

Client
  Interface utilisateur
  Appels API
  Composants
```

---

## Domaine

Le domaine contient les règles métier.

Il ne doit pas dépendre :

- de ASP.NET ;
- de Entity Framework ;
- de SQLite ;
- de JSON ;
- du client Vite.js ;
- des controllers ;
- des DTO HTTP.

Le domaine peut contenir :

- `Article` ;
- `FoodArticle` ;
- `NonFoodArticle` ;
- `StockMovement` ;
- des value objects comme `Ean13`, `Money`, `VatRate`, `Quantity` ;
- des enums métier comme `PackagingLevel`.

---

## Application

La couche application orchestre les cas d’usage.

Elle peut contenir par exemple :

- `CreateArticleUseCase` ;
- `UpdateArticleUseCase` ;
- `RegisterSupplyUseCase` ;
- `RegisterInventoryUseCase` ;
- `GetArticlesQuery`.

Elle définit les ports nécessaires, par exemple :

- `IArticleRepository` ;
- `IStockMovementRepository`.

La couche application ne doit pas dépendre de l’implémentation technique de la persistance.

---

## Infrastructure

La couche infrastructure implémente les ports.

Elle peut contenir :

- le DbContext ;
- les repositories EF Core ;
- les migrations ;
- les mappings de persistance ;
- les configurations techniques.

La persistance ne doit pas imposer sa structure au domaine.

---

## API

L’API est un adapter entrant.

Les controllers doivent rester fins.

Ils ne doivent pas contenir de logique métier.

Ils doivent :

- recevoir les requêtes ;
- appeler les cas d’usage ;
- retourner des réponses HTTP ;
- convertir les erreurs en statuts HTTP adaptés.

---

## Client

Le client Vite.js consomme l’API.

Il ne doit pas devenir la source de vérité des règles métier.

Il peut faire de la validation de confort, mais le backend doit toujours valider les règles importantes.

---

## DTO et mapping

Les DTO servent aux frontières :

- API vers application ;
- application vers API ;
- client vers API.

Les DTO ne sont pas le modèle de domaine.

Ne pas exposer directement les entités du domaine si le projet utilise déjà des DTO.

---

## Arbitrages

En cas d’arbitrage entre plusieurs solutions valables, ne pas décider silencieusement.

Exemples d’arbitrage :

- endpoint unique ou endpoints séparés ;
- héritage ou composition ;
- entité séparée ou value object ;
- stockage d’un champ calculé ou calcul à la volée ;
- suppression physique ou logique ;
- migration immédiate ou reportée.

Dans ces cas, demander une validation avant d’implémenter.
