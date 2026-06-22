# Nettoyage initial du client Vite.js

## Objectif

Cette évolution transforme le template Vite.js gratuit en une base cohérente pour le back-office de gestion de stocks, sans modifier le backend ni inventer de contrats API.

## Périmètre réalisé

### Configuration d'environnement

Le client utilise désormais la variable obligatoire `VITE_API_BASE_URL`.

La valeur du profil HTTP local ASP.NET Core est :

```txt
VITE_API_BASE_URL=https://localhost:7280
```

Les fichiers concernés sont :

- `InventoryManagement.Client/.env.example`, qui documente la configuration attendue ;
- `InventoryManagement.Client/.env.development`, utilisé par Vite en développement ;
- `InventoryManagement.Client/src/vite-env.d.ts`, qui type la variable TypeScript.

### Couche d'accès API

Le module `src/api/http-client.ts` centralise l'URL de base et l'utilisation de `fetch`. Il fournit `apiRequest` et une erreur HTTP typée `ApiError`.

Aucun endpoint Articles fictif n'a été ajouté. Le backend possède une action de création d'article alimentaire, mais `docs/backend/api.md` ne documente pas encore de contrat de consultation permettant d'alimenter la grille.

### Disponibilité de l'API au démarrage

Le backend expose `GET /api/ping`, qui retourne `200 OK` avec `{ "status": "ok" }` sans dépendre de la base de données.

Au chargement, le client appelle cet endpoint par l'intermédiaire de `ping-api.ts`. Tant que l'appel échoue, `ApiAvailabilityGate` affiche un écran d'attente traduit et renouvelle la tentative toutes les 2 secondes. Le polling s'arrête dès la première réponse réussie.

La policy backend `ClientCors` autorise explicitement les origines locales Vite `http://localhost:3039` et `http://127.0.0.1:3039` dans les configurations Development et Docker.

### Navigation et routes

La navigation principale contient uniquement :

- Accueil / Dashboard ;
- Articles.

Les routes de démonstration Users, Products, Blog et Sign-in ont été retirées, ainsi que leurs pages, sections et données mockées.

Les éléments promotionnels du template, les notifications fictives, les espaces de travail fictifs, le compte fictif et la recherche globale non fonctionnelle ont également été retirés du layout.

### Dashboard

Le dashboard ne présente plus de statistiques fictives. Il affiche une introduction au back-office et un accès à la page Articles.

### Page Articles

La page prépare une grille avec les colonnes suivantes :

- Référence ;
- Nom ;
- Type ;
- Prix HT ;
- Prix TTC ;
- TVA ;
- Stock ;
- Statut ;
- Actions.

En l'absence d'endpoint GET documenté, la grille affiche un état vide explicite et aucune donnée fictive. Le bouton « Ajouter un article » est présent mais désactivé tant que le formulaire et son intégration API ne sont pas implémentés.

### Internationalisation

L'infrastructure utilise :

- `i18next` ;
- `react-i18next` ;
- `i18next-browser-languagedetector`.

Les traductions sont stockées dans :

```txt
src/locales/translations/fr.json
src/locales/translations/en.json
```

Le français est la langue de repli. Le navigateur peut initialiser la langue et le choix de l'utilisateur est conservé dans `localStorage`. Le sélecteur du template pilote réellement le provider i18n.

Toutes les chaînes visibles des pages conservées passent par `useTranslate`, notamment les menus, titres, libellés de colonnes, états vides, infobulles et erreurs 404.

## Limites et suites nécessaires

- La liste des articles ne peut pas être branchée tant qu'un endpoint de consultation et son DTO ne sont pas disponibles et documentés.
- Le formulaire d'ajout d'article n'est pas inclus dans ce nettoyage initial.
- La disponibilité vérifiée par `/api/ping` concerne le processus HTTP uniquement et ne garantit pas l'accès à la base de données.
- Aucun contrat API public, modèle métier, schéma de base de données ou migration n'a été modifié.

## Commandes utiles

Depuis `InventoryManagement.Client` :

```bash
npm install
npm run dev
npm run build
```

## Validation

Le changement a été validé avec :

- `dotnet build InventoryManagement.Test/InventoryManagement.Test.csproj`, qui compile les tests, l'API et ses dépendances ;
- `npm run build`, qui compile TypeScript et génère le client Vite.

Le build Vite aboutit avec un avertissement non bloquant sur la taille du chunk principal. Le build backend aboutit avec des avertissements préexistants de nullabilité et de vulnérabilités NuGet.

Le build de `InventoryManagement.sln` reste bloqué par une référence préexistante à `InventoryManagement.Client.esproj`, fichier absent du dépôt. La solution n'a pas été modifiée dans le cadre de cette tâche.

Un test unitaire de `PingController` a été ajouté. Il vérifie la réponse HTTP et le payload attendus.

Les suites de tests backend et frontend n'ont pas été exécutées, conformément à la règle interdisant de lancer les tests sans demande explicite.

La vérification HTTP réelle n'a pas pu être exécutée dans l'environnement de l'agent, car celui-ci interdit à Kestrel d'ouvrir un port local (`SocketException 10013`). Les builds valident néanmoins l'assemblage backend et l'intégration client.

## Usage de l'IA

Codex a été utilisé pour analyser le template, comparer son sélecteur de langue avec l'implémentation de référence fournie, générer la structure i18n, adapter les composants, nettoyer les exemples et mettre à jour la documentation.

Le code généré doit faire l'objet d'une revue humaine. Les arbitrages humains restent notamment nécessaires pour le futur contrat de consultation des articles, le formulaire de création et la stratégie CORS.
