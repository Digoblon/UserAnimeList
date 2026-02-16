import { Link } from 'react-router-dom';
import type { ResponseShortAnimeJson } from '../types/contracts';

const PLACEHOLDER = '/placeholders/anime-no-image.svg';

export function AnimeCard({ anime }: { anime: ResponseShortAnimeJson }) {
  return (
    <article className="anime-card">
      <Link to={`/anime/${anime.id}`}>
        <img src={anime.imageUrl || PLACEHOLDER} alt={anime.name} loading="lazy" />
      </Link>
      <div className="anime-card-body">
        <Link to={`/anime/${anime.id}`} className="anime-title">{anime.name}</Link>
        <small>Score: {anime.score ?? '-'}</small>
      </div>
    </article>
  );
}
