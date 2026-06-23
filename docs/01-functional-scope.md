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

- une indication de vente à emporter ;
- une indication de vente sur place si nécessaire selon le modèle retenu.

La DLC est portée par chaque bucket alimentaire, et non par la fiche article ou le mouvement global.

La TVA dépend du mode de vente :

- 5,5 % pour les articles à emporter ;
- 10 % pour les autres cas.

Si un article peut être vendu dans plusieurs modes, le modèle doit permettre d’exprimer clairement cette règle.

---

## Articles non alimentaires

Le niveau de packaging d’un article non alimentaire est porté par chaque bucket de stock :

- neuf ;
- reconditionné ;
- invendable.

La TVA est de 20 %.

Un article invendable ne doit pas être considéré comme du stock vendable.

---

## Stock

La quantité en stock d’un article correspond à la somme des `QuantityDelta` de ses lignes de mouvements, regroupées par bucket.

L’inventaire permet de réaligner le système avec la réalité physique de l’entrepôt.

Une vente dont la quantité dépasse le stock vendable est refusée intégralement. Aucune vente partielle et aucune ligne de mouvement ne sont enregistrées.

Chaque bucket est identifié par une référence métier globale au format `ref-lot-` suivi de 13 chiffres. Un inventaire peut ajuster une sélection de buckets existants et créer de nouveaux buckets constatés.

---

## Mouvements de stock

Les mouvements de stock identifiés sont :

- approvisionnement ;
- vente ;
- inventaire.

Un approvisionnement ajoute une ligne positive sur un bucket.

Une vente ajoute une ou plusieurs lignes négatives sur les buckets consommés.

Un inventaire ajoute des lignes d’ajustement représentant les écarts constatés par bucket.

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
