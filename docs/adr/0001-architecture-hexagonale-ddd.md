# ADR 0001 — Architecture Hexagonale / DDD

## Statut

Acceptée.

## Contexte

Ce projet est un exercice technique de gestion de stocks.

L’objectif n’est pas uniquement de livrer des fonctionnalités, mais aussi de démontrer :

- une architecture claire ;
- une séparation nette des responsabilités ;
- une modélisation métier défendable ;
- un projet maintenable, testable et compilable ;
- des choix techniques explicités dans la documentation.

Le sujet porte sur un back-office de gestion de stocks avec :

- des articles alimentaires ;
- des articles non alimentaires ;
- des approvisionnements ;
- des ventes ;
- des inventaires ;
- le calcul du stock physique ;
- le calcul du stock vendable ;
- la gestion de la TVA ;
- la gestion des DLC ;
- la gestion du packaging.

L’énoncé étant volontairement ouvert, plusieurs hypothèses métier ont été posées et documentées.

---

## Décision

Le projet adopte une architecture **Hexagonale** avec une approche **DDD pragmatique**.

Le code est organisé autour des couches suivantes :

```text
InventoryManagement.Api
InventoryManagement.Application
InventoryManagement.Domain
InventoryManagement.Infrastructure
InventoryManagement.Test
```

La règle principale est que le domaine reste indépendant de toute technologie.

---

## Vue d’ensemble des dépendances

```text
InventoryManagement.Api
        |
        v
InventoryManagement.Application
        |
        v
InventoryManagement.Domain


InventoryManagement.Infrastructure
        |
        v
InventoryManagement.Application
        |
        v
InventoryManagement.Domain
```

Les dépendances autorisées sont :

```text
Api            -> Application
Api            -> Infrastructure
Application    -> Domain
Infrastructure -> Application
Infrastructure -> Domain
Domain         -> aucune couche projet
```

Le domaine ne dépend pas de :

- ASP.NET Core ;
- Entity Framework Core ;
- SQLite ;
- HTTP ;
- JSON ;
- Docker ;
- la couche API ;
- la couche Infrastructure ;
- le client frontend.

---

## Rôle des couches

### Domain

La couche `Domain` contient les concepts métier et les règles importantes.

Elle contient notamment :

- les entités ;
- les Value Objects ;
- les règles métier ;
- les exceptions métier ;
- les services de domaine purs si nécessaire.

Le domaine ne contient pas de configuration de dependency injection.

Il ne connaît pas les repositories EF Core, les controllers, les DTO HTTP ou la base de données.

### Application

La couche `Application` contient les cas d’usage.

Elle orchestre les opérations métier sans connaître les détails techniques.

Elle contient :

- les ports entrants ;
- les ports sortants ;
- les commands ;
- les results ;
- les implémentations de use cases.

Exemples :

```text
ICreateFoodArticleUseCase
ICreateNonFoodArticleUseCase
IRecordSupplyUseCase
IRecordSaleUseCase
IRecordInventoryUseCase
```

Les commands et results de use cases sont placés dans `Application`, à côté des use cases concernés.

Exemple :

```text
Application
└── Articles
    └── CreateFoodArticle
        ├── ICreateFoodArticleUseCase.cs
        ├── CreateFoodArticleCommand.cs
        ├── CreateFoodArticleUseCase.cs
        └── CreateArticleResult.cs
```

### Infrastructure

La couche `Infrastructure` contient les détails techniques.

Elle implémente les ports sortants définis côté application.

Elle contient notamment :

- `StockDbContext` ;
- les configurations EF Core ;
- les repositories EF Core ;
- l’implémentation de `IClock` ;
- la configuration SQLite ;
- les migrations EF Core.

### Api

La couche `Api` contient les adapters entrants HTTP.

Elle contient :

- les controllers ;
- les DTO de requête HTTP ;
- les DTO de réponse HTTP si nécessaire ;
- la configuration ASP.NET Core ;
- Swagger ;
- les middlewares HTTP.

