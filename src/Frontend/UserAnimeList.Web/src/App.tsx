import { FormEvent, useEffect, useMemo, useState } from 'react';
import { ApiError, api } from './services/api';
import {
  AnimeEntryStatus,
  AnimeListSort,
  AnimeStatus,
  AnimeType,
  RequestAnimeJson,
  RequestAnimeListEntryFilterJson,
  RequestAnimeListEntryJson,
  RequestChangePasswordJson,
  RequestRegisterUserJson,
  RequestUpdateAnimeListEntryJson,
  RequestUpdateUserJson,
  ResponseAnimeJson,
  ResponseAnimeListEntryJson,
  ResponseAnimeListsJson,
  ResponseShortAnimeJson,
  SortDirection,
  SourceType
} from './types/contracts';

type Tab = 'discover' | 'list' | 'profile' | 'catalog' | 'admin' | 'lab';

const animePlaceholder = '/placeholders/anime-no-image.svg';
const profilePlaceholder = '/placeholders/profile-no-image.svg';
const UI_VERSION = 'ui-r3-2026-02-15';

const tokenState = () => ({
  accessToken: localStorage.getItem('accessToken'),
  refreshToken: localStorage.getItem('refreshToken')
});

const enumOptions = (input: object) =>
  Object.keys(input).filter((key) => Number.isNaN(Number(key)));

function enumValue(input: Record<string, string | number>, key: string): number {
  return Number(input[key]);
}

const tabToPath: Record<Tab, string> = {
  discover: '/discover',
  list: '/list',
  profile: '/profile',
  catalog: '/catalog',
  admin: '/admin',
  lab: '/lab'
};

const pathToTab = (path: string): Tab => {
  const normalized = path.toLowerCase().replace(/\/+$/, '') || '/discover';
  const found = (Object.entries(tabToPath) as [Tab, string][]).find(([, p]) => p === normalized);
  return found?.[0] ?? 'discover';
};

