# Tests backend

## Objectif

Ce document décrit la stratégie de tests backend.

---

## Priorités

Les tests doivent prioriser :

1. les règles métier du domaine ;
2. les cas d’usage applicatifs ;
3. les adapters d’infrastructure si nécessaire ;
4. les controllers uniquement lorsque cela apporte une vraie valeur.

---

## Tests de domaine

Les tests de domaine doivent vérifier les invariants métier.

Exemples :

- une référence EAN-13 invalide est refusée ;
- un prix HT négatif est refusé ;
- un article alimentaire doit avoir une DLC ;
- un article non alimentaire doit avoir un packaging ;
- le taux de TVA est correct selon le type d’article ;
- le stock vendable exclut les articles invendables.

---

## Tests applicatifs

Les tests applicatifs doivent vérifier les cas d’usage.

Exemples :

- création d’un article ;
- refus d’une référence déjà utilisée ;
- saisie d’un approvisionnement ;
- saisie d’un inventaire ;
- calcul du stock après mouvements.

Les dépendances externes doivent être remplacées par des fakes ou mocks si nécessaire.

---

## Tests de controllers

Les tests de controllers ne sont pas prioritaires si la logique est correctement placée dans les cas d’usage.

Ils peuvent être utiles pour vérifier :

- le mapping HTTP ;
- les statuts de réponse ;
- les DTO exposés.

---

## Commandes

Ne pas lancer les tests sans demande explicite de l’utilisateur.

Commande possible :

```bash
dotnet test
```

Build possible :

```bash
dotnet build
```

---

## Règle pour les agents IA

Lorsqu’un changement modifie un comportement, ajouter ou mettre à jour les tests.

Ne pas exécuter la suite de tests sauf si l’utilisateur le demande explicitement.

Indiquer clairement dans le résumé final :

- tests ajoutés ;
- tests modifiés ;
- tests non exécutés ;
- build exécuté ou non.
