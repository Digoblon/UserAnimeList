import type { FormEvent } from 'react';
import type { Anime } from '../types/api';

interface HomePageProps {
  query: string;
  loading: boolean;
  animes: Anime[];
  canAdd: boolean;
  onQueryChange: (value: string) => void;
  onSearch: (event: FormEvent) => Promise<void>;
  onAdd: (animeId: string) => Promise<void>;
}

const animeStatus: Record<number, string> = {
  0: 'Airing',
  1: 'Finished',
  2: 'Not yet aired'
};

const ANIME_PLACEHOLDER = '/placeholders/anime-no-image.svg';

export function HomePage({ query, loading, animes, canAdd, onQueryChange, onSearch, onAdd }: HomePageProps) {
  return (
    <section className="card">
      <h2>Home</h2>
      <p>Catálogo inicial com alguns animes e busca por nome.</p>

      <form onSubmit={(event) => void onSearch(event)} className="stack inline">
        <input placeholder="Buscar anime..." value={query} onChange={(event) => onQueryChange(event.target.value)} />
        <button type="submit" disabled={loading}>
          Buscar
        </button>
      </form>

      <div className="anime-grid">
        {animes.map((anime) => (
          <article key={anime.id} className="anime-card">
            <img src={anime.imageUrl || ANIME_PLACEHOLDER} alt={anime.name} />
            <div>
              <strong>{anime.name}</strong>
              <small>{animeStatus[anime.status] ?? 'Unknown'}</small>
              <small>Score: {anime.score ?? '-'}</small>
            </div>
            <button type="button" disabled={loading || !canAdd} onClick={() => void onAdd(anime.id)}>
              Adicionar à lista
            </button>
          </article>
        ))}
      </div>
    </section>
  );
}
