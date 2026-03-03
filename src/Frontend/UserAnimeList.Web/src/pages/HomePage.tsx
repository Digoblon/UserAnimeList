import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { api } from '../api/client';
import { AnimeCard } from '../components/AnimeCard';
import { useAuth } from '../contexts/AuthContext';
import { AnimeSort, SortDirection, type ResponseShortAnimeJson } from '../types/contracts';

export function HomePage() {
  const navigate = useNavigate();
  const { isLogged, isAdmin } = useAuth();
  const [animes, setAnimes] = useState<ResponseShortAnimeJson[]>([]);
  const [query, setQuery] = useState('');
  const [suggestions, setSuggestions] = useState<ResponseShortAnimeJson[]>([]);

  useEffect(() => {
    api.filterAnime({ sortField: AnimeSort.Premiered, sortDirection: SortDirection.Desc })
      .then((r) => setAnimes(r.animes ?? []))
      .catch(() => setAnimes([]));
  }, []);

  useEffect(() => {
    if (query.length < 2) {
      setSuggestions([]);
      return;
    }
    const timer = setTimeout(async () => {
      try {
        const r = await api.searchAnime({ query });
        setSuggestions((r.animes ?? []).slice(0, 5));
      } catch {
        setSuggestions([]);
      }
    }, 300);
    return () => clearTimeout(timer);
  }, [query]);

  return (
    <div className="stack-lg">
      <section className="hero">
        <h1>Descubra e acompanhe seus animes</h1>
        <p>Inspirado em MyAnimeList e AniList, com busca rápida, filtros e lista pessoal.</p>
        <div className="hero-actions">
          {!isLogged && <Link className="btn" to="/auth">Entrar / Criar conta</Link>}
          {isLogged && <Link className="btn" to="/profile">Meu perfil</Link>}
          {isAdmin && <Link className="btn btn-admin" to="/admin">Painel Admin</Link>}
        </div>
      </section>

      <section className="search-box">
        <h2>Buscar anime</h2>
        <input
          placeholder="Digite o nome do anime..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter' && query.trim()) navigate(`/search?query=${encodeURIComponent(query)}`);
          }}
        />
        {suggestions.length > 0 && (
          <div className="autocomplete">
            {suggestions.map((anime) => (
              <button key={anime.id} onClick={() => navigate(`/anime/${anime.id}`)}>
                {anime.name}
              </button>
            ))}
          </div>
        )}
        <button className="btn" onClick={() => navigate(`/search?query=${encodeURIComponent(query)}`)}>Busca avançada</button>
      </section>

      <section>
        <h2>Animes em destaque</h2>
        <div className="grid-cards">
          {animes.map((anime) => <AnimeCard key={anime.id} anime={anime} />)}
        </div>
      </section>
    </div>
  );
}