export default function App() {
  const [tab, setTab] = useState<Tab>(() => pathToTab(window.location.pathname));
  const [authOpen, setAuthOpen] = useState(false);
  const [accessToken, setAccessToken] = useState(tokenState().accessToken);
  const [refreshToken, setRefreshToken] = useState(tokenState().refreshToken);
  const [alert, setAlert] = useState('');

  const [discoverQuery, setDiscoverQuery] = useState('');
  const [discoverList, setDiscoverList] = useState<ResponseShortAnimeJson[]>([]);

  const [profile, setProfile] = useState<{ userName: string; email: string; imageUrl: string } | null>(null);
  const [myList, setMyList] = useState<ResponseAnimeListsJson>({ lists: [] });
  const [myListFilters, setMyListFilters] = useState<RequestAnimeListEntryFilterJson>({
    sortField: AnimeListSort.Name,
    sortDirection: SortDirection.Asc
  });

  const [selectedListEntryId, setSelectedListEntryId] = useState('');
  const [selectedListEntry, setSelectedListEntry] = useState<ResponseAnimeListEntryJson | null>(null);

  const [selectedAnimeId, setSelectedAnimeId] = useState('');
  const [selectedAnime, setSelectedAnime] = useState<ResponseAnimeJson | null>(null);

  const [animePayload, setAnimePayload] = useState<string>(JSON.stringify({
    name: 'New Anime',
    synopsis: 'A new anime from admin panel.',
    episodes: 12,
    genres: [],
    studios: [],
    status: AnimeStatus.Airing,
    source: SourceType.Manga,
    type: AnimeType.Tv,
    airedFrom: '2026-01-01',
    airedUntil: '2026-03-31'
  } satisfies RequestAnimeJson, null, 2));

  const [animeImageFile, setAnimeImageFile] = useState<File | null>(null);
  const [userImageFile, setUserImageFile] = useState<File | null>(null);
  const [targetAnimeId, setTargetAnimeId] = useState('');

  const [genreName, setGenreName] = useState('');
  const [genreDescription, setGenreDescription] = useState('');
  const [genreId, setGenreId] = useState('');

  const [studioName, setStudioName] = useState('');
  const [studioDescription, setStudioDescription] = useState('');
  const [studioId, setStudioId] = useState('');

  const [externalUserId, setExternalUserId] = useState('');
  const [externalListResult, setExternalListResult] = useState<ResponseAnimeListsJson | null>(null);

  const [profileUpdate, setProfileUpdate] = useState<RequestUpdateUserJson>({ userName: '', email: '' });
  const [passwordForm, setPasswordForm] = useState<RequestChangePasswordJson>({
    password: '',
    newPassword: '',
    confirmNewPassword: ''
  });

  api.setToken(accessToken);
  const isAuthed = useMemo(() => Boolean(accessToken), [accessToken]);

  const saveTokens = (newAccess: string, newRefresh: string) => {
    localStorage.setItem('accessToken', newAccess);
    localStorage.setItem('refreshToken', newRefresh);
    setAccessToken(newAccess);
    setRefreshToken(newRefresh);
    api.setToken(newAccess);
  };

  const logout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    setAccessToken(null);
    setRefreshToken(null);
    setProfile(null);
    setMyList({ lists: [] });
    setAlert('Sessão encerrada.');
  };

  const run = async (work: () => Promise<void>) => {
    try {
      await work();
    } catch (err) {
      const error = err as ApiError;
      if (error.payload?.tokenIsExpired && refreshToken) {
        const refreshed = await api.refreshToken({ refreshToken }).then((res) => res.data);
        saveTokens(refreshed.accessToken, refreshed.refreshToken);
        setAlert('Token renovado automaticamente. Execute novamente a ação.');
        return;
      }
      setAlert(error.message || 'Erro inesperado.');
    }
  };

  const loadDiscover = async (query = '') => {
    const response = await api.searchAnime({ query }).then((res) => res.data);
    setDiscoverList(response.animes ?? []);
  };

  const loadProfile = async () => {
    if (!isAuthed) return;
    const data = await api.getUserProfile().then((res) => res.data);
    setProfile(data);
    setProfileUpdate({ userName: data.userName, email: data.email });
  };

  const loadMyList = async () => {
    if (!isAuthed) return;
    const data = await api.listMyAnimeList(myListFilters).then((res) => res.data);
    setMyList(data);
  };

  useEffect(() => {
    const initialTab = pathToTab(window.location.pathname);
    if (window.location.pathname === '/') {
      window.history.replaceState({}, '', tabToPath[initialTab]);
    }

    const onPopState = () => setTab(pathToTab(window.location.pathname));
    window.addEventListener('popstate', onPopState);
    return () => window.removeEventListener('popstate', onPopState);
  }, []);

  const goToTab = (nextTab: Tab) => {
    setTab(nextTab);
    const nextPath = tabToPath[nextTab];
    if (window.location.pathname !== nextPath) {
      window.history.pushState({}, '', nextPath);
    }
  };

  useEffect(() => {
    run(async () => {
      await loadDiscover('');
    });
  }, []);

  useEffect(() => {
    if (tab === 'profile') run(loadProfile);
    if (tab === 'list') run(loadMyList);
  }, [tab]);

  const onLogin = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const fd = new FormData(e.currentTarget);
    const login = String(fd.get('login') ?? '');
    const password = String(fd.get('password') ?? '');

    await run(async () => {
      const response = await api.login({ login, password }).then((res) => res.data);
      saveTokens(response.tokens.accessToken, response.tokens.refreshToken);
      setAuthOpen(false);
      goToTab('profile');
      setAlert(`Bem-vindo, ${response.userName}!`);
      await loadProfile();
    });
  };

  const onRegister = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const fd = new FormData(e.currentTarget);
    const payload: RequestRegisterUserJson = {
      userName: String(fd.get('userName') ?? ''),
      email: String(fd.get('email') ?? ''),
      password: String(fd.get('password') ?? ''),
      confirmPassword: String(fd.get('confirmPassword') ?? '')
    };

    await run(async () => {
      const response = await api.registerUser(payload).then((res) => res.data);
      saveTokens(response.tokens.accessToken, response.tokens.refreshToken);
      setAuthOpen(false);
      goToTab('profile');
      setAlert(`Conta criada para ${response.userName}.`);
      await loadProfile();
    });
  };

  const parseAnimePayload = () => JSON.parse(animePayload) as RequestAnimeJson;

  const onAddToList = (animeId: string) => run(async () => {
    const payload: RequestAnimeListEntryJson = {
      animeId,
      status: AnimeEntryStatus.PlanToWatch,
      progress: 0
    };
    await api.addAnimeListEntry(payload);
    setAlert('Anime adicionado à sua lista.');
    if (tab === 'list') await loadMyList();
  });

  const onUpdateEntryQuick = (entryId: string, status: AnimeEntryStatus) => run(async () => {
    const payload: RequestUpdateAnimeListEntryJson = {
      status,
      progress: status === AnimeEntryStatus.Completed ? 12 : 0,
      score: status === AnimeEntryStatus.Completed ? 8 : undefined
    };
    await api.updateAnimeListEntry(entryId, payload);
    await loadMyList();
    setAlert('Entrada atualizada.');
  });

  const onDeleteEntry = (entryId: string) => run(async () => {
    await api.deleteAnimeListEntry(entryId);
    await loadMyList();
    setAlert('Entrada removida da lista.');
  });

  return (
    <div className="app-shell">
      <header className="topbar">
        <div>
          <p className="brand-kicker">Inspired by MyAnimeList</p>
          <h1>UserAnimeList</h1>
          <p className="ui-version">Frontend {UI_VERSION}</p>
        </div>
        <nav className="tabs">
          {(['discover', 'list', 'profile', 'catalog', 'admin', 'lab'] as Tab[]).map((item) => (
            <button key={item} className={tab === item ? 'active' : ''} onClick={() => goToTab(item)}>
              {item}
            </button>
          ))}
        </nav>
        <div className="auth-actions">
          {!isAuthed ? (
            <button onClick={() => setAuthOpen(true)}>Entrar / Cadastrar</button>
          ) : (
            <button onClick={logout}>Sair</button>
          )}
        </div>
      </header>

      {alert && <div className="alert">{alert}</div>}

      {tab === 'discover' && (
        <section className="panel">
          <div className="panel-header">
            <h2>Discover</h2>
            <p>Busca pública de animes usando <code>POST /anime/search</code>.</p>
          </div>
          <form
            className="search-row"
            onSubmit={(e) => {
              e.preventDefault();
              run(async () => loadDiscover(discoverQuery));
            }}
          >
            <input
              value={discoverQuery}
              onChange={(e) => setDiscoverQuery(e.target.value)}
              placeholder="Ex: fullmetal, one piece..."
            />
            <button type="submit">Buscar</button>
          </form>

          <div className="anime-grid">
            {discoverList.map((anime) => (
              <article key={anime.id} className="anime-card">
                <img
                  src={anime.imageUrl || animePlaceholder}
                  alt={anime.name}
                  onError={(e) => {
                    (e.target as HTMLImageElement).src = animePlaceholder;
                  }}
                />
                <div>
                  <h3>{anime.name}</h3>
                  <p>Score: {anime.score ?? '-'}</p>
                  <p>{AnimeType[anime.type]} • {AnimeStatus[anime.status]}</p>
                </div>
                <div className="row-end">
                  <button disabled={!isAuthed} onClick={() => onAddToList(anime.id)}>
                    + Minha Lista
                  </button>
                  <button
                    onClick={() =>
                      run(async () => {
                        const full = await api.getAnimeById(anime.id).then((res) => res.data);
                        setSelectedAnime(full);
                        setSelectedAnimeId(anime.id);
                        setAlert(`Detalhes carregados para ${full.name}.`);
                      })
                    }
                  >
                    Detalhes
                  </button>
                </div>
              </article>
            ))}
          </div>

          {selectedAnime && (
            <div className="detail-box">
              <h3>{selectedAnime.name}</h3>
              <p>{selectedAnime.synopsis || 'Sem sinopse.'}</p>
              <p>Episódios: {selectedAnime.episodes ?? '?'}</p>
              <p>Source: {SourceType[selectedAnime.source]} • Status: {AnimeStatus[selectedAnime.status]}</p>
            </div>
          )}
        </section>
      )}

      {tab === 'list' && (
        <section className="panel">
          <div className="panel-header">
            <h2>Minha Lista</h2>
            <p>Gerencia <code>/animelist/me/list</code>, <code>/animelist/{'{id}'}</code>, update e delete.</p>
          </div>

          <div className="filter-grid">
            <input
              placeholder="Filtrar por nome"
              value={myListFilters.query ?? ''}
              onChange={(e) => setMyListFilters((prev) => ({ ...prev, query: e.target.value || undefined }))}
            />
            <select
              value={myListFilters.status ?? ''}
              onChange={(e) => setMyListFilters((prev) => ({ ...prev, status: e.target.value === '' ? undefined : Number(e.target.value) as AnimeEntryStatus }))}
            >
              <option value="">Todos status</option>
              {enumOptions(AnimeEntryStatus).map((name) => (
                <option key={name} value={enumValue(AnimeEntryStatus, name)}>{name}</option>
              ))}
            </select>
            <select
              value={myListFilters.sortField ?? AnimeListSort.Name}
              onChange={(e) => setMyListFilters((prev) => ({ ...prev, sortField: Number(e.target.value) as AnimeListSort }))}
            >
              {enumOptions(AnimeListSort).map((name) => (
                <option key={name} value={enumValue(AnimeListSort, name)}>{name}</option>
              ))}
            </select>
            <select
              value={myListFilters.sortDirection ?? SortDirection.Asc}
              onChange={(e) => setMyListFilters((prev) => ({ ...prev, sortDirection: Number(e.target.value) as SortDirection }))}
            >
              {enumOptions(SortDirection).map((name) => (
                <option key={name} value={enumValue(SortDirection, name)}>{name}</option>
              ))}
            </select>
            <button onClick={() => run(loadMyList)}>Aplicar filtros</button>
          </div>

          {!isAuthed ? (
            <p>Faça login para acessar sua lista.</p>
          ) : (
            <div className="list-grid">
              {myList.lists.map((entry) => (
                <article className="list-card" key={entry.id}>
                  <h3>{entry.name}</h3>
                  <p>Status: {AnimeEntryStatus[entry.status]}</p>
                  <p>Score: {entry.score ?? '-'} • Progresso: {entry.progress ?? 0}</p>
                  <div className="row-wrap">
                    <button onClick={() => onUpdateEntryQuick(entry.id, AnimeEntryStatus.Watching)}>Watching</button>
                    <button onClick={() => onUpdateEntryQuick(entry.id, AnimeEntryStatus.Completed)}>Completed</button>
                    <button onClick={() => onDeleteEntry(entry.id)}>Remover</button>
                    <button
                      onClick={() => run(async () => {
                        const full = await api.getAnimeListEntryById(entry.id).then((res) => res.data);
                        setSelectedListEntryId(entry.id);
                        setSelectedListEntry(full);
                      })}
                    >
                      Ver JSON
                    </button>
                  </div>
                </article>
              ))}
            </div>
          )}

          {selectedListEntry && (
            <pre>Entry {selectedListEntryId}\n{JSON.stringify(selectedListEntry, null, 2)}</pre>
          )}
        </section>
      )}

      {tab === 'profile' && (
        <section className="panel">
          <div className="panel-header">
            <h2>Perfil</h2>
            <p>Integra endpoints de <code>/user</code>.</p>
          </div>

          {!isAuthed ? (
            <p>Faça login para acessar o perfil.</p>
          ) : (
            <>
              {profile && (
                <div className="profile-box">
                  <img
                    src={profile.imageUrl || profilePlaceholder}
                    alt={profile.userName}
                    onError={(e) => {
                      (e.target as HTMLImageElement).src = profilePlaceholder;
                    }}
                  />
                  <div>
                    <h3>{profile.userName}</h3>
                    <p>{profile.email}</p>
                  </div>
                </div>
              )}

              <div className="form-stack">
                <h3>Atualizar Perfil</h3>
                <input
                  value={profileUpdate.userName}
                  onChange={(e) => setProfileUpdate((prev) => ({ ...prev, userName: e.target.value }))}
                  placeholder="username"
                />
                <input
                  value={profileUpdate.email}
                  onChange={(e) => setProfileUpdate((prev) => ({ ...prev, email: e.target.value }))}
                  placeholder="email"
                />
                <button onClick={() => run(async () => {
                  await api.updateUser(profileUpdate);
                  await loadProfile();
                  setAlert('Perfil atualizado.');
                })}>Salvar Perfil</button>
              </div>

              <div className="form-stack">
                <h3>Imagem de Perfil</h3>
                <input type="file" accept="image/*" onChange={(e) => setUserImageFile(e.target.files?.[0] ?? null)} />
                <div className="row-wrap">
                  <button onClick={() => run(async () => {
                    if (!userImageFile) return;
                    await api.updateUserImage(userImageFile);
                    await loadProfile();
                    setAlert('Imagem de perfil atualizada.');
                  })}>Upload imagem</button>
                  <button onClick={() => run(async () => {
                    await api.deleteUserImage();
                    await loadProfile();
                    setAlert('Imagem de perfil removida.');
                  })}>Remover imagem</button>
                </div>
              </div>

              <div className="form-stack">
                <h3>Alterar Senha</h3>
                <input type="password" placeholder="Senha atual" value={passwordForm.password} onChange={(e) => setPasswordForm((prev) => ({ ...prev, password: e.target.value }))} />
                <input type="password" placeholder="Nova senha" value={passwordForm.newPassword} onChange={(e) => setPasswordForm((prev) => ({ ...prev, newPassword: e.target.value }))} />
                <input type="password" placeholder="Confirmar nova senha" value={passwordForm.confirmNewPassword} onChange={(e) => setPasswordForm((prev) => ({ ...prev, confirmNewPassword: e.target.value }))} />
                <button onClick={() => run(async () => {
                  const response = await api.changePassword(passwordForm).then((res) => res.data);
                  saveTokens(response.tokens.accessToken, response.tokens.refreshToken);
                  setAlert('Senha alterada e sessão renovada.');
                })}>Alterar senha</button>
              </div>
            </>
          )}
        </section>
      )}

      {tab === 'catalog' && (
        <section className="panel">
          <div className="panel-header">
            <h2>Catálogo (Genre / Studio)</h2>
            <p>CRUD de gênero e estúdio com os contratos Shared.</p>
          </div>

          <div className="catalog-grid">
            <div className="form-stack">
              <h3>Genre</h3>
              <input placeholder="genre id" value={genreId} onChange={(e) => setGenreId(e.target.value)} />
              <input placeholder="name" value={genreName} onChange={(e) => setGenreName(e.target.value)} />
              <textarea rows={3} placeholder="description" value={genreDescription} onChange={(e) => setGenreDescription(e.target.value)} />
              <div className="row-wrap">
                <button onClick={() => run(async () => { await api.registerGenre({ name: genreName, description: genreDescription }); setAlert('Genre criado.'); })}>Criar</button>
                <button onClick={() => run(async () => { await api.updateGenre(genreId, { name: genreName, description: genreDescription }); setAlert('Genre atualizado.'); })}>Atualizar</button>
                <button onClick={() => run(async () => { await api.deleteGenre(genreId); setAlert('Genre removido.'); })}>Remover</button>
                <button onClick={() => run(async () => { const g = await api.getGenreById(genreId).then((res) => res.data); setGenreName(g.name); setGenreDescription(g.description); setAlert(`Genre ${g.name} carregado.`); })}>Buscar por ID</button>
                <button onClick={() => run(async () => { const r = await api.searchGenre({ name: genreName }).then((res) => res.data); setAlert(`Genres encontrados: ${r.genres.length}`); })}>Search by Name</button>
              </div>
            </div>

            <div className="form-stack">
              <h3>Studio</h3>
              <input placeholder="studio id" value={studioId} onChange={(e) => setStudioId(e.target.value)} />
              <input placeholder="name" value={studioName} onChange={(e) => setStudioName(e.target.value)} />
              <textarea rows={3} placeholder="description" value={studioDescription} onChange={(e) => setStudioDescription(e.target.value)} />
              <div className="row-wrap">
                <button onClick={() => run(async () => { await api.registerStudio({ name: studioName, description: studioDescription }); setAlert('Studio criado.'); })}>Criar</button>
                <button onClick={() => run(async () => { await api.updateStudio(studioId, { name: studioName, description: studioDescription }); setAlert('Studio atualizado.'); })}>Atualizar</button>
                <button onClick={() => run(async () => { await api.deleteStudio(studioId); setAlert('Studio removido.'); })}>Remover</button>
                <button onClick={() => run(async () => { const s = await api.getStudioById(studioId).then((res) => res.data); setStudioName(s.name); setStudioDescription(s.description); setAlert(`Studio ${s.name} carregado.`); })}>Buscar por ID</button>
                <button onClick={() => run(async () => { const r = await api.searchStudio({ name: studioName }).then((res) => res.data); setAlert(`Studios encontrados: ${r.studios.length}`); })}>Search by Name</button>
              </div>
            </div>
          </div>
        </section>
      )}

      {tab === 'admin' && (
        <section className="panel">
          <div className="panel-header">
            <h2>Admin Anime</h2>
            <p>Endpoints admin de <code>/anime</code> incluindo imagem.</p>
          </div>
          <textarea rows={12} value={animePayload} onChange={(e) => setAnimePayload(e.target.value)} />
          <input placeholder="anime id target" value={targetAnimeId} onChange={(e) => setTargetAnimeId(e.target.value)} />
          <div className="row-wrap">
            <button onClick={() => run(async () => { await api.registerAnime(parseAnimePayload()); setAlert('Anime criado.'); })}>Criar Anime</button>
            <button onClick={() => run(async () => { await api.updateAnime(targetAnimeId, parseAnimePayload()); setAlert('Anime atualizado.'); })}>Atualizar Anime</button>
            <button onClick={() => run(async () => { await api.deleteAnime(targetAnimeId); setAlert('Anime removido.'); })}>Remover Anime</button>
            <button onClick={() => run(async () => { const full = await api.getAnimeById(targetAnimeId).then((res) => res.data); setSelectedAnime(full); setAlert(`Anime ${full.name} carregado por ID.`); })}>Buscar por ID</button>
          </div>

          <div className="row-wrap">
            <input type="file" accept="image/*" onChange={(e) => setAnimeImageFile(e.target.files?.[0] ?? null)} />
            <button onClick={() => run(async () => {
              if (!animeImageFile) return;
              await api.updateAnimeImage(targetAnimeId, animeImageFile);
              setAlert('Imagem do anime atualizada.');
            })}>Upload imagem anime</button>
            <button onClick={() => run(async () => {
              await api.deleteAnimeImage(targetAnimeId);
              setAlert('Imagem do anime removida.');
            })}>Remover imagem anime</button>
          </div>
        </section>
      )}

      {tab === 'lab' && (
        <section className="panel">
          <div className="panel-header">
            <h2>API Lab</h2>
            <p>Fluxos auxiliares: refresh token, lista pública por usuário e soft delete da conta.</p>
          </div>

          <div className="form-stack">
            <h3>Refresh token</h3>
            <button
              disabled={!refreshToken}
              onClick={() => run(async () => {
                if (!refreshToken) return;
                const tokens = await api.refreshToken({ refreshToken }).then((res) => res.data);
                saveTokens(tokens.accessToken, tokens.refreshToken);
                setAlert('Refresh token executado com sucesso.');
              })}
            >
              Refresh Session
            </button>
          </div>

          <div className="form-stack">
            <h3>Lista pública por UserId</h3>
            <input placeholder="User ID" value={externalUserId} onChange={(e) => setExternalUserId(e.target.value)} />
            <button onClick={() => run(async () => {
              const list = await api.listUserAnimeList(externalUserId, { sortField: AnimeListSort.Name, sortDirection: SortDirection.Asc }).then((res) => res.data);
              setExternalListResult(list);
              setAlert(`Lista pública carregada: ${list.lists.length} itens.`);
            })}>Carregar lista pública</button>
            {externalListResult && <pre>{JSON.stringify(externalListResult, null, 2)}</pre>}
          </div>

          <div className="form-stack danger-zone">
            <h3>Danger Zone</h3>
            <button onClick={() => run(async () => {
              await api.deleteMe();
              logout();
              setAlert('Conta removida (soft delete).');
            })}>Excluir minha conta (soft delete)</button>
          </div>
        </section>
      )}

      {authOpen && (
        <div className="modal">
          <div className="modal-card">
            <h3>Acesso</h3>
            <form onSubmit={onLogin} className="form-stack">
              <input name="login" placeholder="Usuário ou e-mail" required />
              <input name="password" type="password" placeholder="Senha" required />
              <button type="submit">Entrar</button>
            </form>

            <hr />

            <form onSubmit={onRegister} className="form-stack">
              <input name="userName" placeholder="Usuário" required />
              <input name="email" type="email" placeholder="E-mail" required />
              <input name="password" type="password" placeholder="Senha" required />
              <input name="confirmPassword" type="password" placeholder="Confirmar senha" required />
              <button type="submit">Cadastrar</button>
            </form>

            <button className="ghost" onClick={() => setAuthOpen(false)}>Fechar</button>
          </div>
        </div>
      )}
    </div>
  );
}
