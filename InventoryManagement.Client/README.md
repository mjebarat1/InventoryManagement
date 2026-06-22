# Inventory Management Client

Client React/TypeScript du back-office de gestion de stocks, basé sur Vite et Material UI.

## Prérequis

- Node.js 20 ou supérieur ;
- API backend disponible localement.

## Configuration

Copier `.env.example` si une configuration locale spécifique est nécessaire :

```txt
VITE_API_BASE_URL=https://localhost:7280
```

Le fichier `.env.development` contient déjà cette valeur pour le développement local.

## Commandes

```bash
npm install
npm run dev
npm run build
```

## Internationalisation

Les traductions françaises et anglaises sont stockées dans `src/locales/translations`. Toute nouvelle chaîne visible doit être ajoutée dans les deux fichiers JSON et utilisée via `useTranslate`.

## Limites actuelles

La page Articles reste vide tant qu'un endpoint de consultation n'est pas documenté côté backend. La configuration CORS du backend devra également être traitée avant les premiers appels depuis le navigateur.
