# Index de la documentation

Ce fichier est le point d’entrée de la documentation du projet.

Avant de travailler sur le projet, lire ce fichier puis ouvrir uniquement les documents utiles à la tâche en cours.

---

## Documentation générale

| Document | Rôle | À lire quand |
|---|---|---|
| `../README.md` | Vue d’ensemble du projet, installation, commandes, choix, hypothèses, usage de l’IA | Toujours avant de commencer |
| `../AGENTS.md` | Processus de travail et règles à respecter par les agents IA | Toujours avant de commencer |
| `01-functional-scope.md` | Périmètre fonctionnel de l’exercice de gestion de stocks | Lors d’un changement métier ou fonctionnel |
| `02-architecture.md` | Architecture Hexagonale / DDD, couches, dépendances | Avant tout changement backend, domaine ou infrastructure |
| `03-domain-model.md` | Modèle métier, invariants, articles, stocks, mouvements | Avant de modifier les règles métier |
| `04-ai-usage.md` | Usage de l’IA, décisions humaines, limites | À mettre à jour quand l’IA est utilisée |

---

## Documentation backend

| Document | Rôle | À lire quand |
|---|---|---|
| `backend/api.md` | Endpoints API, DTO, contrats de requête/réponse | Lors d’un changement controller/API/client |
| `backend/database-migrations.md` | Règles et commandes de migration de base de données | Lors d’un changement de persistance ou de schéma |
| `backend/testing.md` | Stratégie de tests backend | Lors de l’ajout ou modification de tests backend |

---

## Documentation client

| Document | Rôle | À lire quand |
|---|---|---|
| `client/ui-guidelines.md` | Organisation du client Vite.js, composants, règles UI | Lors d’un changement frontend |
| `client/api-client.md` | Conventions d’appel API depuis le client | Lors d’une intégration frontend/backend |
| `client/testing.md` | Stratégie de tests frontend | Lors de l’ajout ou modification de tests client |

---

## Décisions d’architecture

| Document | Rôle | À lire quand |
|---|---|---|
| `adr/0001-architecture-hexagonale-ddd.md` | Décision expliquant le choix Hexagonal / DDD | Lors d’un arbitrage ou d’une explication d’architecture |

---

## Règle d’utilisation

Ne pas tout lire systématiquement.

Lire d’abord :

1. `README.md`
2. `AGENTS.md`
3. `docs/00-index.md`

Puis sélectionner les documents utiles selon la tâche.

En cas de doute, lire la documentation d’architecture et le modèle de domaine avant de modifier le code.