Les controllers doivent rester fins.

Ils doivent principalement :

- recevoir la requête HTTP ;
- transformer le DTO HTTP en command applicative ;
- appeler le bon use case ;
- transformer le résultat en réponse HTTP.

---

## Ports et adapters

Le projet applique la logique hexagonale suivante :

```text
Controller HTTP
    -> port entrant / use case
        -> domaine
        -> port sortant
            -> adapter infrastructure
                -> SQLite / EF Core
```

### Ports entrants

Les ports entrants représentent les cas d’usage exposés par l’application.

Exemple :

```csharp
public interface ICreateFoodArticleUseCase
    : IUseCase<CreateFoodArticleCommand, CreateArticleResult>
{
}
```

L’interface générique commune est :

```csharp
public interface IUseCase<in TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
```

### Ports sortants

Les ports sortants représentent ce dont l’application a besoin à l’extérieur.

Exemples :

```text
IArticleRepository
IStockMovementRepository
IClock
```

Les repositories sont déclarés côté application et implémentés côté infrastructure.

---

## Dependency Injection

La dependency injection est configurée dans les couches qui en ont besoin.

### Application

La couche `Application` expose une méthode :

```csharp
services.AddApplication();
```

Elle enregistre les use cases :

```csharp
services.AddScoped<ICreateFoodArticleUseCase, CreateFoodArticleUseCase>();
```

Les use cases sont enregistrés en `Scoped`, car ils dépendent généralement de repositories et donc indirectement d’un `DbContext`, lui aussi `Scoped`.

### Infrastructure

La couche `Infrastructure` expose une méthode :

```csharp
services.AddInfrastructure(configuration);
```

Elle enregistre :

```csharp
services.AddDbContext<StockDbContext>(...);
services.AddScoped<IArticleRepository, ArticleRepository>();
services.AddScoped<IStockMovementRepository, StockMovementRepository>();
services.AddSingleton<IClock, SystemClock>();
```

`SystemClock` est enregistré en `Singleton`, car il ne porte pas d’état et ne dépend pas d’un service scoped.

### Domain

La couche `Domain` ne contient pas de `DependencyInjection.cs`.

Le domaine ne doit pas savoir comment il est instancié.

---

## Modélisation des articles

Un `Article` représente une fiche produit stable.

Il possède :

- un identifiant technique ;
- une référence EAN-13 unique ;
- un nom ;
- un prix HT ;
- un type métier.

Deux types d’articles existent :

```text
Article
├── FoodArticle
└── NonFoodArticle
```

La référence EAN-13 est modélisée par un Value Object.

Le prix est modélisé par un Value Object `Money`.

La TVA est modélisée par un Value Object `VatRate`.

---

## Articles alimentaires et modes de vente

L’énoncé indique que certains articles alimentaires peuvent être vendus :

- à emporter ;
- non à emporter ;
- ou les deux.

Une première option aurait été de créer un enum avec une valeur `Both`.

Cette option a été écartée.

### Décision

Un article alimentaire porte une liste de modes de vente autorisés.

```text
FoodArticle
└── SaleModes
    ├── TakeAway
    └── OnSite
```

L’enum `SaleMode` ne contient donc pas `Both`.

Exemple :

```csharp
public enum SaleMode
{
    TakeAway = 1,
    OnSite = 2
}
```

Un article supportant les deux modes aura simplement :

```text
SaleModes = [TakeAway, OnSite]
```

### Raison

La valeur `Both` ne serait pas extensible.

Si un troisième mode est ajouté plus tard, par exemple :

- Delivery ;
- Drive ;
- ClickAndCollect ;

alors `Both` ne veut plus rien dire.

Un article pourrait accepter deux modes parmi trois, mais pas forcément tous les modes.

La liste de modes autorisés est donc plus évolutive.

