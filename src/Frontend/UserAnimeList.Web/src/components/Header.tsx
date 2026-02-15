interface HeaderProps {
  userName?: string;
  onHome: () => void;
  onProfile: () => void;
  onAuth: () => void;
  onLogout: () => void;
}

export function Header({ userName, onHome, onProfile, onAuth, onLogout }: HeaderProps) {
  return (
    <header className="header card">
      <div>
        <h1>UserAnimeList</h1>
        <p>Frontend de portfólio para demonstrar fluxos da API.</p>
      </div>

      <nav className="header-actions">
        <button type="button" className="btn-ghost" onClick={onHome}>
          Home
        </button>

        {userName ? (
          <>
            <button type="button" className="btn-ghost" onClick={onProfile}>
              Perfil
            </button>
            <button type="button" className="btn-ghost" onClick={onLogout}>
              Sair
            </button>
            <span className="badge">{userName}</span>
          </>
        ) : (
          <button type="button" onClick={onAuth}>
            Entrar / Cadastrar
          </button>
        )}
      </nav>
    </header>
  );
}
