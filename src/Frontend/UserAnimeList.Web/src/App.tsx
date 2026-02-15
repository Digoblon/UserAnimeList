import { FormEvent, useEffect, useMemo, useState } from 'react';
import { ApiError, api } from './services/api';
import {
  AnimeEntryStatus,
  AnimeListSort,
  AnimeStatus,
  AnimeType,
  RequestAnimeJson,
  RequestAnimeListEntryJson,
  RequestChangePasswordJson,
  RequestRegisterUserJson,
  ResponseAnimeListsJson,
  ResponseShortAnimeJson,
  SortDirection,
  SourceType
} from './types/contracts';

type Tab = 'home' | 'profile' | 'list' | 'admin' | 'explorer';

const animePlaceholder = '/placeholders/anime-no-image.svg';
const profilePlaceholder = '/placeholders/profile-no-image.svg';

const tokenState = () => ({ accessToken: localStorage.getItem('accessToken'), refreshToken: localStorage.getItem('refreshToken') });

export default function App() {
  const [tab, setTab] = useState<Tab>('home');
  const [authOpen, setAuthOpen] = useState(false);
  const [accessToken, setAccessToken] = useState(tokenState().accessToken);
  const [refreshToken, setRefreshToken] = useState(tokenState().refreshToken);
  const [message, setMessage] = useState('');

  const [search, setSearch] = useState('');
  const [homeAnimes, setHomeAnimes] = useState<ResponseShortAnimeJson[]>([]);

  const [profile, setProfile] = useState<{ userName: string; email: string; imageUrl: string } | null>(null);
  const [myList, setMyList] = useState<ResponseAnimeListsJson>({ lists: [] });

  api.setToken(accessToken);

  const isAuthed = useMemo(() => Boolean(accessToken), [accessToken]);

  const run = async (fn: () => Promise<unknown>) => {
    try {
      await fn();
    } catch (err) {
      const e = err as ApiError;
      if (e.payload?.tokenIsExpired && refreshToken) {
        const next = await api.refreshToken({ refreshToken }).then((r) => r.data);
        saveTokens(next.accessToken, next.refreshToken);
      } else {
        setMessage(e.message);
      }
    }
  };

  const saveTokens = (a: string, r: string) => {
    localStorage.setItem('accessToken', a);
    localStorage.setItem('refreshToken', r);
    setAccessToken(a);
    setRefreshToken(r);
    api.setToken(a);
  };

  const logout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    setAccessToken(null);
    setRefreshToken(null);
    setProfile(null);
    setMyList({ lists: [] });
  };

  const loadHome = async (q = '') => {
    setMessage('');
    const data = await api.searchAnime({ query: q }).then((r) => r.data);
    setHomeAnimes(data.animes ?? []);
  };

  useEffect(() => {
    loadHome('').catch(() => loadHome('a').catch(() => setMessage('Não foi possível carregar home.')));
  }, []);

  const loadProfileAndList = async () => {
    if (!isAuthed) return;
    const p = await api.getUserProfile().then((r) => r.data);
    const l = await api.listMyAnimeList({ sortField: AnimeListSort.Name, sortDirection: SortDirection.Asc }).then((r) => r.data);
    setProfile(p);
    setMyList(l);
  };

  useEffect(() => {
    if (tab === 'profile' || tab === 'list') {
      run(loadProfileAndList);
    }
  }, [tab]);

  const submitLogin = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const fd = new FormData(e.currentTarget);
    const login = String(fd.get('login') ?? '');
    const password = String(fd.get('password') ?? '');
    await run(async () => {
      const data = await api.login({ login, password }).then((r) => r.data);
      saveTokens(data.tokens.accessToken, data.tokens.refreshToken);
      setAuthOpen(false);
      setTab('profile');
    });
  };

  const submitRegister = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const fd = new FormData(e.currentTarget);
    const payload: RequestRegisterUserJson = {
      userName: String(fd.get('userName') ?? ''),
      email: String(fd.get('email') ?? ''),
      password: String(fd.get('password') ?? ''),
      confirmPassword: String(fd.get('confirmPassword') ?? '')
    };
    await run(async () => {
      const data = await api.registerUser(payload).then((r) => r.data);
      saveTokens(data.tokens.accessToken, data.tokens.refreshToken);
      setAuthOpen(false);
      setTab('profile');
    });
  };

  const addToMyList = async (animeId: string) => run(async () => {
    const payload: RequestAnimeListEntryJson = { animeId, status: AnimeEntryStatus.PlanToWatch };
    await api.addAnimeListEntry(payload);
    setMessage('Adicionado à sua lista.');
  });

  const [animeJson, setAnimeJson] = useState(JSON.stringify({
    name: 'Novo Anime', synopsis: 'Descrição', episodes: 12, genres: [], studios: [], status: AnimeStatus.Airing, source: SourceType.Manga, type: AnimeType.Tv
  }, null, 2));
  const [animeImageFile, setAnimeImageFile] = useState<File | null>(null);
  const [userImageFile, setUserImageFile] = useState<File | null>(null);

  const parseAnimePayload = (): RequestAnimeJson => JSON.parse(animeJson) as RequestAnimeJson;

  return (
    <div className="app">
      <header>
        <h1>UserAnimeList</h1>
        <nav>
          {(['home', 'profile', 'list', 'admin', 'explorer'] as Tab[]).map((item) => (
            <button key={item} className={tab === item ? 'active' : ''} onClick={() => setTab(item)}>{item}</button>
          ))}
        </nav>
        <div>
          {!isAuthed ? <button onClick={() => setAuthOpen(true)}>Entrar / Cadastrar</button> : <button onClick={logout}>Sair</button>}
        </div>
      </header>

      {message && <p className="msg">{message}</p>}

      {tab === 'home' && (
        <section>
          <h2>Home</h2>
          <form onSubmit={(e) => { e.preventDefault(); run(() => loadHome(search)); }}>
            <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Buscar anime" />
            <button type="submit">Buscar</button>
          </form>
          <div className="grid">
            {homeAnimes.map((anime) => (
              <article key={anime.id} className="card">
                <img src={anime.imageUrl || animePlaceholder} onError={(e) => { (e.target as HTMLImageElement).src = animePlaceholder; }} />
                <h3>{anime.name}</h3>
                <p>Score: {anime.score ?? '-'}</p>
                <button disabled={!isAuthed} onClick={() => addToMyList(anime.id)}>Adicionar à lista</button>
              </article>
            ))}
          </div>
        </section>
      )}

      {tab === 'profile' && (
        <section>
          <h2>Perfil</h2>
          {!isAuthed ? <p>Faça login para ver o perfil.</p> : profile && (
            <div className="profile">
              <img src={profile.imageUrl || profilePlaceholder} onError={(e) => { (e.target as HTMLImageElement).src = profilePlaceholder; }} />
              <div><strong>{profile.userName}</strong><p>{profile.email}</p></div>
            </div>
          )}
          {isAuthed && (
            <div className="row">
              <input type="file" accept="image/*" onChange={(e) => setUserImageFile(e.target.files?.[0] ?? null)} />
              <button onClick={() => run(async () => {
                if (!userImageFile) return;
                await api.updateUserImage(userImageFile);
                await loadProfileAndList();
              })}>Atualizar imagem de perfil</button>
            </div>
          )}
        </section>
      )}

      {tab === 'list' && (
        <section>
          <h2>Minha lista</h2>
          {!isAuthed ? <p>Faça login para ver sua lista.</p> : (
            <ul>
              {myList.lists.map((x) => <li key={x.id}>{x.name} • {AnimeEntryStatus[x.status]} • {x.progress ?? 0}</li>)}
            </ul>
          )}
        </section>
      )}

      {tab === 'admin' && (
        <section>
          <h2>Admin (CRUD Anime)</h2>
          <textarea rows={12} value={animeJson} onChange={(e) => setAnimeJson(e.target.value)} />
          <div className="row">
            <button onClick={() => run(async () => { await api.registerAnime(parseAnimePayload()); setMessage('Anime criado'); })}>Criar anime</button>
            <button onClick={() => run(async () => {
              const id = prompt('ID anime para atualizar'); if (!id) return;
              await api.updateAnime(id, parseAnimePayload()); setMessage('Anime atualizado');
            })}>Atualizar anime</button>
            <button onClick={() => run(async () => {
              const id = prompt('ID anime para remover'); if (!id) return;
              await api.deleteAnime(id); setMessage('Anime removido');
            })}>Remover anime</button>
          </div>
          <div className="row">
            <input type="file" accept="image/*" onChange={(e) => setAnimeImageFile(e.target.files?.[0] ?? null)} />
            <button onClick={() => run(async () => {
              const id = prompt('ID anime para upload de imagem');
              if (!id || !animeImageFile) return;
              await api.updateAnimeImage(id, animeImageFile);
              setMessage('Imagem do anime atualizada');
            })}>Upload imagem anime</button>
            <button onClick={() => run(async () => {
              const id = prompt('ID anime para remover imagem');
              if (!id) return;
              await api.deleteAnimeImage(id);
              setMessage('Imagem do anime removida');
            })}>Remover imagem anime</button>
          </div>
        </section>
      )}

      {tab === 'explorer' && <EndpointExplorer isAuthed={isAuthed} refreshToken={refreshToken} saveTokens={saveTokens} />}

      {authOpen && (
        <div className="modal">
          <div className="modal-content">
            <h3>Entrar</h3>
            <form onSubmit={submitLogin}>
              <input name="login" placeholder="Usuário ou email" required />
              <input type="password" name="password" placeholder="Senha" required />
              <button type="submit">Entrar</button>
            </form>
            <h3>Cadastrar</h3>
            <form onSubmit={submitRegister}>
              <input name="userName" placeholder="Usuário" required />
              <input type="email" name="email" placeholder="Email" required />
              <input type="password" name="password" placeholder="Senha" required />
              <input type="password" name="confirmPassword" placeholder="Confirmar senha" required />
              <button type="submit">Cadastrar</button>
            </form>
            <button onClick={() => setAuthOpen(false)}>Fechar</button>
          </div>
        </div>
      )}
    </div>
  );
}