---

## TVA des articles alimentaires

Un article alimentaire peut avoir plusieurs taux de TVA possibles selon le mode de vente.

Exemple :

```text
Sandwich
Prix HT : 10 €

TakeAway -> TVA 5,5 %
OnSite   -> TVA 10 %
```

Donc un `FoodArticle` n’a pas un prix TTC unique.

La méthode suivante a été supprimée de la classe abstraite `Article` :

```csharp
public Money GetPriceIncludingTax()
{
    return PriceExcludingTax.AddVat(GetVatRate());
}
```

Elle supposait qu’un article possède un seul taux de TVA, ce qui est faux pour un article alimentaire vendable selon plusieurs modes.

### Décision

Pour un article alimentaire, le prix TTC doit être calculé avec un mode de vente donné.

Exemple :

```csharp
foodArticle.GetVatRateFor(SaleMode.TakeAway);
foodArticle.GetPriceIncludingTaxFor(SaleMode.TakeAway);
```

Le mode réellement utilisé est porté par la vente, pas uniquement par l’article.

---

## Articles non alimentaires

Un article non alimentaire possède une TVA fixe à 20 %.

Il n’a pas de mode de vente alimentaire.

Le packaging n’est pas porté directement par l’article.

---

## Packaging

L’énoncé indique que les articles non alimentaires peuvent avoir un niveau de packaging :

- neuf ;
- reconditionné ;
- invendable.

Une première option consistait à porter `PackagingLevel` directement sur `NonFoodArticle`.

Cette option a été écartée.

### Décision

Le packaging est porté par le mouvement d’approvisionnement non alimentaire.

```text
SupplyMovement associé à un NonFoodStockBucket
└── PackagingLevel
```

### Raison

Un même article non alimentaire peut être présent en stock dans plusieurs états.

Exemple :

```text
Article : Casque audio

Approvisionnement 1 :
- 10 unités
- Packaging : New

Approvisionnement 2 :
- 5 unités
- Packaging : Refurbished

Approvisionnement 3 :
- 2 unités
- Packaging : Unsellable
```

Mettre `PackagingLevel` sur l’article reviendrait à dire que toutes les unités de cet article ont toujours le même état, ce qui est trop global.

---

## DLC

La DLC n’est pas portée directement par `FoodArticle`.

Elle est portée par le mouvement d’approvisionnement alimentaire.

```text
SupplyMovement associé à un FoodStockBucket
└── ExpirationDate
```

### Raison

Un même article alimentaire peut être approvisionné plusieurs fois avec des DLC différentes.

Exemple :

```text
Article : Yaourt nature

Approvisionnement 1 :
- 40 unités
- DLC : 25/06/2026

Approvisionnement 2 :
- 60 unités
- DLC : 10/07/2026
```

La DLC décrit donc un lot ou une entrée réelle de stock, pas la fiche produit globale.

---

## Value Objects

Les concepts suivants sont modélisés comme Value Objects :

```text
Ean13Reference
Money
Quantity
VatRate
```

### Ean13Reference

Protège l’invariant de référence article.

Une référence invalide ne doit pas entrer dans le domaine.

### Money

Évite de manipuler directement des `decimal` pour des montants métier.

### Quantity

Évite de manipuler directement des `int` pour des quantités métier.

Elle protège notamment les règles sur les quantités positives ou nulles.

### VatRate

Centralise les taux de TVA connus du domaine.

Exemples :

```text
FoodTakeAway -> 5,5 %
FoodOnSite   -> 10 %
NonFood      -> 20 %
```

---

## Persistance EF Core

L’infrastructure utilise EF Core avec SQLite.

Le mapping est fait avec Fluent API.

Le domaine ne contient pas d’attributs EF Core.

### Héritage

Les hiérarchies `Article` et `StockMovement` sont persistées en TPH (`Table Per Hierarchy`).
Cette stratégie a été retenue pour conserver une persistance simple dans le cadre de l’exercice.

