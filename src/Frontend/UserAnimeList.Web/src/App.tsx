import { useMemo, useState } from 'react';
import { ApiError, api } from './services/api';
import {
  AnimeEntryStatus,
  AnimeListSort,
  AnimeSort,
  AnimeStatus,
  AnimeType,
  Season,
  SortDirection,
  SourceType,
  type RequestAnimeFilterJson,
  type RequestAnimeJson,
  type RequestAnimeListEntryFilterJson,
  type RequestAnimeListEntryJson,
  type RequestAnimeSearchJson,
  type RequestChangePasswordJson,
  type RequestGenreGetByNameJson,
  type RequestLoginJson,
  type RequestNewTokenJson,
  type RequestRegisterGenreJson,
  type RequestRegisterStudioJson,
  type RequestRegisterUserJson,
  type RequestStudioGetByNameJson,
  type RequestUpdateAnimeListEntryJson,
  type RequestUpdateGenreJson,
  type RequestUpdateStudioJson,
  type RequestUpdateUserJson
} from './types/contracts';

const defaults = {
  login: { login: '', password: '' } as RequestLoginJson,
  newToken: { refreshToken: '' } as RequestNewTokenJson,
  registerUser: { userName: '', email: '', password: '', confirmPassword: '' } as RequestRegisterUserJson,
  updateUser: { userName: '', email: '' } as RequestUpdateUserJson,
  changePassword: { password: '', newPassword: '', confirmNewPassword: '' } as RequestChangePasswordJson,
  registerAnime: {
    name: '',
    synopsis: '',
    episodes: 12,
    genres: [],
    studios: [],
    status: AnimeStatus.Airing,
    source: SourceType.Original,
    type: AnimeType.Tv,
    airedFrom: '',
    airedUntil: ''
  } as RequestAnimeJson,
  animeSearch: { query: '' } as RequestAnimeSearchJson,
  animeFilter: {
    query: '',
    status: AnimeStatus.Airing,
    type: AnimeType.Tv,
    genres: [],
    studios: [],
    airedFrom: '',
    airedUntil: '',
    premieredSeason: Season.Spring,
    premieredYear: new Date().getFullYear(),
    sortField: AnimeSort.Name,
    sortDirection: SortDirection.Asc
  } as RequestAnimeFilterJson,
  registerGenre: { name: '', description: '' } as RequestRegisterGenreJson,
  genreByName: { name: '' } as RequestGenreGetByNameJson,
  updateGenre: { name: '', description: '' } as RequestUpdateGenreJson,
  registerStudio: { name: '', description: '' } as RequestRegisterStudioJson,
  studioByName: { name: '' } as RequestStudioGetByNameJson,
  updateStudio: { name: '', description: '' } as RequestUpdateStudioJson,
  animeListEntry: {
    animeId: '',
    status: AnimeEntryStatus.Watching,
    score: 8,
    progress: 1,
    dateStarted: '',
    dateFinished: ''
  } as RequestAnimeListEntryJson,
  animeListFilter: {
    query: '',
    status: AnimeEntryStatus.Watching,
    dateStarted: '',
    dateFinished: '',
    sortField: AnimeListSort.Name,
    sortDirection: SortDirection.Asc
  } as RequestAnimeListEntryFilterJson,
  updateAnimeListEntry: {
    status: AnimeEntryStatus.Watching,
    score: 8,
    progress: 1,
    dateStarted: '',
    dateFinished: ''
  } as RequestUpdateAnimeListEntryJson
};

function normalize<T extends object>(value: T): T {
  const copy: Record<string, unknown> = { ...(value as Record<string, unknown>) };
  Object.keys(copy).forEach((key) => {
    if (copy[key] === '') delete copy[key];
  });
  return copy as T;
}

