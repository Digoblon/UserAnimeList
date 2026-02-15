import type { FormEvent } from 'react';
import type { Anime } from '../types/api';

interface SearchPanelProps {
  query: string;
  foundAnime: Anime[];
  loading: boolean;
  canAdd: boolean;
  onQueryChange: (value: string) => void;
  onSearchSubmit: (event: FormEvent) => Promise<void>;
  onAdd: (animeId: string) => Promise<void>;
}

const animeStatus: Record<number, string> = {
  0: 'Airing',
  1: 'Finished',
  2: 'Not yet aired'
};

export function SearchPanel({
  query,
  foundAnime,
  loading,
  canAdd,
  onQueryChange,
  onSearchSubmit,
  onAdd
}: SearchPanelProps) {
  return (
    <section className="card">
      <h2>Buscar animes</h2>
      <form onSubmit={(event) => void onSearchSubmit(event)} className="stack inline">
        <input placeholder="Ex.: Fullmetal, One Piece..." value={query} onChange={(event) => onQueryChange(event.target.value)} />
        <button type="submit" disabled={loading}>
          Buscar
        </button>
      </form>

      <div className="anime-grid">
        {foundAnime.map((anime) => (
          <article key={anime.id} className="anime-card">
            <img src={anime.imageUrl || 'https://placehold.co/160x220?text=Anime'} alt={anime.name} />
            <div>
              <strong>{anime.name}</strong>
              <small>{animeStatus[anime.status] ?? 'Unknown'}</small>
              <small>Score: {anime.score ?? '-'}</small>
            </div>
            <button onClick={() => void onAdd(anime.id)} disabled={loading || !canAdd} type="button">
              Adicionar à lista
            </button>
          </article>
        ))}
      </div>
    </section>
  );
}
