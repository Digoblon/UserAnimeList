import { useState } from 'react';
import { Navigate } from 'react-router-dom';
import { api } from '../api/client';
import { useAuth } from '../contexts/AuthContext';
import { AnimeStatus, AnimeType, SourceType } from '../types/contracts';

export function AdminPage() {
  const { accessToken, isAdmin } = useAuth();
  const [message, setMessage] = useState('');

  const [animeId, setAnimeId] = useState('');
  const [anime, setAnime] = useState({ name: '', synopsis: '', episodes: 12, genres: '', studios: '', status: AnimeStatus.Airing, source: SourceType.Original, type: AnimeType.Tv, airedFrom: '', airedUntil: '' });

  const [genreId, setGenreId] = useState('');
  const [genre, setGenre] = useState({ name: '', description: '' });

  const [studioId, setStudioId] = useState('');
  const [studio, setStudio] = useState({ name: '', description: '' });

  if (!isAdmin) return <Navigate to="/" replace />;

  return (
    <div className="stack-lg">
      <h1>Painel Administrativo</h1>

      <section className="card">
        <h2>Anime (admin)</h2>
        <div className="form-grid">
          <input placeholder="ID do anime (para update/delete)" value={animeId} onChange={(e) => setAnimeId(e.target.value)} />
          <input placeholder="Nome" value={anime.name} onChange={(e) => setAnime((s) => ({ ...s, name: e.target.value }))} />
          <input placeholder="Sinopse" value={anime.synopsis} onChange={(e) => setAnime((s) => ({ ...s, synopsis: e.target.value }))} />
          <input type="number" placeholder="Episódios" value={anime.episodes} onChange={(e) => setAnime((s) => ({ ...s, episodes: Number(e.target.value) }))} />
          <input placeholder="IDs de gênero (csv)" value={anime.genres} onChange={(e) => setAnime((s) => ({ ...s, genres: e.target.value }))} />
          <input placeholder="IDs de studio (csv)" value={anime.studios} onChange={(e) => setAnime((s) => ({ ...s, studios: e.target.value }))} />
          <input type="date" value={anime.airedFrom} onChange={(e) => setAnime((s) => ({ ...s, airedFrom: e.target.value }))} />
          <input type="date" value={anime.airedUntil} onChange={(e) => setAnime((s) => ({ ...s, airedUntil: e.target.value }))} />
          <select value={anime.status} onChange={(e) => setAnime((s) => ({ ...s, status: Number(e.target.value) as AnimeStatus }))}>
            {Object.entries(AnimeStatus).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>
          <select value={anime.type} onChange={(e) => setAnime((s) => ({ ...s, type: Number(e.target.value) as AnimeType }))}>
            {Object.entries(AnimeType).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>
          <select value={anime.source} onChange={(e) => setAnime((s) => ({ ...s, source: Number(e.target.value) as SourceType }))}>
            {Object.entries(SourceType).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>
        </div>
        <div className="inline">
          <button className="btn" onClick={async () => {
            try {
              await api.registerAnime(accessToken, { ...anime, genres: anime.genres.split(',').map((x) => x.trim()).filter(Boolean), studios: anime.studios.split(',').map((x) => x.trim()).filter(Boolean) });
              setMessage('Anime registrado.');
            } catch { setMessage('Falha ao registrar anime.'); }
          }}>Registrar anime</button>
          <button className="btn" onClick={async () => {
            try {
              await api.updateAnime(accessToken, animeId, { ...anime, genres: anime.genres.split(',').map((x) => x.trim()).filter(Boolean), studios: anime.studios.split(',').map((x) => x.trim()).filter(Boolean) });
              setMessage('Anime atualizado.');
            } catch { setMessage('Falha ao atualizar anime.'); }
          }}>Atualizar anime</button>
          <button className="btn btn-danger" onClick={async () => { try { await api.deleteAnime(accessToken, animeId); setMessage('Anime removido.'); } catch { setMessage('Falha ao remover anime.'); } }}>Excluir anime</button>
        </div>
        <div className="inline">
          <input type="file" accept="image/*" onChange={async (e) => { const file = e.target.files?.[0]; if (!file) return; try { await api.updateAnimeImage(accessToken, animeId, { image: file }); setMessage('Imagem do anime atualizada.'); } catch { setMessage('Falha ao atualizar imagem do anime.'); } }} />
          <button className="btn" onClick={async () => { try { await api.deleteAnimeImage(accessToken, animeId); setMessage('Imagem do anime removida.'); } catch { setMessage('Falha ao remover imagem do anime.'); } }}>Remover imagem do anime</button>
        </div>
      </section>

      <section className="card">
        <h2>Genre (admin)</h2>
        <div className="form-grid">
          <input placeholder="Genre ID" value={genreId} onChange={(e) => setGenreId(e.target.value)} />
          <input placeholder="Nome" value={genre.name} onChange={(e) => setGenre((s) => ({ ...s, name: e.target.value }))} />
          <input placeholder="Descrição" value={genre.description} onChange={(e) => setGenre((s) => ({ ...s, description: e.target.value }))} />
        </div>
        <div className="inline">
          <button className="btn" onClick={async () => { try { await api.registerGenre(accessToken, genre); setMessage('Gênero registrado.'); } catch { setMessage('Falha ao registrar gênero.'); } }}>Registrar gênero</button>
          <button className="btn" onClick={async () => { try { await api.updateGenre(accessToken, genreId, genre); setMessage('Gênero atualizado.'); } catch { setMessage('Falha ao atualizar gênero.'); } }}>Atualizar gênero</button>
          <button className="btn btn-danger" onClick={async () => { try { await api.deleteGenre(accessToken, genreId); setMessage('Gênero removido.'); } catch { setMessage('Falha ao remover gênero.'); } }}>Excluir gênero</button>
        </div>
      </section>

      <section className="card">
        <h2>Studio (admin)</h2>
        <div className="form-grid">
          <input placeholder="Studio ID" value={studioId} onChange={(e) => setStudioId(e.target.value)} />
          <input placeholder="Nome" value={studio.name} onChange={(e) => setStudio((s) => ({ ...s, name: e.target.value }))} />
          <input placeholder="Descrição" value={studio.description} onChange={(e) => setStudio((s) => ({ ...s, description: e.target.value }))} />
        </div>
        <div className="inline">
          <button className="btn" onClick={async () => { try { await api.registerStudio(accessToken, studio); setMessage('Studio registrado.'); } catch { setMessage('Falha ao registrar studio.'); } }}>Registrar studio</button>
          <button className="btn" onClick={async () => { try { await api.updateStudio(accessToken, studioId, studio); setMessage('Studio atualizado.'); } catch { setMessage('Falha ao atualizar studio.'); } }}>Atualizar studio</button>
          <button className="btn btn-danger" onClick={async () => { try { await api.deleteStudio(accessToken, studioId); setMessage('Studio removido.'); } catch { setMessage('Falha ao remover studio.'); } }}>Excluir studio</button>
        </div>
      </section>

      {message && <p className="hint">{message}</p>}
    </div>
  );
}
