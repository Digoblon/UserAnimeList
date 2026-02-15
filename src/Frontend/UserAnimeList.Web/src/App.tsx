import { useEffect, useMemo, useState, type FormEvent } from 'react';
import { AuthPanel } from './components/AuthPanel';
import { Header } from './components/Header';
import { MyListPanel } from './components/MyListPanel';
import { ProfilePanel } from './components/ProfilePanel';
import { SearchPanel } from './components/SearchPanel';
import { ApiError, api } from './services/api';
import type { Anime, AnimeListEntry, ListFilter, RegisterUserRequest, UserProfile } from './types/api';

const ACCESS_TOKEN_KEY = 'ual_access_token';
const REFRESH_TOKEN_KEY = 'ual_refresh_token';

const emptyRegisterData: RegisterUserRequest = {
  userName: '',
  email: '',
  password: '',
  confirmPassword: ''
};

export function App() {
  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [registerData, setRegisterData] = useState<RegisterUserRequest>(emptyRegisterData);

  const [accessToken, setAccessToken] = useState(localStorage.getItem(ACCESS_TOKEN_KEY) ?? '');
  const [refreshToken, setRefreshToken] = useState(localStorage.getItem(REFRESH_TOKEN_KEY) ?? '');

  const [query, setQuery] = useState('');
  const [foundAnime, setFoundAnime] = useState<Anime[]>([]);
  const [myList, setMyList] = useState<AnimeListEntry[]>([]);
  const [listFilter] = useState<ListFilter>({ sortField: 0, sortDirection: 0 });
  const [profile, setProfile] = useState<UserProfile | null>(null);

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const isLoggedIn = useMemo(() => accessToken.length > 10, [accessToken]);

  useEffect(() => {
    if (!isLoggedIn) {
      setProfile(null);
      setMyList([]);
      return;
    }

    void bootstrapUserArea();
  }, [isLoggedIn]);

  async function bootstrapUserArea() {
    setLoading(true);

    try {
      const token = await getValidToken();
      const [profileResponse, listResponse] = await Promise.all([
        api.getProfile(token),
        api.getMyList(token, listFilter)
      ]);

      setProfile(profileResponse);
      setMyList(listResponse.lists ?? []);
    } catch (error) {
      setMessage(handleApiFailure(error, 'Não foi possível carregar a área autenticada.'));
      logout(false);
    } finally {
      setLoading(false);
    }
  }

  function persistTokens(nextAccessToken: string, nextRefreshToken: string) {
    localStorage.setItem(ACCESS_TOKEN_KEY, nextAccessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, nextRefreshToken);
    setAccessToken(nextAccessToken);
    setRefreshToken(nextRefreshToken);
  }

  function logout(showMessage = true) {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    setAccessToken('');
    setRefreshToken('');
    setProfile(null);
    setMyList([]);
    if (showMessage) setMessage('Você saiu da sessão.');
  }

  async function getValidToken() {
    if (accessToken) return accessToken;

    if (!refreshToken) {
      throw new Error('Você precisa fazer login.');
    }

    const renewed = await api.refreshToken(refreshToken);
    persistTokens(renewed.accessToken, renewed.refreshToken);
    return renewed.accessToken;
  }

  async function withAuth<T>(operation: (token: string) => Promise<T>): Promise<T> {
    try {
      const token = await getValidToken();
      return await operation(token);
    } catch (error) {
      if (error instanceof ApiError && error.tokenIsExpired && refreshToken) {
        const renewed = await api.refreshToken(refreshToken);
        persistTokens(renewed.accessToken, renewed.refreshToken);
        return operation(renewed.accessToken);
      }

      throw error;
    }
  }

  async function handleLoginSubmit(event: FormEvent) {
    event.preventDefault();
    setLoading(true);
    setMessage('Realizando login...');

    try {
      const tokens = await api.login(login, password);
      persistTokens(tokens.accessToken, tokens.refreshToken);
      setLogin('');
      setPassword('');
      setMessage('Login realizado com sucesso.');
      await bootstrapUserArea();
    } catch (error) {
      setMessage(handleApiFailure(error, 'Falha no login. Verifique usuário/senha.'));
    } finally {
      setLoading(false);
    }
  }

  async function handleRegisterSubmit(event: FormEvent) {
    event.preventDefault();
    setLoading(true);

    try {
      await api.register(registerData);
      setRegisterData(emptyRegisterData);
      setMessage('Cadastro realizado. Agora faça login.');
    } catch (error) {
      setMessage(handleApiFailure(error, 'Não foi possível realizar cadastro.'));
    } finally {
      setLoading(false);
    }
  }

  async function handleSearchSubmit(event: FormEvent) {
    event.preventDefault();
    setLoading(true);

    try {
      const response = await api.searchAnime(query);
      setFoundAnime(response.animes ?? []);
      setMessage((response.animes ?? []).length ? 'Busca concluída com sucesso.' : 'Nenhum anime encontrado.');
    } catch (error) {
      setFoundAnime([]);
      setMessage(handleApiFailure(error, 'Busca indisponível no momento.'));
    } finally {
      setLoading(false);
    }
  }

  async function handleAddToList(animeId: string) {
    setLoading(true);

    try {
      await withAuth(async (token) => {
        await api.addToList(token, {
          animeId,
          status: 0,
          progress: 0,
          score: null
        });

        const list = await api.getMyList(token, listFilter);
        setMyList(list.lists ?? []);
      });

      setMessage('Anime adicionado à sua lista.');
    } catch (error) {
      setMessage(handleApiFailure(error, 'Não foi possível adicionar anime na lista.'));
    } finally {
      setLoading(false);
    }
  }

  async function handleUpdateEntry(entry: AnimeListEntry) {
    setLoading(true);

    try {
      await withAuth(async (token) => {
        await api.updateListEntry(token, entry.id, {
          status: entry.status,
          progress: entry.progress,
          score: entry.score
        });

        const list = await api.getMyList(token, listFilter);
        setMyList(list.lists ?? []);
      });

      setMessage('Item da lista atualizado.');
    } catch (error) {
      setMessage(handleApiFailure(error, 'Não foi possível atualizar item da lista.'));
    } finally {
      setLoading(false);
    }
  }

  async function handleDeleteEntry(entryId: string) {
    setLoading(true);

    try {
      await withAuth(async (token) => {
        await api.deleteListEntry(token, entryId);
        const list = await api.getMyList(token, listFilter);
        setMyList(list.lists ?? []);
      });

      setMessage('Item removido da sua lista.');
    } catch (error) {
      setMessage(handleApiFailure(error, 'Não foi possível remover item da lista.'));
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="page">
      <Header userName={profile?.userName} onLogout={() => logout(true)} />

      <AuthPanel
        login={login}
        password={password}
        registerData={registerData}
        loading={loading}
        onLoginSubmit={handleLoginSubmit}
        onRegisterSubmit={handleRegisterSubmit}
        onLoginChange={setLogin}
        onPasswordChange={setPassword}
        onRegisterChange={setRegisterData}
      />

      <ProfilePanel profile={profile} />

      <SearchPanel
        query={query}
        foundAnime={foundAnime}
        loading={loading}
        canAdd={isLoggedIn}
        onQueryChange={setQuery}
        onSearchSubmit={handleSearchSubmit}
        onAdd={handleAddToList}
      />

      <MyListPanel entries={myList} loading={loading} onUpdate={handleUpdateEntry} onDelete={handleDeleteEntry} />

      {message && <footer className="toast">{message}</footer>}
    </main>
  );
}

function handleApiFailure(error: unknown, fallback: string): string {
  if (error instanceof Error && error.message) {
    return error.message;
  }

  return fallback;
}
