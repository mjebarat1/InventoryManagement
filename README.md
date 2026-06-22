# Stock Management

## Présentation

Ce projet est une application de gestion de stocks réalisée dans le cadre d’un exercice technique.

L’objectif principal est de modéliser un back office permettant de gérer des articles, leurs mouvements de stock, les approvisionnements, les ventes, les inventaires, ainsi que le stock vendable.

Le projet met l’accent sur la qualité de conception, la séparation des responsabilités et la modélisation métier.

---

## Choix techniques

* Backend : C# / ASP.NET Core
* Base de données : SQLite
* ORM : Entity Framework Core
* Architecture : Hexagonale avec une séparation Application / Domain / Infrastructure
* Approche métier : DDD pragmatique avec des Value Objects et des entités métier riches
* Persistance : EF Core avec mapping Fluent API
* Mapping de l’héritage : TPH, afin de garder une persistance simple pour l’exercice

---

## Architecture

Le projet est organisé en plusieurs couches :

```text
src
├── StockManagement.Api
├── StockManagement.Application
├── StockManagement.Domain
└── StockManagement.Infrastructure

tests
└── StockManagement.Tests
```

### StockManagement.Api

Cette couche contient les adapters entrants HTTP :

* Controllers
* DTOs HTTP
* Configuration ASP.NET Core
* Swagger

Elle ne contient pas de logique métier.

### StockManagement.Application

Cette couche contient les cas d’usage de l’application :

* création d’article
* recherche d’articles
* saisie d’approvisionnement
* saisie de vente
* saisie d’inventaire
* consultation de l’historique
* consultation du stock vendable

Elle contient aussi les ports entrants et sortants.

Exemples de ports entrants :

```text
ICreateFoodArticleUseCase
ICreateNonFoodArticleUseCase
IRecordSupplyUseCase
IRecordSaleUseCase
IRecordInventoryUseCase
IGetSellableStockUseCase
```

Exemples de ports sortants :

```text
IArticleRepository
IStockMovementRepository
IClock
```

### StockManagement.Domain

Cette couche contient le modèle métier.

Elle contient notamment :

* Article
* FoodArticle
* NonFoodArticle
* StockMovement
* FoodSupplyMovement
* NonFoodSupplyMovement
* SaleMovement
* FoodInventoryMovement
* NonFoodInventoryMovement
* InventoryMovement
* Ean13Reference
* Money
* VatRate
* Quantity

Les règles métier sont portées par le domaine autant que possible.

### StockManagement.Infrastructure

Cette couche contient les détails techniques :

* DbContext EF Core
* Configurations EF Core
* Repositories EF Core
* Implémentation SQLite
* Implémentation de l’horloge système

---

## Hypothèses métier

### Articles

Un article représente une fiche produit stable.

Il possède :

* une référence unique au format EAN-13
* un nom
* un prix de vente HT
* un type : alimentaire ou non alimentaire

Le prix TTC est calculé à partir du prix HT et du taux de TVA applicable.

### TVA

Les règles de TVA sont considérées comme des règles métier du domaine (Je les ai mis en dur dans le code pour gagner du temps) :

* article alimentaire à emporter : 5,5 %
* article alimentaire sur place : 10 %
* article non alimentaire : 20 %

Les taux sont centralisés dans un Value Object `VatRate`.

### DLC

La DLC n’est pas portée directement par l’article.

Elle est portée par les mouvements d’approvisionnement alimentaire, car un même article alimentaire peut être approvisionné plusieurs fois avec des DLC différentes.

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
La DLC est modélisée avec le type natif DateOnly.

Ce choix est volontaire, car une DLC représente une date métier, sans notion d’heure, de minute ou de fuseau horaire.

Dans cette version, aucune règle métier complexe n’est portée directement par la DLC. La règle principale consiste à comparer la date de DLC avec la date du jour afin de déterminer si un stock alimentaire est encore vendable.

La date du jour n’est pas récupérée directement depuis le domaine. Elle est fournie par une abstraction IClock, implémentée dans l’infrastructure par SystemClock, afin de conserver un domaine testable.

Une approche plus poussée en DDD pourrait consister à créer un Value Object ExpirationDate si des règles métier spécifiques devaient être ajoutées, par exemple :

interdire une DLC antérieure à la date d’approvisionnement ;
centraliser la règle d’expiration ;
ajouter une notion d’alerte avant expiration ;
éviter de manipuler directement des DateOnly dans plusieurs endroits du domaine.

Dans le cadre de cet exercice, DateOnly a été jugé suffisant pour garder le modèle simple, lisible et adapté au périmètre demandé.

### Packaging

Le niveau de packaging n’est pas porté directement par l’article non alimentaire.

Il est porté par les mouvements d’approvisionnement ou d’inventaire non alimentaire, car un même article peut être présent en stock dans plusieurs états :

* neuf
* reconditionné
* invendable

### Stock physique et stock vendable

Le stock physique représente ce qui est réellement présent dans l’entrepôt.

Le stock vendable représente la partie du stock pouvant être vendue.

