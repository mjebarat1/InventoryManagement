# AGENTS.md

## Contexte du projet

Ce dépôt contient un projet technique autour d’un back-office de gestion de stocks.

L’objectif n’est pas seulement de livrer des fonctionnalités, mais de montrer :

- des bonnes pratiques de développement ;
- une architecture claire et adaptée ;
- une bonne séparation des responsabilités ;
- un projet qui compile et reste maintenable.

Le projet contient actuellement :

- un backend en C# ;
- une architecture Hexagonale / DDD déjà mise en place ;
- au moins une action de controller fonctionnelle ;
- un projet client basé sur un template gratuit Vite.js ;
- des fichiers de documentation à la racine et dans le dossier `docs`.

L’architecture actuelle est considérée comme un choix structurant. Elle doit être respectée.

---

## Règle impérative : lire la documentation avant d’agir

Avant toute analyse, modification ou génération de code, lire la documentation du projet.

Commencer toujours par :

1. `README.md`
2. `AGENTS.md`
3. `docs/00-index.md`

Ensuite, lire uniquement les documents utiles à la demande.

Ne pas supposer une règle métier, une convention ou un choix architectural sans vérifier la documentation existante.

Si la documentation est incomplète, contradictoire ou ambiguë, le signaler avant de proposer une modification.

---

## Navigation dans la documentation

Le fichier `docs/00-index.md` est le point d’entrée de la documentation.

Il indique :

- où trouver le périmètre fonctionnel ;
- où trouver les règles d’architecture ;
- où trouver le modèle de domaine ;
- où trouver les conventions backend ;
- où trouver les conventions client ;
- où trouver les règles de migration de base de données ;
- où trouver les règles de tests ;
- où documenter l’usage de l’IA.

Pour une modification backend, lire d’abord :

- `docs/02-architecture.md`
- `docs/03-domain-model.md`
- les documents utiles dans `docs/backend`

Pour une modification frontend, lire d’abord :

- `docs/client/ui-guidelines.md`
- `docs/client/api-client.md`

Pour une modification métier, lire d’abord :

- `docs/01-functional-scope.md`
- `docs/02-architecture.md`
- `docs/03-domain-model.md`

---

## Processus obligatoire de travail

Pour chaque demande de développement, suivre le processus suivant.

### 1. Analyser la demande

Identifier clairement :

- l’objectif fonctionnel ;
- les règles métier concernées ;
- les concepts du domaine impactés ;
- les cas d’usage applicatifs impactés ;
- les adapters backend impactés ;
- les impacts éventuels sur le client ;
- les impacts éventuels sur la base de données ;
- les tests à ajouter ou modifier ;
- la documentation à mettre à jour.

### 2. Poser des questions si nécessaire

Poser des questions avant de coder si :

- le comportement métier attendu est ambigu ;
- plusieurs implémentations sont possibles ;
- la demande impacte le modèle de domaine ;
- la demande impacte l’architecture ;
- la demande nécessite un arbitrage technique ;
- la demande nécessite une modification de base de données ;
- le comportement attendu côté client n’est pas clair ;
- la documentation existante n’est pas suffisante.

Ne jamais prendre un arbitrage architectural important en silence.

### 3. Proposer un plan avant d’écrire le code

Avant toute modification de code, proposer un plan court.

Le plan doit indiquer :

- les fichiers ou projets probablement modifiés ;
- les impacts sur le domaine ;
- les impacts sur l’application / les cas d’usage ;
- les impacts sur l’infrastructure ;
- les impacts sur l’API ;
- les impacts sur le client ;
- les impacts éventuels sur la base de données ;
- les tests à ajouter ou modifier ;
- la documentation à mettre à jour.

Attendre un accord explicite avant de modifier le code.

Ne pas commencer à coder sans validation du plan.

### 4. Implémenter uniquement le plan validé

Après validation, implémenter uniquement ce qui a été accepté.

Ne pas faire de refactoring non demandé.

Ne pas renommer des concepts existants sans accord.

Ne pas introduire une nouvelle librairie, un nouveau framework ou un nouveau pattern sans validation.

---

## Règles d’architecture

Le projet suit une architecture Hexagonale avec des principes DDD.

Respecter les règles suivantes :

- le domaine ne dépend d’aucune technologie ;
- le domaine ne dépend pas des controllers ;
- le domaine ne dépend pas d’Entity Framework ;
- le domaine ne dépend pas de la base de données ;
- le domaine ne dépend pas du client ;
- le domaine ne dépend pas de HTTP ;
- le domaine ne dépend pas de la sérialisation JSON ;
- les controllers sont des adapters entrants ;
- les repositories sont des ports côté application/domaine et des adapters côté infrastructure ;
- les cas d’usage orchestrent les opérations métier ;
- les règles métier importantes doivent vivre dans le domaine ;
- les DTO ne doivent pas remplacer les objets du domaine.

Si une décision nécessite un arbitrage entre plusieurs approches, arrêter le travail et demander.

---

## Règles DDD

Préserver les invariants métier.

Utiliser des noms explicites et métier.

Éviter de créer un modèle anémique si le comportement appartient naturellement à une entité, un value object ou un service de domaine.

Ne pas ajouter de setters publics uniquement pour faciliter le mapping.

Ne pas exposer directement les entités du domaine à l’API ou au client si des DTO existent déjà.

Avant d’ajouter une fonctionnalité, identifier si elle concerne :

