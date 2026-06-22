# Périmètre fonctionnel

## Objectif

Le projet consiste à réaliser un back-office de gestion de stocks.

L’énoncé est volontairement ouvert : il n’est pas nécessaire de tout développer. Le but est de montrer la capacité à structurer proprement le projet, à faire des choix cohérents et à livrer une base fonctionnelle qui compile.

---

## Concepts principaux

Le projet manipule principalement :

- des articles ;
- des articles alimentaires ;
- des articles non alimentaires ;
- des quantités de stock ;
- des approvisionnements ;
- des ventes ;
- des inventaires ;
- des mouvements de stock.

---

## Article

Un article est caractérisé par :

- un numéro de référence unique ;
- un nom ;
- un prix de vente hors taxes ;
- un prix de vente toutes taxes comprises ;
- un type : alimentaire ou non alimentaire.

Le numéro de référence suit le format EAN-13.

---

## Articles alimentaires

Un article alimentaire possède :

- une DLC, c’est-à-dire une date limite de consommation ;
- une indication de vente à emporter ;
- une indication de vente sur place si nécessaire selon le modèle retenu.

La TVA dépend du mode de vente :

- 5,5 % pour les articles à emporter ;
- 10 % pour les autres cas.

Si un article peut être vendu dans plusieurs modes, le modèle doit permettre d’exprimer clairement cette règle.

---

## Articles non alimentaires

Un article non alimentaire possède un niveau de packaging :

- neuf ;
- reconditionné ;
- invendable.

La TVA est de 20 %.

Un article invendable ne doit pas être considéré comme du stock vendable.

---

## Stock

La quantité en stock d’un article correspond à la somme des approvisionnements depuis le dernier inventaire, moins les quantités vendues.

L’inventaire permet de réaligner le système avec la réalité physique de l’entrepôt.

---

## Mouvements de stock

Les mouvements de stock identifiés sont :

- approvisionnement ;
- vente ;
- inventaire.

Un approvisionnement augmente le stock.

Une vente diminue le stock.

Un inventaire remet le stock au niveau constaté physiquement.

---

## Fonctionnalités possibles

Les fonctionnalités envisageables sont :

- ajouter un article ;
- modifier un article ;
- supprimer un article ;
- rechercher des articles ;
- afficher la liste des articles ;
- saisir un approvisionnement ;
- saisir une vente ;
- saisir un inventaire ;
- afficher l’historique des mouvements ;
- afficher le stock vendable.

Tout n’est pas obligatoire.

---

## Priorité recommandée pour l’exercice

Pour montrer une bonne maîtrise technique sans surcharger le projet, prioriser :

1. création d’article ;
2. distinction alimentaire / non alimentaire ;
3. persistance propre ;
4. calcul de TVA ;
5. mouvements de stock simples ;
6. inventaire ;
7. affichage côté client ;
8. tests unitaires sur les règles métier importantes.

---

## Hypothèses à documenter

Toute hypothèse fonctionnelle doit être notée dans le README ou dans ce fichier.

Exemples :

- choix du format de référence ;
- choix du calcul du prix TTC ;
- choix du modèle de vente à emporter / sur place ;
- choix de gestion du stock vendable ;
- choix de comportement pour un article supprimé ;
- choix de comportement en cas de stock insuffisant.
