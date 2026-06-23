# Inventory Management

## Présentation

Ce projet est une application de gestion de stocks réalisée dans le cadre d’un exercice technique.

L’objectif est de modéliser les bases d’un back-office permettant de gérer des articles, leurs mouvements de stock, les approvisionnements, les ventes, les inventaires, ainsi que la notion de stock vendable.

Le projet met l’accent sur :

- la qualité de conception ;
- la séparation des responsabilités ;
- une architecture hexagonale pragmatique ;
- une modélisation métier inspirée du DDD ;
- une persistance réelle avec Entity Framework Core et SQLite ;
- une interface client permettant d’interagir avec l’API.

---

## Choix techniques

- Backend : C# / ASP.NET Core
- Frontend : React / Vite
- Base de données : SQLite
- ORM : Entity Framework Core
- Architecture : hexagonale avec séparation Application / Domain / Infrastructure
- Approche métier : DDD pragmatique avec entités métier et Value Objects
- Persistance : EF Core avec mapping Fluent API
- Mapping de l’héritage : TPH afin de garder une persistance simple pour l’exercice
- Documentation API : Swagger

---

## Architecture

Le projet est organisé en plusieurs couches :

```text
InventoryManagement.Api
InventoryManagement.Application
InventoryManagement.Domain
InventoryManagement.Infrastructure
InventoryManagement.Client
```

### InventoryManagement.Api

Cette couche contient les adapters entrants HTTP :

- controllers ;
- DTOs HTTP ;
- configuration ASP.NET Core ;
- configuration Swagger ;
- configuration CORS ;
- gestion globale des erreurs.

Elle ne contient pas de logique métier.

### InventoryManagement.Application

Cette couche contient les cas d’usage de l’application.

Elle orchestre les scénarios métier en s’appuyant sur le domaine et sur les ports sortants.

Exemples de responsabilités :

- création d’article ;
- recherche d’articles ;
- consultation d’une fiche article ;
- consultation des mouvements ;
- saisie d’approvisionnement ;
- saisie de vente ;
- saisie d’inventaire ;
- calcul du stock physique ;
- calcul du stock vendable.

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

### InventoryManagement.Domain

Cette couche contient le modèle métier.

Elle contient notamment :

- Article
- FoodArticle
- NonFoodArticle
- StockBucket
- StockMovement
- StockMovementLine
- FoodSupplyMovement
- NonFoodSupplyMovement
- SaleMovement
- InventoryMovement
- Ean13Reference
- Money
- VatRate
- Quantity

Les règles métier sont portées par le domaine autant que possible.

### InventoryManagement.Infrastructure

Cette couche contient les détails techniques :

- DbContext EF Core ;
- configurations EF Core ;
- migrations EF Core ;
- repositories EF Core ;
- implémentation SQLite ;
- implémentation de l’horloge système.

### InventoryManagement.Client

Cette couche contient l’interface utilisateur React.

Elle permet notamment :

- d’afficher la liste des articles ;
- de rechercher et consulter les articles ;
- de créer des articles alimentaires ou non alimentaires ;
- de naviguer entre la grille et la fiche article ;
- d’appeler l’API via une URL configurable.

---

## Modélisation métier

### Articles

Un article représente une fiche produit stable.

Il possède :

- une référence unique au format EAN-13 ;
- un nom ;
- un prix de vente HT ;
- un type : alimentaire ou non alimentaire.

Le prix TTC est calculé à partir du prix HT et du taux de TVA applicable.

La fiche article ne porte pas directement les quantités en stock.

Les quantités sont calculées à partir des mouvements de stock et de leurs lignes.

---

## TVA

Les règles de TVA sont considérées comme des règles métier du domaine.

Pour simplifier l’exercice, les taux sont définis dans le code.

Les règles retenues sont :

- article alimentaire à emporter : 5,5 % ;
- article alimentaire sur place : 10 % ;
- article non alimentaire : 20 %.

Les taux sont centralisés dans un Value Object `VatRate`.

---

## Modes de vente des articles alimentaires

Un article alimentaire peut être vendu selon un ou plusieurs modes de vente :

- à emporter ;
- sur place.

Ces modes de vente sont portés par l’article alimentaire, car ils représentent une caractéristique stable de la fiche produit.

Exemple :

