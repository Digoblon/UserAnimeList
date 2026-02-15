import type { FormEvent } from 'react';
import type { RegisterUserRequest } from '../types/api';

interface AuthPanelProps {
  login: string;
  password: string;
  registerData: RegisterUserRequest;
  loading: boolean;
  onLoginSubmit: (event: FormEvent) => Promise<void>;
  onRegisterSubmit: (event: FormEvent) => Promise<void>;
  onLoginChange: (value: string) => void;
  onPasswordChange: (value: string) => void;
  onRegisterChange: (value: RegisterUserRequest) => void;
}

export function AuthPanel({
  login,
  password,
  registerData,
  loading,
  onLoginSubmit,
  onRegisterSubmit,
  onLoginChange,
  onPasswordChange,
  onRegisterChange
}: AuthPanelProps) {
  return (
    <section className="grid two-columns">
      <article className="card">
        <h2>Entrar</h2>
        <p>Acesse seus dados e sua lista pessoal.</p>
        <form onSubmit={(event) => void onLoginSubmit(event)} className="stack">
          <input
            placeholder="Usuário ou e-mail"
            value={login}
            onChange={(event) => onLoginChange(event.target.value)}
            required
          />
          <input
            type="password"
            placeholder="Senha"
            value={password}
            onChange={(event) => onPasswordChange(event.target.value)}
            required
          />
          <button type="submit" disabled={loading}>
            Entrar
          </button>
        </form>
      </article>

      <article className="card">
        <h2>Criar conta</h2>
        <p>Cadastro rápido para testar todos os fluxos da API.</p>
        <form onSubmit={(event) => void onRegisterSubmit(event)} className="stack">
          <input
            placeholder="Nome de usuário"
            value={registerData.userName}
            onChange={(event) => onRegisterChange({ ...registerData, userName: event.target.value })}
            required
          />
          <input
            placeholder="E-mail"
            type="email"
            value={registerData.email}
            onChange={(event) => onRegisterChange({ ...registerData, email: event.target.value })}
            required
          />
          <input
            placeholder="Senha"
            type="password"
            value={registerData.password}
            onChange={(event) => onRegisterChange({ ...registerData, password: event.target.value })}
            required
          />
          <input
            placeholder="Confirmar senha"
            type="password"
            value={registerData.confirmPassword}
            onChange={(event) => onRegisterChange({ ...registerData, confirmPassword: event.target.value })}
            required
          />
          <button type="submit" disabled={loading}>
            Cadastrar
          </button>
        </form>
      </article>
    </section>
  );
}
