# UserAnimeList.Web

Frontend reconstruído do zero com React + TypeScript + Vite.

## Objetivo
- Consumir **todos os endpoints** da API (`login`, `token`, `user`, `anime`, `animelist`, `genre`, `studio`).
- Espelhar contratos da pasta `src/Shared/UserAnimeList.Communication` em `src/types/contracts.ts`.

## Rodando
```bash
cd src/Frontend/UserAnimeList.Web
npm install
npm run dev
```

Backend esperado em `http://localhost:5103` (proxy configurado em `vite.config.ts`).

## Placeholders
- Anime: `public/placeholders/anime-no-image.svg`
- Perfil: `public/placeholders/profile-no-image.svg`