Pour les articles alimentaires, les unités expirées ne sont pas considérées comme vendables.

Pour les articles non alimentaires, les unités avec un packaging `Unsellable` ne sont pas considérées comme vendables.

### Inventaire

L’inventaire permet d’aligner le système avec la réalité constatée dans l’entrepôt.

L’inventaire n’est pas seulement une quantité globale : il peut être réalisé par catégorie de stock.

### Vente

La vente est modélisée ici comme un mouvement de sortie de stock.

Le projet ne cherche pas à modéliser un système complet de facturation, de paiement ou de ticket de caisse.

---
## Hypothèse sur les approvisionnements, la DLC et le packaging

Dans cette implémentation, un approvisionnement est considéré comme une **entrée de stock** au sens large, et pas uniquement comme une livraison fournisseur composée exclusivement de produits neufs.

Cette hypothèse permet de représenter simplement plusieurs situations réelles :

* une livraison fournisseur classique ;
* une entrée de stock reconditionné ;
* une entrée de stock contenant des produits non vendables ;
* une correction ou une régularisation de stock.

Pour les articles alimentaires, la DLC n’est pas portée directement par l’article, mais par le mouvement d’approvisionnement alimentaire.

Un même article alimentaire peut donc avoir plusieurs approvisionnements avec des DLC différentes.

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

Pour les articles non alimentaires, le niveau de packaging n’est pas porté directement par l’article, mais par le mouvement d’approvisionnement non alimentaire.

Un même article non alimentaire peut donc être présent en stock avec plusieurs états différents :

* neuf ;
* reconditionné ;
* invendable.

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

L’inventaire est volontairement modélisé de manière simple dans cette première version : il permet d’aligner le stock physique global d’un article avec la réalité constatée dans l’entrepôt.

Une version plus avancée pourrait introduire des inventaires spécialisés afin de corriger le stock plus finement par catégorie :

* inventaire alimentaire par DLC ;
* inventaire non alimentaire par niveau de packaging.

Ce choix permet de conserver un modèle simple pour l’exercice tout en gardant une extension possible si le besoin métier devient plus précis.


## Choix de persistance

SQLite a été choisi afin de conserver une persistance réelle tout en simplifiant l’exécution du projet.

La base est stockée dans un fichier local `.db`.

Ce choix permet d’éviter une dépendance à SQL Server Express ou LocalDB, tout en gardant une architecture qui permettrait de remplacer SQLite par SQL Server ou PostgreSQL en modifiant uniquement la couche Infrastructure.

---

## Choix sur le Unit of Work

Aucun `UnitOfWork` explicite n’a été ajouté dans un premier temps.

EF Core `DbContext` joue déjà naturellement ce rôle.

Pour simplifier l’exercice, les repositories encapsulent directement la persistance.

Un `IUnitOfWork` pourrait être introduit plus tard si certains cas d’usage nécessitent de coordonner plusieurs repositories dans une même transaction explicite.

---

## Lancement du projet

### Prérequis

* .NET SDK 8 ou supérieur
* SQLite
* Docker optionnel

### Restaurer les packages

```bash
dotnet restore
```

### Compiler le projet

```bash
dotnet build
```

### Lancer les tests

```bash
dotnet test/README.md
/AGENTS.md
```

### Lancer l’API

```bash
dotnet run --project src/StockManagement.Api
```

L’API est ensuite accessible via Swagger :

```text
https://localhost:xxxx/swagger
```

### Lancer le client

Le client nécessite Node.js 20 ou supérieur.

```bash
cd InventoryManagement.Client
npm install
```

L'URL de l'API est configurée avec `VITE_API_BASE_URL`. La valeur locale actuelle est :

```txt
VITE_API_BASE_URL=https://localhost:7280
```

Le fichier `.env.example` documente cette variable et `.env.development` fournit la configuration de développement local.

```bash
npm run dev
```

Pour compiler le client :

```bash
npm run build
```

Au chargement, le client appelle `GET /api/ping`. Tant que l'API n'est pas joignable, un écran d'attente est affiché et une nouvelle tentative est effectuée toutes les 2 secondes.

La configuration CORS locale autorise le serveur Vite sur `http://localhost:3039` et `http://127.0.0.1:3039`.

Docker publie également l'API en HTTP sur `http://localhost:58995`, mais le client utilise directement l'endpoint HTTPS afin d'éviter une redirection HTTP vers HTTPS pendant les appels CORS.

---

## Base de données et migrations

Le projet utilise SQLite comme base de données locale.

Le schéma de base est géré avec les migrations Entity Framework Core afin de conserver une structure de base versionnée et reproductible.

### Générer une migration

Depuis la racine de la solution :

```bash
dotnet ef migrations add InitialCreate --project ./InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj --startup-project ./InventoryManagement.Api/InventoryManagement.Api.csproj --context StockDbContext --output-dir Persistence/Migrations
```

### Appliquer les migrations

