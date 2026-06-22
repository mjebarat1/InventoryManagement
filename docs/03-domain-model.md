# Modèle de domaine

## Objectif

Ce fichier décrit les concepts métier du projet de gestion de stocks.

Il sert de référence avant toute modification des règles métier.

---

## Article

Un article représente un produit géré dans l’entrepôt.

Propriétés métier attendues :

- référence unique ;
- nom ;
- prix hors taxes ;
- prix toutes taxes comprises ou capacité à le calculer ;
- type d’article.

La référence doit respecter le format EAN-13.

---

## Référence EAN-13

La référence article est unique.

Elle suit le format EAN-13.

Il est recommandé de modéliser cette référence comme un value object afin de centraliser la validation.

Règles possibles :

- exactement 13 chiffres ;
- pas de lettres ;
- pas de caractères spéciaux.

Le contrôle de clé EAN-13 peut être ajouté si le temps le permet, mais cette hypothèse doit être documentée.

---

## Prix

Le prix hors taxes est une donnée importante.

Le prix TTC peut être :

- calculé à partir du prix HT et du taux de TVA ;
- ou stocké si le choix est justifié.

Le choix retenu doit être documenté.

Pour éviter les erreurs, préférer un type décimal pour les montants.

---

## TVA

Les taux identifiés sont :

- 5,5 % pour les articles alimentaires à emporter ;
- 10 % pour les autres articles alimentaires ;
- 20 % pour les articles non alimentaires.

La logique de TVA est une règle métier.

Elle ne doit pas être codée uniquement dans le controller ou le client.

---

## Article alimentaire

La DLC n’est pas une propriété de la fiche article. Elle est obligatoire sur chaque mouvement d’approvisionnement alimentaire.

Il peut être :

- destiné à la vente à emporter ;
- non destiné à la vente à emporter ;
- compatible avec les deux modes selon le modèle retenu.

Le modèle doit être explicite sur ce point.

---

## Article non alimentaire

Le niveau de packaging n’est pas une propriété de la fiche article. Il est obligatoire sur les mouvements d’approvisionnement non alimentaire.

Valeurs identifiées :

- neuf ;
- reconditionné ;
- invendable.

Un article invendable ne doit normalement pas être considéré comme vendable.

---

## Stock

Le stock représente la quantité disponible dans l’entrepôt.

La quantité en stock est calculée à partir des mouvements.

Règle métier :

```txt
stock = inventaire le plus récent + approvisionnements depuis cet inventaire - ventes depuis cet inventaire
```

S’il n’existe aucun inventaire, le stock peut être calculé depuis le début de l’historique.

Cette hypothèse doit être confirmée ou documentée.

---

## Mouvement de stock

Un mouvement de stock représente un événement métier.

Types de mouvements :

- approvisionnement ;
- vente ;
- inventaire.

Un approvisionnement augmente le stock.

Une vente diminue le stock.

Un inventaire fixe la quantité réelle constatée.

---

## Stock vendable

Le stock vendable peut différer du stock total.

Exemples :

- un article non alimentaire invendable ne doit pas être vendable ;
- un article alimentaire avec DLC dépassée ne doit pas être vendable ;
- une quantité nulle ou négative ne doit pas être vendable.

Toute règle de stock vendable doit être documentée.

---

## Invariants métier

Les invariants à préserver sont notamment :

- une référence article est obligatoire ;
- une référence article est unique ;
- une référence article doit respecter le format EAN-13 ;
- un nom d’article est obligatoire ;
- un prix HT ne doit pas être négatif ;
- une quantité ne doit pas être négative lorsqu’elle représente une quantité saisie ;
- une DLC est obligatoire pour un approvisionnement alimentaire ;
- un packaging est obligatoire pour un approvisionnement non alimentaire ;
- une vente ne doit pas rendre le stock incohérent, sauf choix fonctionnel documenté.

---

## Questions ouvertes

Les points suivants doivent être arbitrés si nécessaire :

- doit-on autoriser une vente avec stock insuffisant ?
- doit-on stocker le prix TTC ou le calculer ?
- doit-on créer un endpoint unique de création d’article ou deux endpoints séparés ?
- doit-on gérer la suppression physique ou logique des articles ?
- doit-on gérer l’historique complet des modifications d’article ?
- doit-on bloquer la vente d’un produit alimentaire dont la DLC est dépassée ?
