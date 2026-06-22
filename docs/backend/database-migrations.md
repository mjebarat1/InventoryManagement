# Migrations de base de données

## Objectif

Ce document décrit les règles à respecter lors d’un changement de schéma de base de données.

---

## Règle principale

Toute modification du schéma de base de données doit passer par une migration propre.

Ne pas modifier la base manuellement.

Ne pas contourner les migrations pour aller plus vite.

---

## Quand créer une migration ?

Créer une migration si la modification ajoute, modifie ou supprime :

- une table ;
- une colonne ;
- une relation ;
- un index ;
- une contrainte ;
- une propriété persistée ;
- un mapping ayant un impact sur le schéma.

---

## Nom de migration

Le nom doit être explicite.

Exemples :

```bash
AddArticlePackagingLevel
AddStockMovements
AddFoodArticleExpirationDate
```

Éviter les noms vagues comme :

```bash
UpdateDatabase
FixStuff
Migration1
```

---

## Commandes

Adapter les commandes à la structure réelle de la solution.

Exemple EF Core :

```bash
dotnet ef migrations add AddStockMovements --project StockManagement.Infrastructure --startup-project StockManagement.Api --context StockDbContext --output-dir Persistence/Migrations
```

Application de la migration :

```bash
dotnet ef database update --project StockManagement.Infrastructure --startup-project StockManagement.Api --context StockDbContext
```

Build :

```bash
dotnet build
```

---

## SQLite

Si le projet utilise SQLite avec un fichier `.db`, vérifier :

- le chemin réel du fichier ;
- la chaîne de connexion ;
- les droits d’écriture ;
- le dossier parent du fichier ;
- l’environnement d’exécution, notamment Docker si utilisé.

Attention à ne pas construire une chaîne de connexion dupliquée du type :

```txt
Data Source=Data Source=/app/data/stock-management.db
```

La forme attendue est plutôt :

```txt
Data Source=/app/data/stock-management.db
```

---

## Documentation à mettre à jour

Après une migration importante, mettre à jour :

- `README.md` si une commande change ;
- ce fichier si la procédure change ;
- `docs/03-domain-model.md` si le modèle métier change ;
- `docs/backend/api.md` si l’API est impactée.

---

## Règle pour les agents IA

En cas de changement base de données :

1. proposer le modèle ;
2. attendre validation si le choix est structurant ;
3. créer la migration ;
4. vérifier le build ;
5. documenter la migration.
