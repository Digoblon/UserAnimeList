import { useEffect, useMemo, useState, type FormEvent } from 'react';
import { AuthModal } from './components/AuthModal';
import { Header } from './components/Header';
import { HomePage } from './components/HomePage';
import { ProfilePage } from './components/ProfilePage';
import { ApiError, api } from './services/api';
import type { Anime, AnimeListEntry, RegisterUserRequest, UserProfile } from './types/api';

const ACCESS_TOKEN_KEY = 'ual_access_token';
const REFRESH_TOKEN_KEY = 'ual_refresh_token';

const emptyRegisterData: RegisterUserRequest = {
  userName: '',
  email: '',
  password: '',
  confirmPassword: ''
};

type Page = 'home' | 'profile';

export function App() {
  const [page, setPage] = useState<Page>('home');
  const [authOpen, setAuthOpen] = useState(false);

  const [login, setLogin] = useState('');
  const [password, setPassword] = useState('');
  const [registerData, setRegisterData] = useState<RegisterUserRequest>(emptyRegisterData);

  const [accessToken, setAccessToken] = useState(localStorage.getItem(ACCESS_TOKEN_KEY) ?? '');
  const [refreshToken, setRefreshToken] = useState(localStorage.getItem(REFRESH_TOKEN_KEY) ?? '');

  const [query, setQuery] = useState('');
  const [homeAnimes, setHomeAnimes] = useState<Anime[]>([]);
  const [myList, setMyList] = useState<AnimeListEntry[]>([]);
  const [profile, setProfile] = useState<UserProfile | null>(null);

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const isLoggedIn = useMemo(() => accessToken.length > 10, [accessToken]);

  useEffect(() => {
    void loadHomeInitialCatalog();
  }, []);

  useEffect(() => {
    if (page === 'profile' && isLoggedIn) {
      void loadProfileAndList();
    }
  }, [page, isLoggedIn]);

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
    setPage('home');
    if (showMessage) setMessage('Você saiu da sessão.');
  }

  async function getValidToken() {
    if (accessToken) return accessToken;

    if (!refreshToken) throw new Error('Você precisa fazer login.');

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

  async function loadHomeInitialCatalog() {
    setLoading(true);
    try {
      const first = await api.searchAnime('');
      let items = first.animes ?? [];
      if (!items.length) {
        const fallback = await api.searchAnime('a');
        items = fallback.animes ?? [];
      }
      setHomeAnimes(items.slice(0, 12));
    } catch {
      setHomeAnimes([]);
      setMessage('Não foi possível carregar catálogo inicial.');
    } finally {
      setLoading(false);
    }
  }

  async function loadProfileAndList() {
    setLoading(true);
    try {
      await withAuth(async (token) => {
        const [profileResponse, listResponse] = await Promise.all([
          api.getProfile(token),
          api.getMyList(token)
        ]);

        setProfile(profileResponse);
        setMyList(listResponse.lists ?? []);
      });
    } catch (error) {
      setMessage(resolveError(error, 'Não foi possível carregar perfil e lista.'));
      logout(false);
      setAuthOpen(true);
    } finally {
      setLoading(false);
    }
  }

  async function handleLoginSubmit(event: FormEvent) {
    event.preventDefault();
    setLoading(true);
    try {
      const tokens = await api.login(login, password);
      persistTokens(tokens.accessToken, tokens.refreshToken);
      setLogin('');
      setPassword('');
      setMessage('Login realizado com sucesso.');
      setAuthOpen(false);
      setPage('profile');
      await loadProfileAndList();
    } catch (error) {
      setMessage(resolveError(error, 'Falha no login.'));
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
      setMessage('Cadastro concluído. Faça login para continuar.');
    } catch (error) {
      setMessage(resolveError(error, 'Não foi possível realizar cadastro.'));
    } finally {
      setLoading(false);
    }
  }

  async function handleSearchSubmit(event: FormEvent) {
    event.preventDefault();
    setLoading(true);
    try {
      const response = await api.searchAnime(query);
      const items = response.animes ?? [];
      setHomeAnimes(items);
      setMessage(items.length ? 'Busca concluída.' : 'Nenhum anime encontrado.');
    } catch (error) {
      setMessage(resolveError(error, 'Busca indisponível.'));
    } finally {
      setLoading(false);
    }
  }

  async function handleAddToList(animeId: string) {
    setLoading(true);
    try {
      await withAuth(async (token) => {
        await api.addToList(token, { animeId, status: 0, progress: 0, score: null });
      });
      setMessage('Anime adicionado na sua lista.');
    } catch (error) {
      setMessage(resolveError(error, 'Não foi possível adicionar anime.'));
      if (!isLoggedIn) setAuthOpen(true);
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
          score: entry.score,
          progress: entry.progress
        });

        const list = await api.getMyList(token);
        setMyList(list.lists ?? []);
      });
      setMessage('Item atualizado.');
    } catch (error) {
      setMessage(resolveError(error, 'Não foi possível atualizar item.'));
    } finally {
      setLoading(false);
    }
  }

  async function handleDeleteEntry(entryId: string) {
    setLoading(true);
    try {
      await withAuth(async (token) => {
        await api.deleteListEntry(token, entryId);
        const list = await api.getMyList(token);
        setMyList(list.lists ?? []);
      });
      setMessage('Item removido da lista.');
    } catch (error) {
      setMessage(resolveError(error, 'Não foi possível remover item.'));
    } finally {
      setLoading(false);
    }
  }

  function openProfile() {
    if (!isLoggedIn) {
      setAuthOpen(true);
      setMessage('Faça login para acessar o perfil.');
      return;
    }

    setPage('profile');
  }

  return (
    <main className="page">
      <Header
        userName={profile?.userName}
        onHome={() => setPage('home')}
        onProfile={openProfile}
        onAuth={() => setAuthOpen(true)}
        onLogout={() => logout(true)}
      />

      {page === 'home' && (
        <HomePage
          query={query}
          loading={loading}
          animes={homeAnimes}
          canAdd={isLoggedIn}
          onQueryChange={setQuery}
          onSearch={handleSearchSubmit}
          onAdd={handleAddToList}
        />
      )}

      {page === 'profile' && (
        <ProfilePage profile={profile} entries={myList} loading={loading} onUpdate={handleUpdateEntry} onDelete={handleDeleteEntry} />
      )}

      <AuthModal
        isOpen={authOpen}
        loading={loading}
        login={login}
        password={password}
        registerData={registerData}
        onClose={() => setAuthOpen(false)}
        onLoginSubmit={handleLoginSubmit}
        onRegisterSubmit={handleRegisterSubmit}
        onLoginChange={setLogin}
        onPasswordChange={setPassword}
        onRegisterChange={setRegisterData}
      />

      {message && <footer className="toast">{message}</footer>}
    </main>
  );
}

function resolveError(error: unknown, fallback: string) {
  if (error instanceof Error && error.message) return error.message;
  return fallback;
}