```text
Article : Sandwich poulet

Modes de vente autorisés :
- à emporter
- sur place
```

Ces modes permettent ensuite de déterminer le taux de TVA applicable lors d’une vente.

---

## DLC

La DLC n’est pas portée directement par l’article alimentaire.

Elle est portée par le `StockBucket`.

Ce choix permet à un même article alimentaire d’avoir plusieurs lots avec des DLC différentes.

Exemple :

```text
Article : Yaourt nature

Bucket 1 :
- DLC : 25/06/2026

Bucket 2 :
- DLC : 10/07/2026
```

Un approvisionnement alimentaire crée :

```text
FoodSupplyMovement
- représente l’événement métier d’approvisionnement

StockBucket
- porte la DLC du lot alimentaire

StockMovementLine
- porte la quantité entrée dans ce bucket
```

La DLC est modélisée avec le type natif `DateOnly`.

Ce choix est volontaire, car une DLC représente une date métier, sans notion d’heure, de minute ou de fuseau horaire.

Dans cette version, aucune règle métier complexe n’est portée directement par la DLC.

La règle principale consiste à comparer la date de DLC avec la date du jour afin de déterminer si un stock alimentaire est encore vendable.

La date du jour n’est pas récupérée directement depuis le domaine.

Elle est fournie par une abstraction `IClock`, implémentée dans l’infrastructure par `SystemClock`, afin de conserver un domaine testable.

Une approche plus poussée en DDD pourrait consister à créer un Value Object `ExpirationDate` si des règles métier spécifiques devaient être ajoutées, par exemple :

- interdire une DLC antérieure à la date d’approvisionnement ;
- centraliser la règle d’expiration ;
- ajouter une notion d’alerte avant expiration ;
- éviter de manipuler directement des `DateOnly` dans plusieurs endroits du domaine.

Dans le cadre de cet exercice, `DateOnly` a été jugé suffisant pour garder le modèle simple, lisible et adapté au périmètre demandé.

---

## Packaging

Le niveau de packaging n’est pas porté directement par l’article non alimentaire.

Il est porté par le `StockBucket`.

Ce choix permet à un même article non alimentaire d’être présent en stock avec plusieurs états différents :

- neuf ;
- reconditionné ;
- invendable.

Exemple :

```text
Article : Casque audio

Bucket 1 :
- Packaging : New

Bucket 2 :
- Packaging : Refurbished

Bucket 3 :
- Packaging : Unsellable
```

Un approvisionnement non alimentaire crée :

```text
NonFoodSupplyMovement
- représente l’événement métier d’approvisionnement

StockBucket
- porte le niveau de packaging

StockMovementLine
- porte la quantité entrée dans ce bucket
```

---

## Modélisation du stock par buckets et lignes de mouvement

Dans cette version, le stock n’est pas calculé à partir d’une quantité globale portée par un mouvement.

Le modèle distingue trois notions :

```text
StockMovement
- représente un événement métier : approvisionnement, vente ou inventaire

StockBucket
- représente une catégorie de stock ou un lot
- porte les caractéristiques permettant de déterminer si le stock est vendable

StockMovementLine
- représente l’impact chiffré d’un mouvement sur un bucket
- porte le delta de quantité
```

Un mouvement de stock peut donc contenir une ou plusieurs lignes.

Exemple de vente consommant plusieurs buckets :

```text
Vente de 12 unités

Bucket A :
- quantité avant : 10
- quantité consommée : 10
- delta : -10
- quantité après : 0

Bucket B :
- quantité avant : 20
- quantité consommée : 2
- delta : -2
- quantité après : 18
```

Le stock physique total d’un article est calculé en additionnant les `QuantityDelta` de toutes les lignes de mouvement liées aux buckets de cet article.

```text
Stock physique = somme des QuantityDelta
```

Ce choix permet :

- de conserver un historique détaillé ;
- de recalculer le stock à partir des mouvements ;
- de gérer les ventes qui consomment plusieurs lots ;
- de gérer les inventaires par bucket ;
- de distinguer le stock physique du stock vendable.

---

## Stock physique et stock vendable

Le stock physique représente toutes les unités présentes dans l’entrepôt.

Le stock vendable représente uniquement les unités pouvant être vendues.

Pour les articles alimentaires, un bucket est considéré comme vendable si sa DLC n’est pas expirée à la date de consultation.