function EndpointExplorer({ isAuthed, refreshToken, saveTokens }: { isAuthed: boolean; refreshToken: string | null; saveTokens: (a: string, r: string) => void }) {
  const [out, setOut] = useState('');
  const [id, setId] = useState('');
  const [name, setName] = useState('');

  const exec = async (label: string, fn: () => Promise<unknown>) => {
    try {
      const value = await fn();
      setOut(`${label}\n${JSON.stringify(value, null, 2)}`);
    } catch (e) {
      setOut(`${label}\n${(e as Error).message}`);
    }
  };

  const demoChangePassword: RequestChangePasswordJson = { password: 'old', newPassword: 'newPassword123', confirmNewPassword: 'newPassword123' };

  return (
    <section>
      <h2>Endpoint Explorer (todos endpoints)</h2>
      <p>Use para validar rapidamente cada endpoint e payload da pasta Shared.</p>
      <div className="row"><input value={id} onChange={(e) => setId(e.target.value)} placeholder="id / userId" /><input value={name} onChange={(e) => setName(e.target.value)} placeholder="name/query" /></div>
      <div className="row wrap">
        <button onClick={() => exec('GET anime/{id}', () => api.getAnimeById(id))}>Anime by id</button>
        <button onClick={() => exec('POST genre/search', () => api.searchGenre({ name }))}>Genre search</button>
        <button onClick={() => exec('GET genre/{id}', () => api.getGenreById(id))}>Genre by id</button>
        <button onClick={() => exec('POST studio/search', () => api.searchStudio({ name }))}>Studio search</button>
        <button onClick={() => exec('GET studio/{id}', () => api.getStudioById(id))}>Studio by id</button>
        <button onClick={() => exec('GET animelist/list/{userId}', () => api.listUserAnimeList(id, { query: name }))}>User list by userId</button>
        {isAuthed && <>
          <button onClick={() => exec('GET animelist/{id}', () => api.getAnimeListEntryById(id))}>List entry by id</button>
          <button onClick={() => exec('PUT animelist/{id}', () => api.updateAnimeListEntry(id, { status: AnimeEntryStatus.Watching, progress: 1, score: 7 }))}>Update list entry</button>
          <button onClick={() => exec('DELETE animelist/{id}', () => api.deleteAnimeListEntry(id))}>Delete list entry</button>
          <button onClick={() => exec('PUT user', () => api.updateUser({ userName: 'updated-user', email: 'updated@email.com' }))}>Update user</button>
          <button onClick={() => exec('PUT user/change-password', () => api.changePassword(demoChangePassword).then((r) => { saveTokens(r.data.tokens.accessToken, r.data.tokens.refreshToken); return r; }))}>Change password</button>
          <button onClick={() => exec('DELETE user/me/image', () => api.deleteUserImage())}>Delete user image</button>
          <button onClick={() => exec('DELETE user/me', () => api.deleteMe())}>Delete me</button>
          <button onClick={() => exec('POST genre', () => api.registerGenre({ name: name || 'Genre', description: 'desc' }))}>Register genre</button>
          <button onClick={() => exec('PUT genre/{id}', () => api.updateGenre(id, { name: name || 'Genre up', description: 'updated' }))}>Update genre</button>
          <button onClick={() => exec('DELETE genre/{id}', () => api.deleteGenre(id))}>Delete genre</button>
          <button onClick={() => exec('POST studio', () => api.registerStudio({ name: name || 'Studio', description: 'desc' }))}>Register studio</button>
          <button onClick={() => exec('PUT studio/{id}', () => api.updateStudio(id, { name: name || 'Studio up', description: 'updated' }))}>Update studio</button>
          <button onClick={() => exec('DELETE studio/{id}', () => api.deleteStudio(id))}>Delete studio</button>
        </>}
        {refreshToken && <button onClick={() => exec('POST token/refresh-token', () => api.refreshToken({ refreshToken }).then((r) => { saveTokens(r.data.accessToken, r.data.refreshToken); return r; }))}>Refresh token</button>}
      </div>
      <pre>{out}</pre>
      <p>Endpoints de upload de imagem estão no painel Admin/Perfil, pois exigem seleção de arquivo.</p>
    </section>
  );
}
