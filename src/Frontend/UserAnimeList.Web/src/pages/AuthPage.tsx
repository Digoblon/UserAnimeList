import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export function AuthPage() {
  const navigate = useNavigate();
  const { login, register } = useAuth();
  const [message, setMessage] = useState('');

  const [loginForm, setLoginForm] = useState({ login: '', password: '' });
  const [registerForm, setRegisterForm] = useState({ userName: '', email: '', password: '', confirmPassword: '' });

  return (
    <section className="auth-grid">
      <article className="card">
        <h2>Entrar</h2>
        <input placeholder="Usuário ou e-mail" value={loginForm.login} onChange={(e) => setLoginForm((s) => ({ ...s, login: e.target.value }))} />
        <input type="password" placeholder="Senha" value={loginForm.password} onChange={(e) => setLoginForm((s) => ({ ...s, password: e.target.value }))} />
        <button className="btn" onClick={async () => {
          try { await login(loginForm); navigate('/profile'); }
          catch { setMessage('Falha no login.'); }
        }}>Entrar</button>
      </article>

      <article className="card">
        <h2>Criar conta</h2>
        <input placeholder="Nome" value={registerForm.userName} onChange={(e) => setRegisterForm((s) => ({ ...s, userName: e.target.value }))} />
        <input placeholder="E-mail" value={registerForm.email} onChange={(e) => setRegisterForm((s) => ({ ...s, email: e.target.value }))} />
        <input type="password" placeholder="Senha" value={registerForm.password} onChange={(e) => setRegisterForm((s) => ({ ...s, password: e.target.value }))} />
        <input type="password" placeholder="Confirmar senha" value={registerForm.confirmPassword} onChange={(e) => setRegisterForm((s) => ({ ...s, confirmPassword: e.target.value }))} />
        <button className="btn" onClick={async () => {
          try { await register(registerForm); navigate('/profile'); }
          catch { setMessage('Falha no cadastro.'); }
        }}>Criar conta</button>
      </article>
      {message && <p className="hint">{message}</p>}
    </section>
  );
}
