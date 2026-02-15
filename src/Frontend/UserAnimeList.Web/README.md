# UserAnimeList Web

Frontend React + TypeScript + Vite para demonstrar a API do projeto UserAnimeList.

## Arquitetura do frontend

O frontend foi organizado por módulos de negócio, alinhando os contextos da API:

- **Auth**: login, cadastro e persistência de sessão.
- **User**: carregamento do perfil autenticado.
- **Anime**: busca de animes.
- **AnimeList**: adicionar, atualizar e remover entradas da lista pessoal.

### Estrutura principal

- `src/App.tsx`: orquestra estado global da UI e fluxo autenticado.
- `src/services/api.ts`: cliente HTTP e funções de integração com endpoints da API.
- `src/components/*`: componentes de apresentação por contexto (AuthPanel, SearchPanel, MyListPanel, ProfilePanel, Header).
- `src/types/api.ts`: contratos e tipos dos payloads/respostas.

## Fluxo de uso

1. Cadastrar (`POST /user`) ou fazer login (`POST /login`)
2. Carregar perfil (`GET /user`) e lista (`GET /animelist/me/list`)
3. Buscar anime (`POST /anime/search`)
4. Adicionar na lista (`POST /animelist`)
5. Atualizar/remover item (`PUT/DELETE /animelist/{id}`)
6. Renovar sessão quando necessário (`POST /token/refresh-token`)

## Execução

```bash
npm install
npm run dev
```

App em `http://localhost:5173` com proxy `/api -> http://localhost:5103`.

## Build

```bash
npm run build
```