export function App() {
  const [accessToken, setAccessToken] = useState('');
  const [result, setResult] = useState('Execute um endpoint para ver o retorno.');
  const [loading, setLoading] = useState(false);

  const [jsonByKey, setJsonByKey] = useState<Record<string, string>>({
    login: JSON.stringify(defaults.login, null, 2),
    newToken: JSON.stringify(defaults.newToken, null, 2),
    registerUser: JSON.stringify(defaults.registerUser, null, 2),
    updateUser: JSON.stringify(defaults.updateUser, null, 2),
    changePassword: JSON.stringify(defaults.changePassword, null, 2),
    registerAnime: JSON.stringify(defaults.registerAnime, null, 2),
    animeSearch: JSON.stringify(defaults.animeSearch, null, 2),
    animeFilter: JSON.stringify(defaults.animeFilter, null, 2),
    registerGenre: JSON.stringify(defaults.registerGenre, null, 2),
    genreByName: JSON.stringify(defaults.genreByName, null, 2),
    updateGenre: JSON.stringify(defaults.updateGenre, null, 2),
    registerStudio: JSON.stringify(defaults.registerStudio, null, 2),
    studioByName: JSON.stringify(defaults.studioByName, null, 2),
    updateStudio: JSON.stringify(defaults.updateStudio, null, 2),
    animeListEntry: JSON.stringify(defaults.animeListEntry, null, 2),
    animeListFilter: JSON.stringify(defaults.animeListFilter, null, 2),
    updateAnimeListEntry: JSON.stringify(defaults.updateAnimeListEntry, null, 2)
  });

  const [idByKey, setIdByKey] = useState<Record<string, string>>({
    animeId: '',
    genreId: '',
    studioId: '',
    animeListId: '',
    userId: ''
  });

  const [fileByKey, setFileByKey] = useState<Record<string, File | null>>({ userImage: null, animeImage: null });

  const enumHelp = useMemo(
    () => ({ AnimeStatus, AnimeType, AnimeEntryStatus, AnimeSort, AnimeListSort, SortDirection, SourceType, Season }),
    []
  );

  function parseJson<T>(key: string): T {
    return JSON.parse(jsonByKey[key]) as T;
  }

  function setSuccess(payload: unknown) {
    setResult(JSON.stringify(payload ?? { ok: true }, null, 2));
  }

  function setFailure(error: unknown) {
    if (error instanceof ApiError) {
      setResult(JSON.stringify({ status: error.status, error: error.payload ?? error.message }, null, 2));
      return;
    }
    setResult(String(error));
  }

  async function run(label: string, fn: () => Promise<unknown>) {
    setLoading(true);
    try {
      const payload = await fn();
      setSuccess({ endpoint: label, payload });
    } catch (error) {
      setFailure(error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="page">
      <header>
        <h1>UserAnimeList Frontend (Refeito do Zero)</h1>
        <p>Console completo de endpoints usando contratos da pasta Shared (requests, responses e enums).</p>
      </header>

      <section className="card">
        <h2>Token atual (Bearer)</h2>
        <input value={accessToken} onChange={(e) => setAccessToken(e.target.value)} placeholder="Cole seu access token" />
      </section>

      <section className="card">
        <h2>Auth & User</h2>
        <div className="grid">
          <Endpoint title="POST /login" json={jsonByKey.login} onJson={(v) => setJsonByKey((s) => ({ ...s, login: v }))}
            onRun={() => run('/login', async () => {
              const response = await api.login(parseJson<RequestLoginJson>('login'));
              setAccessToken(response.tokens.accessToken);
              return response;
            })} loading={loading} />

          <Endpoint title="POST /token/refresh-token" json={jsonByKey.newToken} onJson={(v) => setJsonByKey((s) => ({ ...s, newToken: v }))}
            onRun={() => run('/token/refresh-token', async () => {
              const response = await api.refreshToken(parseJson<RequestNewTokenJson>('newToken'));
              setAccessToken(response.accessToken);
              return response;
            })} loading={loading} />

          <Endpoint title="POST /user" json={jsonByKey.registerUser} onJson={(v) => setJsonByKey((s) => ({ ...s, registerUser: v }))}
            onRun={() => run('/user POST', () => api.registerUser(parseJson<RequestRegisterUserJson>('registerUser')))} loading={loading} />

          <Action title="GET /user" onRun={() => run('/user GET', () => api.getUserProfile(accessToken))} loading={loading} />

          <Endpoint title="PUT /user" json={jsonByKey.updateUser} onJson={(v) => setJsonByKey((s) => ({ ...s, updateUser: v }))}
            onRun={() => run('/user PUT', () => api.updateUser(accessToken, parseJson<RequestUpdateUserJson>('updateUser')))} loading={loading} />

          <Endpoint title="PUT /user/change-password" json={jsonByKey.changePassword} onJson={(v) => setJsonByKey((s) => ({ ...s, changePassword: v }))}
            onRun={() => run('/user/change-password', async () => {
              const response = await api.changePassword(accessToken, parseJson<RequestChangePasswordJson>('changePassword'));
              setAccessToken(response.tokens.accessToken);
              return response;
            })} loading={loading} />

          <UploadAction title="PUT /user/me/image" onFile={(f) => setFileByKey((s) => ({ ...s, userImage: f }))}
            onRun={() => run('/user/me/image PUT', () => api.updateUserImage(accessToken, { image: fileByKey.userImage }))} loading={loading} />

          <Action title="DELETE /user/me/image" onRun={() => run('/user/me/image DELETE', () => api.deleteUserImage(accessToken))} loading={loading} />
          <Action title="DELETE /user/me" onRun={() => run('/user/me DELETE', () => api.deleteUserMe(accessToken))} loading={loading} />
        </div>
      </section>

      <section className="card">
        <h2>Anime</h2>
        <input placeholder="Anime ID" value={idByKey.animeId} onChange={(e) => setIdByKey((s) => ({ ...s, animeId: e.target.value }))} />
        <div className="grid">
          <Endpoint title="POST /anime" json={jsonByKey.registerAnime} onJson={(v) => setJsonByKey((s) => ({ ...s, registerAnime: v }))}
            onRun={() => run('/anime POST', () => api.registerAnime(accessToken, normalize(parseJson<RequestAnimeJson>('registerAnime'))))} loading={loading} />

          <Action title="GET /anime/{id}" onRun={() => run('/anime/{id} GET', () => api.getAnimeById(idByKey.animeId))} loading={loading} />

          <Endpoint title="GET /anime/search" json={jsonByKey.animeSearch} onJson={(v) => setJsonByKey((s) => ({ ...s, animeSearch: v }))}
            onRun={() => run('/anime/search', () => api.searchAnime(normalize(parseJson<RequestAnimeSearchJson>('animeSearch'))))} loading={loading} />

          <Endpoint title="GET /anime/filter" json={jsonByKey.animeFilter} onJson={(v) => setJsonByKey((s) => ({ ...s, animeFilter: v }))}
            onRun={() => run('/anime/filter', () => api.filterAnime(normalize(parseJson<RequestAnimeFilterJson>('animeFilter'))))} loading={loading} />

          <Endpoint title="PUT /anime/{id}" json={jsonByKey.registerAnime} onJson={(v) => setJsonByKey((s) => ({ ...s, registerAnime: v }))}
            onRun={() => run('/anime/{id} PUT', () => api.updateAnime(accessToken, idByKey.animeId, normalize(parseJson<RequestAnimeJson>('registerAnime'))))} loading={loading} />

          <UploadAction title="PUT /anime/{id}/image" onFile={(f) => setFileByKey((s) => ({ ...s, animeImage: f }))}
            onRun={() => run('/anime/{id}/image PUT', () => api.updateAnimeImage(accessToken, idByKey.animeId, { image: fileByKey.animeImage }))} loading={loading} />

          <Action title="DELETE /anime/{id}/image" onRun={() => run('/anime/{id}/image DELETE', () => api.deleteAnimeImage(accessToken, idByKey.animeId))} loading={loading} />
          <Action title="DELETE /anime/{id}" onRun={() => run('/anime/{id} DELETE', () => api.deleteAnime(accessToken, idByKey.animeId))} loading={loading} />
        </div>
      </section>

      <section className="card">
        <h2>Genre & Studio</h2>
        <div className="inline">
          <input placeholder="Genre ID" value={idByKey.genreId} onChange={(e) => setIdByKey((s) => ({ ...s, genreId: e.target.value }))} />
          <input placeholder="Studio ID" value={idByKey.studioId} onChange={(e) => setIdByKey((s) => ({ ...s, studioId: e.target.value }))} />
        </div>
        <div className="grid">
          <Endpoint title="POST /genre" json={jsonByKey.registerGenre} onJson={(v) => setJsonByKey((s) => ({ ...s, registerGenre: v }))}
            onRun={() => run('/genre POST', () => api.registerGenre(accessToken, parseJson<RequestRegisterGenreJson>('registerGenre')))} loading={loading} />
          <Action title="GET /genre/{id}" onRun={() => run('/genre/{id} GET', () => api.getGenreById(idByKey.genreId))} loading={loading} />
          <Endpoint title="POST /genre/search" json={jsonByKey.genreByName} onJson={(v) => setJsonByKey((s) => ({ ...s, genreByName: v }))}
            onRun={() => run('/genre/search', () => api.getGenreByName(parseJson<RequestGenreGetByNameJson>('genreByName')))} loading={loading} />
          <Endpoint title="PUT /genre/{id}" json={jsonByKey.updateGenre} onJson={(v) => setJsonByKey((s) => ({ ...s, updateGenre: v }))}
            onRun={() => run('/genre/{id} PUT', () => api.updateGenre(accessToken, idByKey.genreId, parseJson<RequestUpdateGenreJson>('updateGenre')))} loading={loading} />
          <Action title="DELETE /genre/{id}" onRun={() => run('/genre/{id} DELETE', () => api.deleteGenre(accessToken, idByKey.genreId))} loading={loading} />

          <Endpoint title="POST /studio" json={jsonByKey.registerStudio} onJson={(v) => setJsonByKey((s) => ({ ...s, registerStudio: v }))}
            onRun={() => run('/studio POST', () => api.registerStudio(accessToken, parseJson<RequestRegisterStudioJson>('registerStudio')))} loading={loading} />
          <Action title="GET /studio/{id}" onRun={() => run('/studio/{id} GET', () => api.getStudioById(idByKey.studioId))} loading={loading} />
          <Endpoint title="POST /studio/search" json={jsonByKey.studioByName} onJson={(v) => setJsonByKey((s) => ({ ...s, studioByName: v }))}
            onRun={() => run('/studio/search', () => api.getStudioByName(parseJson<RequestStudioGetByNameJson>('studioByName')))} loading={loading} />
          <Endpoint title="PUT /studio/{id}" json={jsonByKey.updateStudio} onJson={(v) => setJsonByKey((s) => ({ ...s, updateStudio: v }))}
            onRun={() => run('/studio/{id} PUT', () => api.updateStudio(accessToken, idByKey.studioId, parseJson<RequestUpdateStudioJson>('updateStudio')))} loading={loading} />
          <Action title="DELETE /studio/{id}" onRun={() => run('/studio/{id} DELETE', () => api.deleteStudio(accessToken, idByKey.studioId))} loading={loading} />
        </div>
      </section>

      <section className="card">
        <h2>AnimeList</h2>
        <div className="inline">
          <input placeholder="AnimeList Entry ID" value={idByKey.animeListId} onChange={(e) => setIdByKey((s) => ({ ...s, animeListId: e.target.value }))} />
          <input placeholder="User ID" value={idByKey.userId} onChange={(e) => setIdByKey((s) => ({ ...s, userId: e.target.value }))} />
        </div>
        <div className="grid">
          <Endpoint title="POST /animelist" json={jsonByKey.animeListEntry} onJson={(v) => setJsonByKey((s) => ({ ...s, animeListEntry: v }))}
            onRun={() => run('/animelist POST', () => api.addAnimeListEntry(accessToken, normalize(parseJson<RequestAnimeListEntryJson>('animeListEntry'))))} loading={loading} />
          <Action title="GET /animelist/{id}" onRun={() => run('/animelist/{id} GET', () => api.getAnimeListEntryById(accessToken, idByKey.animeListId))} loading={loading} />
          <Endpoint title="GET /animelist/list/{userId}" json={jsonByKey.animeListFilter} onJson={(v) => setJsonByKey((s) => ({ ...s, animeListFilter: v }))}
            onRun={() => run('/animelist/list/{userId}', () => api.listAnimeByUserId(idByKey.userId, normalize(parseJson<RequestAnimeListEntryFilterJson>('animeListFilter'))))} loading={loading} />
          <Endpoint title="GET /animelist/me/list" json={jsonByKey.animeListFilter} onJson={(v) => setJsonByKey((s) => ({ ...s, animeListFilter: v }))}
            onRun={() => run('/animelist/me/list', () => api.listMyAnime(accessToken, normalize(parseJson<RequestAnimeListEntryFilterJson>('animeListFilter'))))} loading={loading} />
          <Endpoint title="PUT /animelist/{id}" json={jsonByKey.updateAnimeListEntry} onJson={(v) => setJsonByKey((s) => ({ ...s, updateAnimeListEntry: v }))}
            onRun={() => run('/animelist/{id} PUT', () => api.updateAnimeListEntry(accessToken, idByKey.animeListId, normalize(parseJson<RequestUpdateAnimeListEntryJson>('updateAnimeListEntry'))))} loading={loading} />
          <Action title="DELETE /animelist/{id}" onRun={() => run('/animelist/{id} DELETE', () => api.deleteAnimeListEntry(accessToken, idByKey.animeListId))} loading={loading} />
        </div>
      </section>

      <section className="card">
        <h2>Enums da Shared</h2>
        <pre>{JSON.stringify(enumHelp, null, 2)}</pre>
      </section>

      <section className="card">
        <h2>Resposta</h2>
        <pre>{result}</pre>
      </section>
    </main>
  );
}

interface EndpointProps {
  title: string;
  json: string;
  loading: boolean;
  onJson: (value: string) => void;
  onRun: () => void;
}

function Endpoint({ title, json, onJson, onRun, loading }: EndpointProps) {
  return (
    <article className="endpoint">
      <h3>{title}</h3>
      <textarea value={json} onChange={(e) => onJson(e.target.value)} rows={8} />
      <button onClick={onRun} disabled={loading}>Executar</button>
    </article>
  );
}

function Action({ title, onRun, loading }: { title: string; onRun: () => void; loading: boolean }) {
  return (
    <article className="endpoint">
      <h3>{title}</h3>
      <button onClick={onRun} disabled={loading}>Executar</button>
    </article>
  );
}

function UploadAction({ title, onRun, onFile, loading }: { title: string; onRun: () => void; onFile: (file: File | null) => void; loading: boolean }) {
  return (
    <article className="endpoint">
      <h3>{title}</h3>
      <input type="file" accept="image/*" onChange={(e) => onFile(e.target.files?.[0] ?? null)} />
      <button onClick={onRun} disabled={loading}>Executar</button>
    </article>
  );
}
