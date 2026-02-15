import type { AnimeListEntry } from '../types/api';

interface MyListPanelProps {
  entries: AnimeListEntry[];
  loading: boolean;
  onUpdate: (entry: AnimeListEntry) => Promise<void>;
  onDelete: (entryId: string) => Promise<void>;
}

const listStatus: Record<number, string> = {
  0: 'Watching',
  1: 'Completed',
  2: 'PlanToWatch',
  3: 'Dropped',
  4: 'OnHold'
};

const statusOptions = [0, 1, 2, 3, 4];

export function MyListPanel({ entries, loading, onUpdate, onDelete }: MyListPanelProps) {
  return (
    <section className="card">
      <h2>Minha lista</h2>
      <div className="list">
        {entries.map((entry) => (
          <article key={entry.id} className="list-item list-item-column">
            <div className="list-item-header">
              <strong>{entry.name}</strong>
              <span className="score">{entry.score ?? '-'}</span>
            </div>

            <small>Status atual: {listStatus[entry.status] ?? 'Unknown'}</small>

            <div className="stack inline controls-row">
              <select
                defaultValue={entry.status}
                onChange={(event) =>
                  void onUpdate({
                    ...entry,
                    status: Number(event.target.value)
                  })
                }
                disabled={loading}
              >
                {statusOptions.map((option) => (
                  <option key={option} value={option}>
                    {listStatus[option]}
                  </option>
                ))}
              </select>

              <input
                type="number"
                min={0}
                max={10}
                defaultValue={entry.score ?? undefined}
                placeholder="Nota"
                onBlur={(event) =>
                  void onUpdate({
                    ...entry,
                    score: event.target.value ? Number(event.target.value) : null
                  })
                }
                disabled={loading}
              />

              <input
                type="number"
                min={0}
                defaultValue={entry.progress ?? undefined}
                placeholder="Progresso"
                onBlur={(event) =>
                  void onUpdate({
                    ...entry,
                    progress: event.target.value ? Number(event.target.value) : null
                  })
                }
                disabled={loading}
              />

              <button type="button" className="btn-danger" onClick={() => void onDelete(entry.id)} disabled={loading}>
                Remover
              </button>
            </div>
          </article>
        ))}
        {!entries.length && <small>Sem registros até o momento.</small>}
      </div>
    </section>
  );
}
