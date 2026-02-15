# UserAnimeList.Web (novo frontend)

Frontend reconstruído do zero para cobrir **todos os endpoints** da API.

## Princípios desta versão
- Uso dos contratos baseados na pasta `src/Shared/UserAnimeList.Communication`.
- Inclui todos os requests, responses e enums da Shared em `src/types/contracts.ts`.
- Inclui chamadas para todos os endpoints existentes nos controllers da API.

## Rodar
```bash
npm install
npm run dev
```
