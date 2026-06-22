# Usage de l’IA

## Objectif

Ce fichier documente l’usage de l’IA dans le projet.

Il sert à rester transparent sur l’aide utilisée, tout en montrant que les choix ont été compris, relus et assumés.

---

## Outils utilisés

Outils IA utilisés ou prévus :

- ChatGPT ;
- Codex ;
- éventuellement autres outils à préciser.

---

## Types d’usage

L’IA peut être utilisée pour :

- clarifier l’énoncé ;
- réfléchir à l’architecture ;
- comparer plusieurs choix techniques ;
- générer une première version de code ;
- générer des tests unitaires ;
- relire du code ;
- améliorer la documentation ;
- préparer le README.

---

## Règle de revue humaine

Le code généré par IA ne doit pas être accepté automatiquement.

Chaque proposition doit être relue.

Les points à vérifier sont :

- respect de l’architecture Hexagonale / DDD ;
- absence de logique métier dans les controllers ;
- cohérence du modèle de domaine ;
- qualité des noms ;
- cohérence des migrations ;
- compilation du projet ;
- pertinence des tests ;
- lisibilité de la documentation.

---

## Journal d’usage

Ajouter ici les usages importants de l’IA.

| Date | Outil | Usage | Revue humaine / décision |
|---|---|---|---|
| À compléter | ChatGPT | Aide à la structuration du process IA et de la documentation | Contenu relu et adapté au projet |
| À compléter | Codex | À compléter selon les tâches réalisées | À compléter |

---

## Formulation possible pour le README

Une formulation possible :

```md
L’usage de l’IA a été autorisé dans l’énoncé. Elle a été utilisée comme assistant de réflexion, de structuration et ponctuellement de génération de code/documentation. Les propositions ont été relues, adaptées et validées manuellement afin de respecter l’architecture Hexagonale / DDD du projet.
```

---

## Limites

L’IA peut proposer du code incorrect ou non aligné avec l’architecture du projet.

En cas de doute, la documentation du projet et les choix humains priment sur la proposition de l’IA.