```bash
dotnet ef database update --project ./InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj --startup-project ./InventoryManagement.Api/InventoryManagement.Api.csproj --context StockDbContext
```

### Chemins de base de données

Deux chaînes de connexion sont utilisées selon l’environnement.

En exécution locale, la base est située dans le dossier `data` à la racine du projet :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../data/stock-management.db"
  }
}
```

En exécution Docker, le dossier local `./data` est monté dans le conteneur sur `/app/data` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/app/data/stock-management.db"
  }
}
```

Le fichier SQLite reste donc en dehors de l’image Docker.

L’image Docker contient uniquement l’application. La base de données est montée au runtime via Docker Compose.

### Docker Compose

Le fichier `docker-compose.yml` monte le dossier local `./data` dans le conteneur :

```yaml
volumes:
  - ./data:/app/data
```

Cela permet de conserver la base de données même si le conteneur est supprimé ou recréé.

### Environnements

Le projet utilise plusieurs fichiers de configuration afin de distinguer les contextes d’exécution :

```text
appsettings.json
appsettings.Development.json
appsettings.Docker.json
```

- `Development` utilise un chemin local relatif vers `../data/stock-management.db`.
- `Docker` utilise le chemin Linux `/app/data/stock-management.db`, qui correspond au volume monté par Docker Compose.

### Choix retenu

Dans cette version, les migrations sont lancées explicitement avec la commande `dotnet ef database update`.

Ce choix permet de garder le cycle de création et de mise à jour de la base explicite.

Une évolution possible serait d’appliquer automatiquement les migrations au démarrage de l’application uniquement dans un environnement de développement ou Docker local.

Ce choix n’est pas activé systématiquement afin d’éviter qu’un démarrage applicatif modifie automatiquement la structure de la base dans un environnement plus proche de la production.


```bash
dotnet ef database update --project src/StockManagement.Infrastructure --startup-project src/StockManagement.Api
```

Si la base est créée automatiquement au démarrage, aucune commande supplémentaire n’est nécessaire.

---

## Docker

Si le projet est lancé avec Docker Compose :

```bash
docker compose up --build
```

La base SQLite doit être stockée dans un volume Docker afin de conserver les données lors de la recréation du conteneur.

---

## Usage de l’IA

J’ai utilisé ChatGPT comme assistant de réflexion et d’aide à la conception.

L’IA a été utilisée pour :

* analyser l’énoncé
* comparer plusieurs approches d’architecture
* réfléchir à la modélisation DDD
* discuter les choix entre modèle simple et modèle métier plus riche
* générer des exemples de structure de projet
* proposer des exemples de configuration EF Core

Le code final a été relu, adapté et organisé manuellement.

Pour le scénario d'ajout et de consultation d'article, Codex a été utilisé pour l'analyse, la génération de code backend/frontend, les tests et la documentation. Les contrats API, les limites fonctionnelles et les choix d'affichage doivent être relus et validés humainement avant livraison.

---


## Fonctionnalités réalisées

- création d'articles alimentaires et non alimentaires ;
- recherche d'articles paginée, filtrée et triée ;
- consultation d'une fiche article et de ses mouvements ;
- formulaire client adaptatif et navigation entre la grille et la fiche ;
- interface française et anglaise.

---

## Limites connues


Dans cette version, l’inventaire ajuste le stock physique global d’un article.

Il ne corrige pas encore séparément les quantités par DLC ou par niveau de packaging.

Cette simplification est volontaire afin de limiter la complexité de l’exercice.

Une évolution possible serait d’ajouter :

FoodInventoryMovement, pour inventorier les articles alimentaires par DLC ;
NonFoodInventoryMovement, pour inventorier les articles non alimentaires par niveau de packaging.

La gestion des retours clients, du reconditionnement avancé, de la destruction physique des produits expirés ou invendables peut être ajoutée via de nouveaux types de mouvements.

Le modèle utilise une stratégie TPH pour simplifier la persistance de l’héritage avec EF Core. Cela implique que certaines colonnes spécifiques à certains mouvements peuvent être nulles pour d’autres types de mouvements.

Gestion des erreurs HTTP

Dans cette première version, les erreurs métier levées par le domaine, notamment via les Value Objects ou les règles métier, sont interceptées globalement et traduites en réponse HTTP 400 Bad Request.

Ce choix permet d’éviter de retourner des erreurs 500 Internal Server Error pour des erreurs liées aux données envoyées par l’utilisateur, par exemple une référence EAN-13 invalide.

Cette gestion est volontairement simplifiée dans le cadre de l’exercice.

Une évolution possible serait de raffiner le mapping des erreurs applicatives vers des codes HTTP plus précis, par exemple :

400 Bad Request pour les données invalides ;
404 Not Found lorsqu’une ressource demandée n’existe pas ;
409 Conflict lorsqu’une ressource existe déjà, par exemple une référence article déjà utilisée ;
422 Unprocessable Entity pour certaines erreurs de validation métier plus fines.

Cette évolution permettrait de mieux distinguer les erreurs de validation, les conflits métier et les ressources introuvables.

---