Pour les articles non alimentaires, un bucket est considéré comme vendable si son niveau de packaging est `New` ou `Refurbished`.

Les buckets avec le niveau `Unsellable` ne sont pas considérés comme vendables.

Exemples :

```text
Article alimentaire :
- Bucket DLC 25/06/2026 : vendable si la date du jour est inférieure ou égale au 25/06/2026
- Bucket DLC 10/07/2026 : vendable si la date du jour est inférieure ou égale au 10/07/2026
```

```text
Article non alimentaire :
- Bucket New : vendable
- Bucket Refurbished : vendable
- Bucket Unsellable : non vendable
```

---

## Approvisionnement

L’approvisionnement est modélisé comme un mouvement d’entrée de stock.

Pour un article alimentaire, l’approvisionnement crée un bucket avec une DLC.

Pour un article non alimentaire, l’approvisionnement crée un bucket avec un niveau de packaging.

Chaque approvisionnement crée ensuite une ligne de mouvement positive sur le bucket créé.

Exemple alimentaire :

```text
Article : Yaourt nature
Quantité approvisionnée : 40
DLC : 25/06/2026

Création :
- FoodSupplyMovement
- StockBucket avec DLC 25/06/2026
- StockMovementLine avec QuantityDelta = +40
```

Exemple non alimentaire :

```text
Article : Casque audio
Quantité approvisionnée : 10
Packaging : New

Création :
- NonFoodSupplyMovement
- StockBucket avec Packaging = New
- StockMovementLine avec QuantityDelta = +10
```

Dans cette implémentation, un approvisionnement est considéré comme une entrée de stock au sens large, et pas uniquement comme une livraison fournisseur composée exclusivement de produits neufs.

Cette hypothèse permet de représenter simplement plusieurs situations réelles :

- une livraison fournisseur classique ;
- une entrée de stock reconditionné ;
- une entrée de stock contenant des produits non vendables ;
- une correction ou une régularisation de stock.

---

## Vente

La vente est modélisée comme un mouvement de sortie de stock.

La vente ne porte pas directement une quantité globale.

Elle contient des lignes de mouvement négatives sur les buckets consommés.

Ce choix permet de gérer les cas où une vente consomme plusieurs buckets.

Exemple :

```text
Vente demandée : 12 unités

Bucket A disponible : 10 unités
Bucket B disponible : 20 unités

La vente consomme :
- 10 unités du Bucket A
- 2 unités du Bucket B
```

Pour les articles alimentaires, une règle métier de type FEFO peut être utilisée afin de consommer en priorité les buckets dont la DLC est la plus proche.

```text
FEFO = First Expired, First Out
```

Pour les articles non alimentaires, seuls les buckets vendables peuvent être consommés.

Les buckets avec un packaging `Unsellable` ne doivent pas être consommés par une vente.

La vente conserve également les informations de prix et de TVA appliquées au moment de la vente.

Cela permet de garder un historique cohérent même si le prix de l’article change plus tard.

---

## Inventaire

L’inventaire permet d’aligner le système avec la réalité constatée dans l’entrepôt.

Il est modélisé comme un mouvement métier dédié.

Dans le modèle de domaine, l’inventaire peut ajuster un ou plusieurs buckets.

Chaque écart constaté est représenté par une `StockMovementLine`.

Exemple :

```text
Bucket calculé : 10 unités
Quantité constatée : 7 unités
Delta d’inventaire : -3
```

L’inventaire ne modifie pas les anciens mouvements.

Il ajoute un nouveau mouvement d’inventaire avec les lignes d’ajustement nécessaires.

Ce choix permet de conserver l’historique complet des écarts constatés.

---

## Choix de persistance

SQLite a été choisi afin de conserver une persistance réelle tout en simplifiant l’exécution du projet.

La base est stockée dans un fichier local `.db`.

Ce choix permet d’éviter une dépendance à SQL Server Express ou LocalDB, tout en gardant une architecture qui permettrait de remplacer SQLite par SQL Server ou PostgreSQL en modifiant uniquement la couche Infrastructure.

Le schéma de base est géré avec les migrations Entity Framework Core afin de conserver une structure de base versionnée et reproductible.

---

## Choix sur le Unit of Work

