import { useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { api } from '../api/client';
import { AnimeCard } from '../components/AnimeCard';
import { AnimeSort, AnimeStatus, AnimeType, Season, SortDirection, type RequestAnimeFilterJson, type ResponseShortAnimeJson } from '../types/contracts';

export function SearchPage() {
  const [params] = useSearchParams();
  const [results, setResults] = useState<ResponseShortAnimeJson[]>([]);

  const [genreName, setGenreName] = useState('');
  const [studioName, setStudioName] = useState('');
  const [genreHints, setGenreHints] = useState<{id:string;name:string}[]>([]);
  const [studioHints, setStudioHints] = useState<{id:string;name:string}[]>([]);

  const [filter, setFilter] = useState<RequestAnimeFilterJson>({
    query: params.get('query') ?? '',
    genres: params.get('genre') ? [params.get('genre')!] : [],
    sortField: AnimeSort.Name,
    sortDirection: SortDirection.Asc
  });

  const enumOptions = useMemo(() => ({ AnimeStatus, AnimeType, Season, AnimeSort, SortDirection }), []);

  async function submit() {
    try {
      const response = await api.filterAnime(filter);
      setResults(response.animes ?? []);
    } catch {
      setResults([]);
    }
  }

  useEffect(() => { void submit(); }, []);

  async function searchHelpers() {
    if (genreName.trim()) {
      try {
        const r = await api.getGenreByName({ name: genreName });
        setGenreHints(r.genres ?? []);
      } catch { setGenreHints([]); }
    } else setGenreHints([]);

    if (studioName.trim()) {
      try {
        const r = await api.getStudioByName({ name: studioName });
        setStudioHints(r.studios ?? []);
      } catch { setStudioHints([]); }
    } else setStudioHints([]);
  }

  return (
    <div className="stack-lg">
      <h1>Busca avançada</h1>
      <section className="card">
        <div className="form-grid">
          <input placeholder="Nome" value={filter.query ?? ''} onChange={(e) => setFilter((f) => ({ ...f, query: e.target.value }))} />
          <input placeholder="IDs de gênero (csv)" value={(filter.genres ?? []).join(',')} onChange={(e) => setFilter((f) => ({ ...f, genres: e.target.value.split(',').map((x) => x.trim()).filter(Boolean) }))} />
          <input placeholder="IDs de estúdio (csv)" value={(filter.studios ?? []).join(',')} onChange={(e) => setFilter((f) => ({ ...f, studios: e.target.value.split(',').map((x) => x.trim()).filter(Boolean) }))} />
          <input type="date" value={filter.airedFrom ?? ''} onChange={(e) => setFilter((f) => ({ ...f, airedFrom: e.target.value }))} />
          <input type="date" value={filter.airedUntil ?? ''} onChange={(e) => setFilter((f) => ({ ...f, airedUntil: e.target.value }))} />
          <input type="number" placeholder="Ano de estreia" value={filter.premieredYear ?? ''} onChange={(e) => setFilter((f) => ({ ...f, premieredYear: Number(e.target.value) || undefined }))} />

          <select value={filter.status ?? ''} onChange={(e) => setFilter((f) => ({ ...f, status: e.target.value === '' ? undefined : Number(e.target.value) as AnimeStatus }))}>
            <option value="">Status</option>
            {Object.entries(enumOptions.AnimeStatus).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>

          <select value={filter.type ?? ''} onChange={(e) => setFilter((f) => ({ ...f, type: e.target.value === '' ? undefined : Number(e.target.value) as AnimeType }))}>
            <option value="">Tipo</option>
            {Object.entries(enumOptions.AnimeType).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>

          <select value={filter.premieredSeason ?? ''} onChange={(e) => setFilter((f) => ({ ...f, premieredSeason: e.target.value === '' ? undefined : Number(e.target.value) as Season }))}>
            <option value="">Season</option>
            {Object.entries(enumOptions.Season).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>

          <select value={filter.sortField ?? AnimeSort.Name} onChange={(e) => setFilter((f) => ({ ...f, sortField: Number(e.target.value) as AnimeSort }))}>
            {Object.entries(enumOptions.AnimeSort).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>

          <select value={filter.sortDirection ?? SortDirection.Asc} onChange={(e) => setFilter((f) => ({ ...f, sortDirection: Number(e.target.value) as SortDirection }))}>
            {Object.entries(enumOptions.SortDirection).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>
        </div>
        <button className="btn" onClick={submit}>Aplicar filtros</button>
      </section>


      <section className="card">
        <h3>Assistente de gênero e estúdio</h3>
        <div className="form-grid">
          <input placeholder="Buscar gênero por nome" value={genreName} onChange={(e) => setGenreName(e.target.value)} />
          <input placeholder="Buscar estúdio por nome" value={studioName} onChange={(e) => setStudioName(e.target.value)} />
        </div>
        <button className="btn" onClick={searchHelpers}>Pesquisar nomes</button>
        <div className="inline">
          {genreHints.map((g) => <button key={g.id} onClick={() => setFilter((f) => ({ ...f, genres: [...(f.genres ?? []), g.id] }))} className="chip">+ {g.name}</button>)}
          {studioHints.map((s) => <button key={s.id} onClick={() => setFilter((f) => ({ ...f, studios: [...(f.studios ?? []), s.id] }))} className="chip">+ {s.name}</button>)}
        </div>
      </section>

      <section>
        <div className="grid-cards">
          {results.map((anime) => <AnimeCard key={anime.id} anime={anime} />)}
        </div>
      </section>
    </div>
  );
}
