import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../api/client';
import { useAuth } from '../contexts/AuthContext';
import { AnimeEntryStatus, AnimeListSort, SortDirection, type ResponseAnimeListEntryJson, type ResponseShortAnimeListEntryJson, type ResponseUserProfileJson } from '../types/contracts';

const PLACEHOLDER = '/placeholders/profile-no-image.svg';

export function ProfilePage() {
  const navigate = useNavigate();
  const { accessToken, refreshToken, refresh, logout } = useAuth();
  const [profile, setProfile] = useState<ResponseUserProfileJson | null>(null);
  const [myList, setMyList] = useState<ResponseShortAnimeListEntryJson[]>([]);
  const [entryDetails, setEntryDetails] = useState<ResponseAnimeListEntryJson | null>(null);
  const [message, setMessage] = useState('');

  const [updateUser, setUpdateUser] = useState({ userName: '', email: '' });
  const [changePwd, setChangePwd] = useState({ password: '', newPassword: '', confirmNewPassword: '' });
  const [newEntry, setNewEntry] = useState({ animeId: '', status: AnimeEntryStatus.Watching, score: '', progress: '' });
  const [entryId, setEntryId] = useState('');
  const [publicUserId, setPublicUserId] = useState('');

  async function loadProfile() {
    const p = await api.getUserProfile(accessToken);
    setProfile(p);
    setUpdateUser({ userName: p.userName, email: p.email });
  }
  async function loadList() {
    const l = await api.listMyAnime(accessToken, { sortField: AnimeListSort.Name, sortDirection: SortDirection.Asc });
    setMyList(l.lists ?? []);
  }

  useEffect(() => {
    if (!accessToken) { navigate('/auth'); return; }
    Promise.all([loadProfile(), loadList()]).catch(() => setMessage('Falha ao carregar perfil.'));
  }, [accessToken]);

  return (
    <div className="stack-lg">
      <section className="card profile-head">
        <img src={profile?.imageUrl || PLACEHOLDER} alt={profile?.userName || 'profile'} />
        <div>
          <h1>{profile?.userName || 'Perfil'}</h1>
          <p>{profile?.email}</p>
          <div className="inline">
            <button className="btn" onClick={async () => { try { await refresh(); setMessage('Token renovado com sucesso.'); } catch { setMessage('Falha ao renovar token.'); } }}>Renovar sessão</button>
            <button className="btn" onClick={() => { logout(); navigate('/'); }}>Deslogar</button>
          </div>
          <small>Refresh token: {refreshToken ? 'disponível' : 'indisponível'}</small>
        </div>
      </section>

      <section className="card">
        <h2>Atualizar perfil</h2>
        <div className="form-grid">
          <input value={updateUser.userName} onChange={(e) => setUpdateUser((s) => ({ ...s, userName: e.target.value }))} />
          <input value={updateUser.email} onChange={(e) => setUpdateUser((s) => ({ ...s, email: e.target.value }))} />
        </div>
        <button className="btn" onClick={async () => { try { await api.updateUser(accessToken, updateUser); await loadProfile(); setMessage('Perfil atualizado.'); } catch { setMessage('Falha ao atualizar perfil.'); } }}>Salvar alterações</button>
      </section>

      <section className="card">
        <h2>Foto de perfil</h2>
        <input type="file" accept="image/*" onChange={async (e) => {
          const file = e.target.files?.[0];
          if (!file) return;
          try { await api.updateUserImage(accessToken, { image: file }); await loadProfile(); setMessage('Imagem atualizada.'); }
          catch { setMessage('Falha ao atualizar imagem.'); }
        }} />
        <button className="btn" onClick={async () => { try { await api.deleteUserImage(accessToken); await loadProfile(); setMessage('Imagem removida.'); } catch { setMessage('Falha ao remover imagem.'); } }}>Remover imagem</button>
      </section>

      <section className="card">
        <h2>Alterar senha</h2>
        <div className="form-grid">
          <input type="password" placeholder="Senha atual" onChange={(e) => setChangePwd((s) => ({ ...s, password: e.target.value }))} />
          <input type="password" placeholder="Nova senha" onChange={(e) => setChangePwd((s) => ({ ...s, newPassword: e.target.value }))} />
          <input type="password" placeholder="Confirmar nova senha" onChange={(e) => setChangePwd((s) => ({ ...s, confirmNewPassword: e.target.value }))} />
        </div>
        <button className="btn" onClick={async () => {
          try {
            const r = await api.changePassword(accessToken, changePwd);
            setMessage('Senha alterada. Nova sessão aplicada.');
            localStorage.setItem('ual_access', r.tokens.accessToken);
            localStorage.setItem('ual_refresh', r.tokens.refreshToken);
            window.location.reload();
          } catch { setMessage('Falha ao alterar senha.'); }
        }}>Alterar senha</button>
      </section>

      <section className="card">
        <h2>Minha lista</h2>
        <div className="form-grid">
          <input placeholder="Anime ID" value={newEntry.animeId} onChange={(e) => setNewEntry((s) => ({ ...s, animeId: e.target.value }))} />
          <select value={newEntry.status} onChange={(e) => setNewEntry((s) => ({ ...s, status: Number(e.target.value) as AnimeEntryStatus }))}>
            {Object.entries(AnimeEntryStatus).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
          </select>
          <input placeholder="Nota" value={newEntry.score} onChange={(e) => setNewEntry((s) => ({ ...s, score: e.target.value }))} />
          <input placeholder="Progresso" value={newEntry.progress} onChange={(e) => setNewEntry((s) => ({ ...s, progress: e.target.value }))} />
        </div>
        <button className="btn" onClick={async () => {
          try {
            await api.addAnimeListEntry(accessToken, { animeId: newEntry.animeId, status: newEntry.status, score: newEntry.score ? Number(newEntry.score) : null, progress: newEntry.progress ? Number(newEntry.progress) : null });
            await loadList();
            setMessage('Entrada adicionada.');
          } catch { setMessage('Falha ao adicionar entrada.'); }
        }}>Adicionar à lista</button>

        <div className="list-items">
          {myList.map((entry) => (
            <article key={entry.id} className="list-row">
              <strong>{entry.name}</strong>
              <span>{AnimeEntryStatus[entry.status]}</span>
              <span>Score: {entry.score ?? '-'}</span>
              <button onClick={async () => {
                try { const d = await api.getAnimeListEntryById(accessToken, entry.id); setEntryDetails(d); setEntryId(entry.id); }
                catch { setMessage('Falha ao buscar entrada.'); }
              }}>Detalhes</button>
              <button onClick={async () => { try { await api.deleteAnimeListEntry(accessToken, entry.id); await loadList(); } catch { setMessage('Falha ao remover.'); } }}>Remover</button>
            </article>
          ))}
        </div>

        {entryDetails && (
          <div className="card muted">
            <h3>Editar entrada selecionada</h3>
            <p>Anime ID: {entryDetails.animeId}</p>
            <div className="inline">
              <select defaultValue={entryDetails.status} onChange={(e) => setEntryDetails((d) => d ? ({ ...d, status: Number(e.target.value) as AnimeEntryStatus }) : d)}>
                {Object.entries(AnimeEntryStatus).filter(([, v]) => typeof v === 'number').map(([k, v]) => <option key={k} value={v}>{k}</option>)}
              </select>
              <input defaultValue={entryDetails.score ?? ''} onChange={(e) => setEntryDetails((d) => d ? ({ ...d, score: e.target.value ? Number(e.target.value) : null }) : d)} />
              <input defaultValue={entryDetails.progress ?? ''} onChange={(e) => setEntryDetails((d) => d ? ({ ...d, progress: e.target.value ? Number(e.target.value) : null }) : d)} />
            </div>
            <button className="btn" onClick={async () => {
              try {
                await api.updateAnimeListEntry(accessToken, entryId, {
                  status: entryDetails.status,
                  score: entryDetails.score,
                  progress: entryDetails.progress,
                  dateStarted: entryDetails.dateStarted,
                  dateFinished: entryDetails.dateFinished
                });
                await loadList();
                setMessage('Entrada atualizada.');
              } catch { setMessage('Falha ao atualizar entrada.'); }
            }}>Salvar entrada</button>
          </div>
        )}
      </section>

      <section className="card">
        <h2>Lista pública por UserId</h2>
        <div className="inline">
          <input placeholder="User ID" value={publicUserId} onChange={(e) => setPublicUserId(e.target.value)} />
          <button className="btn" onClick={async () => {
            try {
              const list = await api.listAnimeByUserId(publicUserId, { sortField: AnimeListSort.Name, sortDirection: SortDirection.Asc });
              setMyList(list.lists);
              setMessage('Lista pública carregada na visualização atual.');
            } catch { setMessage('Falha ao carregar lista pública.'); }
          }}>Buscar lista</button>
        </div>
      </section>

      <section className="card danger">
        <h2>Excluir conta</h2>
        <button className="btn btn-danger" onClick={async () => {
          try { await api.deleteUserMe(accessToken); logout(); navigate('/'); }
          catch { setMessage('Falha ao excluir conta.'); }
        }}>Excluir minha conta</button>
      </section>

      {message && <p className="hint">{message}</p>}
    </div>
  );
}
