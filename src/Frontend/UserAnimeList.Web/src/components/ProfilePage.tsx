import { MyListPanel } from './MyListPanel';
import type { AnimeListEntry, UserProfile } from '../types/api';

interface ProfilePageProps {
  profile: UserProfile | null;
  entries: AnimeListEntry[];
  loading: boolean;
  onUpdate: (entry: AnimeListEntry) => Promise<void>;
  onDelete: (entryId: string) => Promise<void>;
}

const PROFILE_PLACEHOLDER = '/placeholders/profile-no-image.svg';

export function ProfilePage({ profile, entries, loading, onUpdate, onDelete }: ProfilePageProps) {
  return (
    <section className="stack">
      <article className="card">
        <h2>Perfil</h2>
        {!profile && <small>Você precisa estar autenticado para visualizar o perfil.</small>}
        {profile && (
          <div className="profile-box">
            <img src={profile.imageUrl || PROFILE_PLACEHOLDER} alt={profile.userName} />
            <div>
              <strong>{profile.userName}</strong>
              <small>{profile.email}</small>
            </div>
          </div>
        )}
      </article>

      <MyListPanel entries={entries} loading={loading} onUpdate={onUpdate} onDelete={onDelete} />
    </section>
  );
}