Aucun `UnitOfWork` explicite n’a été ajouté dans un premier temps.

EF Core `DbContext` joue déjà naturellement ce rôle.

Pour simplifier l’exercice, les repositories encapsulent directement la persistance.

Un `IUnitOfWork` pourrait être introduit plus tard si certains cas d’usage nécessitent de coordonner plusieurs repositories dans une même transaction explicite.

---

## Mapping EF Core

Le mapping EF Core est réalisé avec Fluent API dans la couche Infrastructure.

L’héritage des articles et des mouvements est persisté avec une stratégie TPH.

Cette stratégie simplifie le schéma pour l’exercice, au prix de certaines colonnes spécifiques qui peuvent être nulles selon le type de mouvement.

Tables principales :

```text
Articles
FoodArticleSaleModes
ArticleMovements
StockBuckets
StockMovementLines
```

### Articles

La table `Articles` contient les informations communes aux articles.

Le type concret de l’article est distingué par un discriminant.

### FoodArticleSaleModes

La table `FoodArticleSaleModes` contient les modes de vente autorisés pour les articles alimentaires.

### ArticleMovements

La table `ArticleMovements` contient les mouvements de stock.

Le type concret du mouvement est distingué par un discriminant.

Exemples :

```text
FoodSupply
NonFoodSupply
Sale
Inventory
```

### StockBuckets

La table `StockBuckets` contient les buckets de stock.

Elle porte notamment :

- l’article concerné ;
- la DLC pour les articles alimentaires ;
- le packaging pour les articles non alimentaires ;
- la date de création.

### StockMovementLines

La table `StockMovementLines` contient les impacts chiffrés des mouvements sur les buckets.

Elle porte notamment :

- le mouvement parent ;
- le bucket concerné ;
- le delta de quantité ;
- la quantité avant ;
- la quantité après.

---

## Lancement du projet

### Prérequis

- .NET SDK 8 ou supérieur
- Node.js 20 ou supérieur
- SQLite
- Docker optionnel

### Restaurer les packages .NET

Depuis la racine de la solution :

```bash
dotnet restore
```

### Compiler le projet

```bash
dotnet build
```

### Lancer les tests

```bash
dotnet test
```

### Lancer l’API

```bash
dotnet run --project ./InventoryManagement.Api/InventoryManagement.Api.csproj
```

L’API est ensuite accessible via Swagger :

```text
https://localhost:7280/swagger
```

L’URL exacte peut varier selon la configuration locale.

---

## Lancer le client

Le client nécessite Node.js 20 ou supérieur.

Depuis la racine du projet :

```bash
cd InventoryManagement.Client
npm install
```

L’URL de l’API est configurée avec la variable suivante :

```text
VITE_API_BASE_URL
```

Exemple de valeur locale :

```text
VITE_API_BASE_URL=https://localhost:7280
```

Le fichier `.env.example` documente cette variable.

Le fichier `.env.development` fournit la configuration de développement local.

### Lancer le client en développement

```bash
npm run dev
```

### Compiler le client

```bash
npm run build
```

Au chargement, le client appelle `GET /api/ping`.

Tant que l’API n’est pas joignable, un écran d’attente est affiché et une nouvelle tentative est effectuée toutes les 2 secondes.

La configuration CORS locale autorise le serveur Vite sur :

```text
http://localhost:3039
http://127.0.0.1:3039
```

Docker publie également l’API en HTTP sur :

```text
http://localhost:58995
```

Le client utilise directement l’endpoint HTTPS afin d’éviter une redirection HTTP vers HTTPS pendant les appels CORS.

---

## Base de données et migrations

Le projet utilise SQLite comme base de données locale.

Le schéma de base est géré avec les migrations Entity Framework Core.

### Générer une migration

Depuis la racine de la solution :

```bash
dotnet ef migrations add NomDeLaMigration --project ./InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj --startup-project ./InventoryManagement.Api/InventoryManagement.Api.csproj --context StockDbContext --output-dir Persistence/Migrations
```

Exemple :

```bash
dotnet ef migrations add AddStockBucketsAndMovementLines --project ./InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj --startup-project ./InventoryManagement.Api/InventoryManagement.Api.csproj --context StockDbContext --output-dir Persistence/Migrations
```