Exemple pour les mouvements :

```text
StockMovements
- Id
- ArticleId
- MovementType
- Quantity
- CreatedAt
- ExpirationDate
- PackagingLevel
```

## SaleModes et persistance

Comme `FoodArticle` possède plusieurs modes de vente autorisés, la persistance ne se fait pas avec une simple colonne `SaleMode`.

Une table dédiée est utilisée :

```text
FoodArticleSaleModes
- ArticleId
- SaleMode
```

Cela évite une valeur artificielle `Both` et rend le modèle plus extensible.

---

## Base de données et migrations

Le projet utilise SQLite avec migrations EF Core.

Les migrations sont lancées explicitement.

### Générer une migration

```bash
dotnet ef migrations add InitialCreate --project ./InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj --startup-project ./InventoryManagement.Api/InventoryManagement.Api.csproj --context StockDbContext --output-dir Persistence/Migrations
```

### Appliquer les migrations

```bash
dotnet ef database update --project ./InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj --startup-project ./InventoryManagement.Api/InventoryManagement.Api.csproj --context StockDbContext
```

### Chemins de base

En local :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../data/stock-management.db"
  }
}
```

En Docker :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/app/data/stock-management.db"
  }
}
```

Le fichier SQLite reste en dehors de l’image Docker.

Il est monté via Docker Compose :

```yaml
volumes:
  - ./data:/app/data
```

---

## Docker

Le conteneur applicatif ne doit pas contenir la base SQLite dans l’image.

La base reste dans le dossier local `./data`.

Docker Compose monte ce dossier dans le conteneur.

```text
PC      : ./data/stock-management.db
Docker  : /app/data/stock-management.db
```

L’image contient uniquement l’application.

## Controllers et API

Les controllers sont des adapters entrants.

Ils doivent rester fins.

Pour la création d’article, deux endpoints distincts sont retenus :

```http
POST /api/articles/food
POST /api/articles/non-food
```

### Raison

Les données attendues pour un article alimentaire et un article non alimentaire ne sont pas identiques.

Un seul endpoint `POST /api/articles` impliquerait un DTO avec des champs conditionnels.

Cela rendrait le contrat API moins clair.

Une interface utilisateur peut néanmoins proposer un seul écran `Ajouter un article`.

Le client appelle ensuite l’endpoint adapté selon le type choisi par l’utilisateur.

---

## Gestion des erreurs HTTP

Dans cette première version, les erreurs métier levées par le domaine ou les Value Objects sont interceptées globalement et traduites en `400 Bad Request`.

Cela évite de retourner une erreur `500 Internal Server Error` lorsqu’une donnée envoyée par l’utilisateur est invalide.

Exemple :

```text
Référence EAN-13 invalide -> 400 Bad Request
```

Cette gestion est volontairement simplifiée.

## Tests

Les tests doivent prioriser :

1. le domaine ;
2. les use cases applicatifs ;
3. les adapters si nécessaire ;
4. les controllers uniquement si cela apporte une vraie valeur.

Les tests unitaires ne doivent pas être lancés automatiquement par l’agent IA sans demande explicite.

---

## Conséquences positives

Cette architecture permet :

- de garder un domaine indépendant ;
- de rendre les règles métier testables ;
- de remplacer SQLite sans modifier le domaine ;
- de garder des controllers simples ;
- de limiter la dépendance à EF Core à l’infrastructure ;
- de documenter clairement les arbitrages métier ;
- de faire évoluer le modèle sans casser immédiatement les contrats existants.

---

## Limites connues

Les limites acceptées dans cette première version sont :

- la gestion des erreurs HTTP est encore simplifiée ;
- le calcul du stock vendable peut être raffiné avec une vraie gestion de lots ;
- les retours clients, changements d’état et mises au rebut ne sont pas encore modélisés comme mouvements dédiés.

---
