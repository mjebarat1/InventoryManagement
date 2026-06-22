# ADR 0001 - Utiliser une architecture Hexagonale avec des principes DDD

## Statut

Accepté.

---

## Contexte

L’exercice France Billet demande de réaliser un back-office de gestion de stocks.

L’énoncé est volontairement ouvert et permet de choisir les fonctionnalités et l’architecture.

Il est explicitement apprécié de mettre en place une architecture Hexagonale / DDD avec les building blocks du DDD.

Le projet doit rester raisonnable en taille, mais montrer une bonne capacité de structuration.

---

## Décision

Le projet utilise une architecture Hexagonale avec des principes DDD.

Le domaine est placé au centre.

Les controllers, la base de données et le client sont considérés comme des éléments externes au domaine.

Les cas d’usage applicatifs orchestrent les opérations.

L’infrastructure implémente les ports nécessaires.

---

## Raisons

Ce choix permet :

- de protéger les règles métier ;
- de garder des controllers fins ;
- de rendre les règles testables ;
- de séparer la persistance du métier ;
- de faciliter les évolutions ;
- de montrer une architecture professionnelle sur un exercice court.

---

## Conséquences positives

- Le domaine peut être testé sans base de données.
- Les cas d’usage sont plus lisibles.
- La persistance peut évoluer sans impacter directement le métier.
- Le client reste découplé des règles métier.
- Le projet est plus facile à expliquer en entretien.

---

## Coûts et limites

- L’architecture demande plus de fichiers qu’une approche CRUD directe.
- Certains choix peuvent sembler plus longs pour un petit exercice.
- Il faut rester pragmatique et éviter la sur-ingénierie.
- Tous les building blocks DDD ne sont pas forcément nécessaires.

---

## Règle d’arbitrage

Si une fonctionnalité peut être implémentée de plusieurs façons, privilégier :

1. la clarté métier ;
2. le respect des dépendances ;
3. la testabilité ;
4. la simplicité ;
5. la cohérence avec l’existant.

En cas de doute, demander avant d’implémenter.
