# Análise do Frontend - UserAnimeList.Web

## O que o frontend faz

Aplicação web em React + TypeScript (Vite) que consome a API UserAnimeList para:
- autenticação (login/cadastro)
- exibição de perfil
- busca de animes
- gerenciamento da lista pessoal de animes (adicionar, atualizar status/progresso/nota e remover)

## Como ele é (arquitetura e UX)

- **Estrutura principal**: um único fluxo central em `App.tsx` controla estado, autenticação e ações assíncronas.
- **Componentização por domínio**:
  - `Header` (contexto geral e sessão)
  - `AuthPanel` (login e cadastro)
  - `ProfilePanel` (dados do usuário)
  - `SearchPanel` (busca e cards de anime)
  - `MyListPanel` (itens da lista pessoal)
- **Cliente HTTP dedicado** em `src/services/api.ts`, com tratamento de erro e suporte a refresh token.
- **Sessão persistida** em `localStorage` com `accessToken` e `refreshToken`.
- **Visual**: tema escuro com cards, grid responsivo para animes e feedback via toast.

## Observações técnicas

- Existe um componente `HomePage.tsx`, mas atualmente o fluxo renderizado usa `SearchPanel` diretamente no `App.tsx`.
- Proxy de desenvolvimento configurado para `/api -> http://localhost:5103`.