La migration corrective `RemovePackagingLevelFromSupplyMovement` supprime la colonne résiduelle `PackagingLevel` de `ArticleMovements`, cette donnée étant désormais portée uniquement par `NonFoodStockBucket`.

### Appliquer les migrations

```bash
dotnet ef database update --project ./InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj --startup-project ./InventoryManagement.Api/InventoryManagement.Api.csproj --context StockDbContext
```

En cas de problème avec le chemin relatif de la base SQLite, il est possible de préciser explicitement la chaîne de connexion :

```bash
dotnet ef database update --project ./InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj --startup-project ./InventoryManagement.Api/InventoryManagement.Api.csproj --context StockDbContext --connection "Data Source=C:\\Users\\m.jebarat\\InventoryManagement\\data\\stock-management.db"
```

---

## Chemins de base de données

Deux chaînes de connexion sont utilisées selon l’environnement.

En exécution locale, la base est située dans le dossier `data` à la racine du projet.

Exemple :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../data/stock-management.db"
  }
}
```

En exécution Docker, le dossier local `./data` est monté dans le conteneur sur `/app/data`.

Exemple :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/app/data/stock-management.db"
  }
}
```

Le fichier SQLite reste donc en dehors de l’image Docker.

L’image Docker contient uniquement l’application.

La base de données est montée au runtime via Docker Compose.

---

## Docker

Si le projet est lancé avec Docker Compose :

```bash
docker compose up --build
```

Le fichier `docker-compose.yml` monte le dossier local `./data` dans le conteneur :

```yaml
volumes:
  - ./data:/app/data
```

Cela permet de conserver la base de données même si le conteneur est supprimé ou recréé.

---

## Environnements

Le projet utilise plusieurs fichiers de configuration afin de distinguer les contextes d’exécution :

```text
appsettings.json
appsettings.Development.json
appsettings.Docker.json
```

- `Development` utilise un chemin local relatif vers `../data/stock-management.db`.
- `Docker` utilise le chemin Linux `/app/data/stock-management.db`, qui correspond au volume monté par Docker Compose.

Dans cette version, les migrations sont lancées explicitement avec la commande `dotnet ef database update`.

Ce choix permet de garder le cycle de création et de mise à jour de la base explicite.

Une évolution possible serait d’appliquer automatiquement les migrations au démarrage de l’application uniquement dans un environnement de développement ou Docker local.

Ce choix n’est pas activé systématiquement afin d’éviter qu’un démarrage applicatif modifie automatiquement la structure de la base dans un environnement plus proche de la production.

---

## Fonctionnalités réalisées

À ce stade, les fonctionnalités réalisées sont :

- création d’articles alimentaires ;
- création d’articles non alimentaires ;
- recherche d’articles paginée, filtrée et triée ;
- consultation d’une fiche article ;
- consultation des informations principales d’un article ;
- formulaire client adaptatif selon le type d’article ;
- navigation entre la grille et la fiche article ;
- interface disponible en français et en anglais ;
- socle de modélisation des mouvements de stock avec buckets et lignes de mouvement.

Les scénarios d’approvisionnement, de vente et d’inventaire reposent sur le modèle métier mis en place et peuvent être exposés ou enrichis progressivement.

---

## Limites connues

### Périmètre fonctionnel

Le projet ne cherche pas à modéliser un système complet de caisse, de facturation, de paiement ou de ticket de caisse.

La vente est modélisée comme un mouvement de sortie de stock.

L’approvisionnement est modélisé comme un mouvement d’entrée de stock.

L’inventaire est modélisé comme un mouvement d’ajustement de stock.

### Inventaire

Le modèle de données permet d’inventorier par bucket.

Une version plus avancée pourrait exposer une interface dédiée permettant de saisir les quantités constatées par DLC ou par niveau de packaging.

Exemples d’évolutions possibles :

- inventaire alimentaire par DLC ;
- inventaire non alimentaire par niveau de packaging ;
- ajout d’un bucket inconnu constaté pendant l’inventaire ;
- justification obligatoire d’un écart important.

### Vente

Le modèle permet à une vente de consommer plusieurs buckets.

Une évolution possible serait d’ajouter un service de domaine dédié à l’allocation des buckets à consommer.

Pour les articles alimentaires, cette allocation pourrait suivre une stratégie FEFO.

