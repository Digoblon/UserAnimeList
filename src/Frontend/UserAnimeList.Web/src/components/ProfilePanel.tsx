import type { UserProfile } from '../types/api';

interface ProfilePanelProps {
  profile: UserProfile | null;
}

export function ProfilePanel({ profile }: ProfilePanelProps) {
  return (
    <section className="card">
      <h2>Perfil</h2>
      {!profile && <small>Faça login para visualizar dados do perfil.</small>}
      {profile && (
        <div className="profile-box">
          <img src={profile.imageUrl || 'https://placehold.co/96x96?text=User'} alt={profile.userName} />
          <div>
            <strong>{profile.userName}</strong>
            <small>{profile.email}</small>
          </div>
        </div>
      )}
    </section>
  );
}
