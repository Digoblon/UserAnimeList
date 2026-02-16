import { Link, NavLink } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export function Layout({ children }: { children: React.ReactNode }) {
  const { isLogged, isAdmin } = useAuth();

  return (
    <div>
      <header className="topbar">
        <Link to="/" className="brand">UserAnimeList</Link>
        <nav>
          <NavLink to="/search">Busca avançada</NavLink>
          {isLogged ? <NavLink to="/profile">Perfil</NavLink> : <NavLink to="/auth">Entrar / Criar conta</NavLink>}
          {isAdmin && <NavLink to="/admin" className="admin-btn">Admin</NavLink>}
        </nav>
      </header>
      <main className="container">{children}</main>
    </div>
  );
}
