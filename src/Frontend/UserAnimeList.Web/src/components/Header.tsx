interface HeaderProps {
  userName?: string;
  onLogout?: () => void;
}

export function Header({ userName, onLogout }: HeaderProps) {
  return (
    <header className="header">
      <div>
        <h1>UserAnimeList</h1>
        <p>Frontend alinhado aos módulos da API: Auth, Anime, AnimeList e User.</p>
      </div>

      <div className="header-actions">
        <div className="badge">{userName ? `Olá, ${userName}` : 'Visitante'}</div>
        {userName && (
          <button className="btn-ghost" onClick={onLogout} type="button">
            Sair
          </button>
        )}
      </div>
    </header>
  );
}