Pour les articles non alimentaires, elle pourrait consommer en priorité les buckets `New`, puis `Refurbished`, en excluant toujours les buckets `Unsellable`.

### Retours et reconditionnement

La gestion des retours clients, du reconditionnement avancé, de la destruction physique des produits expirés ou invendables peut être ajoutée via de nouveaux types de mouvements.

Exemples :

- CustomerReturnMovement ;
- ReconditioningMovement ;
- StockDestructionMovement ;
- ExpiredStockRemovalMovement.

### Mapping TPH

Le modèle utilise une stratégie TPH pour simplifier la persistance de l’héritage avec EF Core.

Cela implique que certaines colonnes spécifiques à certains mouvements peuvent être nulles pour d’autres types de mouvements.

Ce choix est acceptable dans le cadre de l’exercice.

Une évolution possible serait d’utiliser une autre stratégie de mapping si le modèle de persistance devenait plus complexe.

---

## Gestion des erreurs HTTP

Dans cette première version, les erreurs métier levées par le domaine, notamment via les Value Objects ou les règles métier, sont interceptées globalement et traduites en réponse HTTP `400 Bad Request`.

Ce choix permet d’éviter de retourner des erreurs `500 Internal Server Error` pour des erreurs liées aux données envoyées par l’utilisateur.

Exemple :

```text
Référence EAN-13 invalide
Quantité invalide
Prix invalide
Type d’article invalide
```

Cette gestion est volontairement simplifiée dans le cadre de l’exercice.

Une évolution possible serait de raffiner le mapping des erreurs applicatives vers des codes HTTP plus précis :

- `400 Bad Request` pour les données invalides ;
- `404 Not Found` lorsqu’une ressource demandée n’existe pas ;
- `409 Conflict` lorsqu’une ressource existe déjà, par exemple une référence article déjà utilisée ;
- `422 Unprocessable Entity` pour certaines erreurs de validation métier plus fines.

Cette évolution permettrait de mieux distinguer les erreurs de validation, les conflits métier et les ressources introuvables.

---

## Usage de l’IA

J’ai utilisé ChatGPT comme assistant de réflexion et d’aide à la conception.

L’IA a été utilisée pour :

- analyser l’énoncé ;
- comparer plusieurs approches d’architecture ;
- réfléchir à la modélisation DDD ;
- discuter les choix entre modèle simple et modèle métier plus riche ;
- réfléchir à la séparation entre article, bucket, mouvement et ligne de mouvement ;
- générer des exemples de structure de projet ;
- proposer des exemples de configuration EF Core ;
- challenger les choix de modélisation ;
- aider à rédiger la documentation.

Codex a également été utilisé pour certaines tâches d’implémentation, notamment :

- analyse du code existant ;
- génération ou adaptation de code backend ;
- génération ou adaptation de code frontend ;
- ajout de tests ;
- mise à jour de documentation.

Le code final a été relu, adapté et organisé manuellement.

Les choix d’architecture, les arbitrages métier, les contrats API et les limites fonctionnelles ont été validés humainement avant intégration.

---

## Temps passé

Temps passé approximatif : à compléter avant la remise.

Répartition indicative :

- analyse de l’énoncé et modélisation : à compléter ;
- mise en place de l’architecture backend : à compléter ;
- persistance EF Core et migrations : à compléter ;
- interface client : à compléter ;
- tests et corrections : à compléter ;
- documentation : à compléter.

---

## Synthèse des principaux choix

Les principaux choix retenus sont :

- séparer clairement les couches API, Application, Domain et Infrastructure ;
- garder la logique métier dans le domaine autant que possible ;
- utiliser des Value Objects pour les concepts métier importants ;
- ne pas porter la quantité directement sur l’article ;
- ne pas porter la DLC directement sur l’article alimentaire ;
- ne pas porter le packaging directement sur l’article non alimentaire ;
- représenter les lots ou catégories de stock avec `StockBucket` ;
- représenter les impacts chiffrés des mouvements avec `StockMovementLine` ;
- calculer le stock physique à partir des deltas de mouvements ;
- calculer le stock vendable selon les caractéristiques des buckets ;
- utiliser SQLite pour simplifier l’exécution tout en conservant une vraie persistance ;
- utiliser EF Core `DbContext` comme Unit of Work implicite ;
- conserver une documentation explicite des hypothèses et des limites.
