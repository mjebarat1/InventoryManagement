# Tests client

## Objectif

Ce document décrit la stratégie de tests côté client.

---

## Priorités

Les tests client ne doivent pas dupliquer les tests métier backend.

Ils peuvent vérifier :

- le rendu d’un composant ;
- le comportement d’un formulaire ;
- l’appel d’une fonction API ;
- l’affichage d’un message d’erreur ;
- la navigation utilisateur.

---

## Règles

Les règles métier importantes restent testées côté backend.

Le client peut tester les comportements UI.

---

## Commandes

Adapter selon les scripts réellement présents dans `package.json`.

Exemples :

```bash
npm test
npm run test
npm run build
```

Ne pas lancer les tests sans demande explicite de l’utilisateur.

Le build client peut être lancé après modification frontend si nécessaire :

```bash
npm run build
```

---

## Règle pour les agents IA

Lorsqu’un changement frontend modifie un comportement, proposer les tests utiles.

Ne pas exécuter les tests sans demande explicite.

Indiquer dans le résumé final si les tests ont été ajoutés, modifiés ou non exécutés.
