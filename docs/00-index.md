# Index de la documentation

Ce fichier est le point d’entrée de la documentation du projet.

Il sert à orienter un développeur ou un agent IA vers les bons documents, sans devoir lire tout le dépôt inutilement.

---

## Ordre de lecture obligatoire pour un agent IA

Lire uniquement les documents nécessaires selon la tâche.

---

## Documentation générale

| Document                 | Rôle                                                                    | À lire quand                                             |
| ------------------------ | ----------------------------------------------------------------------- | -------------------------------------------------------- |
| `01-functional-scope.md` | Périmètre fonctionnel de l’exercice de gestion de stocks                | Lors d’un changement métier ou fonctionnel               |
| `02-architecture.md`     | Architecture Hexagonale / DDD, couches, dépendances, règles d’arbitrage | Avant tout changement backend, domaine ou infrastructure |
| `03-domain-model.md`     | Modèle métier, invariants, articles, stocks, mouvements                 | Avant de modifier les règles métier                      |

---

## Documentation backend

| Document                         | Rôle                                                               | À lire quand                                     |
| -------------------------------- | ------------------------------------------------------------------ | ------------------------------------------------ |
| `backend/api.md`                 | Endpoints API, DTO, contrats de requête/réponse, gestion d’erreurs | Lors d’un changement controller/API/client       |
| `backend/database-migrations.md` | Règles et commandes de migration de base de données                | Lors d’un changement de persistance ou de schéma |
| `backend/testing.md`             | Stratégie de tests backend                                         | Lors de l’ajout ou modification de tests backend |

---

## Documentation client

| Document                  | Rôle                                                  | À lire quand                                    |
| ------------------------- | ----------------------------------------------------- | ----------------------------------------------- |
| `client/ui-guidelines.md` | Organisation du client Vite.js, composants, règles UI | Lors d’un changement frontend                   |
| `client/api-client.md`    | Conventions d’appel API depuis le client              | Lors d’une intégration frontend/backend         |
| `client/testing.md`       | Stratégie de tests frontend                           | Lors de l’ajout ou modification de tests client |
| `client/initial-vite-client-cleanup.md` | Nettoyage initial, configuration, navigation et i18n du client | Pour comprendre la base frontend actuelle |

---

## Décisions d’architecture

| Document                                  | Rôle                                         | À lire quand                                            |
| ----------------------------------------- | -------------------------------------------- | ------------------------------------------------------- |
| `adr/0001-architecture-hexagonale-ddd.md` | Décision expliquant le choix Hexagonal / DDD | Lors d’un arbitrage ou d’une explication d’architecture |

---

## Parcours de lecture par type de tâche

### Nouvelle fonctionnalité métier

Lire :

1. `01-functional-scope.md`
2. `02-architecture.md`
3. `03-domain-model.md`
4. les fichiers backend ou client concernés

### Changement API

Lire :

1. `02-architecture.md`
2. `backend/api.md`
3. `client/api-client.md` si le client est impacté

### Changement base de données

Lire :

1. `02-architecture.md`
2. `03-domain-model.md`
3. `backend/database-migrations.md`

### Changement frontend

Lire :

1. `client/ui-guidelines.md`
2. `client/api-client.md`
3. `backend/api.md` si un endpoint est consommé ou modifié

### Ajout ou modification de tests

Lire :

1. `backend/testing.md` pour le backend
2. `client/testing.md` pour le client
3. `03-domain-model.md` si les tests portent sur des règles métier

### Mise à jour de documentation

Lire :

1. `README.md`
2. `AGENTS.md`
3. le document concerné

Puis sélectionner les documents utiles selon la demande.

En cas de doute, lire :

1. `docs/02-architecture.md`
2. `docs/03-domain-model.md`

Si une règle est absente ou ambiguë, demander une clarification avant de coder.

---

## Règle de mise à jour de l’index

Ce fichier doit être mis à jour dès qu’un nouveau fichier de documentation est ajouté, renommé ou supprimé.

Chaque nouvelle documentation doit apparaître dans l’une des sections suivantes :

- documentation générale ;
- documentation backend ;
- documentation client ;
- décisions d’architecture ;
- autre section dédiée si nécessaire.