- les articles ;
- les articles alimentaires ;
- les articles non alimentaires ;
- les mouvements de stock ;
- les approvisionnements ;
- les ventes ;
- les inventaires ;
- le calcul de prix ;
- la TVA ;
- le packaging ;
- la DLC.

---

## Règles backend

Les controllers doivent rester fins.

Un controller doit principalement :

- recevoir la requête HTTP ;
- effectuer une validation d’entrée simple si nécessaire ;
- appeler le bon cas d’usage ;
- convertir le résultat en réponse HTTP.

Ne pas mettre de logique métier dans les controllers.

Les règles métier doivent être dans le domaine ou dans les cas d’usage selon leur nature.

Les erreurs doivent être explicites.

Limitation acceptée à court terme : certaines erreurs peuvent actuellement retourner `BadRequest`. Cette gestion pourra être raffinée dans un second temps.

---

## Règles frontend

Le client est basé sur Vite.js.

Avant toute modification frontend :

- inspecter la structure existante ;
- respecter les conventions déjà présentes ;
- ne pas remplacer le template ;
- ne pas changer l’architecture frontend sans accord.

Isoler les appels API dans une couche dédiée si la structure le permet.

Ne pas dupliquer les règles métier backend dans le client, sauf validation légère pour l’expérience utilisateur.

Le backend reste la source de vérité des règles métier.

---

## Base de données et migrations

Si une modification impacte le schéma de base de données, créer une migration propre.

Ne pas modifier la base manuellement.

Ne pas utiliser de scripts SQL ponctuels sans validation.

Une modification de schéma doit inclure :

- la modification du modèle ;
- la migration ;
- les changements de mapping ;
- la documentation si la modification est significative.

Le nom de migration doit être clair et expliquer l’intention.

Après création d’une migration, vérifier que le projet build.

En cas de doute sur le bon modèle de données, demander avant de créer la migration.

---

## Build

Après toute modification de code, vérifier que le projet compile.

Pour le backend, utiliser la commande adaptée à la solution, par exemple :

```bash
dotnet build
```

Pour le client, utiliser la commande adaptée depuis le dossier du projet client, par exemple :

```bash
npm run build
```

Adapter les commandes à la structure réelle du dépôt.

Si le build échoue, corriger le problème ou expliquer précisément l’échec.

Le projet livré doit rester compilable.

---

## Tests unitaires

Ajouter ou mettre à jour les tests unitaires lorsqu’un changement modifie un comportement.

Prioriser les tests :

1. du domaine ;
2. des cas d’usage applicatifs ;
3. des adapters si nécessaire ;
4. des controllers uniquement si cela apporte une vraie valeur.

Ne pas lancer la suite de tests unitaires sans demande explicite de l’utilisateur.

Il est autorisé d’ajouter des tests et de vérifier que le projet compile.

Ne lancer des commandes de tests que si l’utilisateur le demande explicitement, par exemple :

```bash
dotnet test
npm test
npm run test
```

Si des tests sont ajoutés mais non exécutés, le mentionner clairement dans le résumé final.

---

## Documentation

Mettre à jour la documentation après tout changement significatif.

La documentation doit expliquer :

- la fonctionnalité implémentée ;
- les hypothèses importantes ;
- les choix d’architecture ;
- les commandes utiles ;
- les impacts sur la base de données ;
- les migrations éventuelles ;
- l’usage de l’IA si pertinent.

Le fichier `README.md` doit rester orienté évaluateur :

- objectif du projet ;
- technologies choisies ;
- architecture choisie ;
- hypothèses ;
- commandes pour lancer le backend ;
- commandes pour lancer le client ;
- commandes de migration ;
- commandes de build ;
- commandes de tests ;
- usage de l’IA ;
- temps passé.

Les détails techniques doivent être placés dans `docs`.

Si un nouveau fichier de documentation est créé, mettre à jour `docs/00-index.md`.

---

## Documentation de l’usage de l’IA

Lorsque l’IA est utilisée, mettre à jour la documentation concernée.

Mentionner :

- l’outil utilisé ;
- le type d’usage : réflexion, architecture, génération de code, tests, documentation ;
- ce qui a été relu ou corrigé humainement ;
- les limites ou arbitrages humains.

Ne jamais laisser entendre que le code généré par IA a été accepté sans revue.

---

## Règles de sécurité et de qualité

Ne pas supprimer une fonctionnalité existante sans accord.

Ne pas masquer du code inachevé.

Si du code est incomplet, le commenter clairement ou ne pas l’intégrer.

Ne pas créer de fausse implémentation uniquement pour faire passer le build.

Ne pas effectuer de refactoring large pendant une tâche fonctionnelle sans validation.

Ne pas modifier un contrat d’API public sans expliquer l’impact.

Ne pas modifier le schéma de base de données sans migration.

Ne pas lancer de commande destructive.

---

## Format attendu des réponses de l’agent

Pour une demande de développement, répondre d’abord avec :

1. Compréhension de la demande
2. Questions éventuelles
3. Plan proposé
4. Fichiers probablement impactés
5. Plan de validation / build
6. Impact documentation

Attendre l’accord avant de coder.

Après implémentation, résumer :

- ce qui a changé ;
- les fichiers modifiés ;
- si le build a été lancé ;
- si des tests ont été ajoutés ;
- si les tests ont été lancés ou non ;
- si la documentation a été mise à jour ;
- les limites restantes.
