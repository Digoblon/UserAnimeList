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


## Links corretos (muito importante)
- **Frontend novo:** `http://localhost:5173`
- **Backend API:** `http://localhost:5103`
- **Docs da API (Scalar):** `http://localhost:5103/scalar/v1`

> Se você abrir `localhost:5103`, não está abrindo o React; está abrindo o backend.
> O site novo sempre deve ser aberto em `localhost:5173`.

## Rotas disponíveis
- `http://localhost:5173/discover`
- `http://localhost:5173/list`
- `http://localhost:5173/profile`
- `http://localhost:5173/catalog`
- `http://localhost:5173/admin`
- `http://localhost:5173/lab`

## Placeholders
- Anime: `public/placeholders/anime-no-image.svg`
- Perfil: `public/placeholders/profile-no-image.svg`


## Troubleshooting de versão antiga
Se abrir uma tela antiga (ex.: apenas login/cadastro sem as abas `discover/list/profile/catalog/admin/lab`), faça:

### Linux/macOS (bash)
```bash
cd src/Frontend/UserAnimeList.Web
rm -rf node_modules dist
npm install
npm run dev -- --host 0.0.0.0 --port 5173
```

### Windows PowerShell
```powershell
cd src/Frontend/UserAnimeList.Web
Remove-Item -Recurse -Force node_modules, dist
npm install
npm run dev -- --host 0.0.0.0 --port 5173
```

### Windows CMD
```cmd
cd src\Frontend\UserAnimeList.Web
rmdir /s /q node_modules
rmdir /s /q dist
npm install
npm run dev -- --host 0.0.0.0 --port 5173
```

Depois faça hard refresh no navegador (`Ctrl+Shift+R`).

### Se o `npm run dev` subir em outra porta
Com `strictPort: true`, se a `5173` estiver ocupada o Vite vai **falhar** ao iniciar (em vez de trocar para outra porta sem você perceber).

Nesse caso:
1. Feche o processo que está usando `5173`; ou
2. Rode em outra porta e abra exatamente essa porta no navegador:

```bash
npm run dev -- --port 5174
```


Checklist visual da versão nova:
- Topo com abas: `discover`, `list`, `profile`, `catalog`, `admin`, `lab`
- Texto de versão no topo: `Frontend ui-r3-2026-02-15`
