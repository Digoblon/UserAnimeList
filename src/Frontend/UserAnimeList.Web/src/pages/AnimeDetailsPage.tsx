import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { api } from '../api/client';
import { useAuth } from '../contexts/AuthContext';
import { AnimeEntryStatus, type ResponseAnimeJson, type ResponseGenreJson, type ResponseStudioJson } from '../types/contracts';

const PLACEHOLDER = '/placeholders/anime-no-image.svg';

export function AnimeDetailsPage() {
  const { id = '' } = useParams();
  const navigate = useNavigate();
  const { accessToken, isLogged } = useAuth();
  const [anime, setAnime] = useState<ResponseAnimeJson | null>(null);
  const [genres, setGenres] = useState<ResponseGenreJson[]>([]);
  const [studios, setStudios] = useState<ResponseStudioJson[]>([]);
  const [message, setMessage] = useState('');

  useEffect(() => {
    (async () => {
      try {
        const data = await api.getAnimeById(id);
        setAnime(data);
        const genreResults = await Promise.all(data.genres.map((genreId) => api.getGenreById(genreId).catch(() => null)));
        setGenres(genreResults.filter(Boolean) as ResponseGenreJson[]);
        const studioResults = await Promise.all(data.studios.map((studioId) => api.getStudioById(studioId).catch(() => null)));
        setStudios(studioResults.filter(Boolean) as ResponseStudioJson[]);
      } catch {
        setMessage('Não foi possível carregar o anime.');
      }
    })();
  }, [id]);

  async function addToList() {
    if (!anime || !isLogged) return;
    try {
      await api.addAnimeListEntry(accessToken, { animeId: anime.id, status: AnimeEntryStatus.Watching, progress: 0, score: null });
      setMessage('Adicionado à sua lista!');
    } catch {
      setMessage('Falha ao adicionar na lista.');
    }
  }

  if (!anime) return <p>{message || 'Carregando...'}</p>;

  return (
    <section className="details-layout">
      <img className="cover" src={anime.imageUrl || PLACEHOLDER} alt={anime.name} />
      <div>
        <h1>{anime.name}</h1>
        <p>{anime.synopsis || 'Sem sinopse.'}</p>
        <p><strong>Score:</strong> {anime.score ?? '-'}</p>
        <p><strong>Episódios:</strong> {anime.episodes ?? '-'}</p>
        <p><strong>Exibição:</strong> {anime.airedFrom ?? '?'} até {anime.airedUntil ?? '?'}</p>
        <p><strong>Season:</strong> {anime.premiered}</p>

        <div className="chips">
          {genres.map((g) => (
            <button key={g.id} onClick={() => navigate(`/search?genre=${g.id}`)} className="chip">#{g.name}</button>
          ))}
        </div>

        <div className="chips">
          {studios.map((s) => (
            <Link key={s.id} to={`/studio/${s.id}`} className="chip">{s.name}</Link>
          ))}
        </div>

        {isLogged && <button className="btn" onClick={addToList}>Adicionar à minha lista</button>}
        {message && <p className="hint">{message}</p>}
      </div>
    </section>
  );
}
