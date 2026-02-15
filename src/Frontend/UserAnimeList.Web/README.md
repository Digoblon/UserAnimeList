# UserAnimeList Web

Frontend React + TypeScript + Vite para demonstrar a API do projeto UserAnimeList.

## Organização por páginas

- **Home**: catálogo inicial e busca de animes.
- **Perfil**: dados do usuário autenticado e a lista pessoal (acessada ao clicar em **Perfil** no header).
- **Modal de Autenticação**: login/cadastro aberto pelo botão **Entrar / Cadastrar**.

## Placeholders de imagem

Os placeholders foram criados em:

- `public/placeholders/anime-no-image.svg`
- `public/placeholders/profile-no-image.svg`

Você pode trocar esses arquivos diretamente para mudar as imagens padrão.

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
