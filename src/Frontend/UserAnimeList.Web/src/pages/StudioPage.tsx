import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { api } from '../api/client';
import type { ResponseStudioJson } from '../types/contracts';

export function StudioPage() {
  const { id = '' } = useParams();
  const [studio, setStudio] = useState<ResponseStudioJson | null>(null);

  useEffect(() => {
    api.getStudioById(id).then(setStudio).catch(() => setStudio(null));
  }, [id]);

  if (!studio) return <p>Studio não encontrado.</p>;

  return (
    <section className="card">
      <h1>{studio.name}</h1>
      <p>{studio.description}</p>
      <small>ID: {studio.id}</small>
    </section>
  );
}
