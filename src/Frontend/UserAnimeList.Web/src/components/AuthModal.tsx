import { useState, type FormEvent } from 'react';
import type { RegisterUserRequest } from '../types/api';

interface AuthModalProps {
  isOpen: boolean;
  loading: boolean;
  login: string;
  password: string;
  registerData: RegisterUserRequest;
  onClose: () => void;
  onLoginSubmit: (event: FormEvent) => Promise<void>;
  onRegisterSubmit: (event: FormEvent) => Promise<void>;
  onLoginChange: (value: string) => void;
  onPasswordChange: (value: string) => void;
  onRegisterChange: (value: RegisterUserRequest) => void;
}

export function AuthModal(props: AuthModalProps) {
  const [mode, setMode] = useState<'login' | 'register'>('login');

  if (!props.isOpen) return null;

  return (
    <div className="modal-overlay" onClick={props.onClose}>
      <div className="modal card" onClick={(event) => event.stopPropagation()}>
        <div className="modal-header">
          <div className="stack inline">
            <button type="button" className={mode === 'login' ? '' : 'btn-ghost'} onClick={() => setMode('login')}>
              Entrar
            </button>
            <button type="button" className={mode === 'register' ? '' : 'btn-ghost'} onClick={() => setMode('register')}>
              Cadastrar
            </button>
          </div>
          <button type="button" className="btn-ghost" onClick={props.onClose}>
            Fechar
          </button>
        </div>

        {mode === 'login' ? (
          <form onSubmit={(event) => void props.onLoginSubmit(event)} className="stack">
            <input
              placeholder="Usuário ou e-mail"
              value={props.login}
              onChange={(event) => props.onLoginChange(event.target.value)}
              required
            />
            <input
              type="password"
              placeholder="Senha"
              value={props.password}
              onChange={(event) => props.onPasswordChange(event.target.value)}
              required
            />
            <button type="submit" disabled={props.loading}>
              Entrar
            </button>
          </form>
        ) : (
          <form onSubmit={(event) => void props.onRegisterSubmit(event)} className="stack">
            <input
              placeholder="Nome de usuário"
              value={props.registerData.userName}
              onChange={(event) => props.onRegisterChange({ ...props.registerData, userName: event.target.value })}
              required
            />
            <input
              placeholder="E-mail"
              type="email"
              value={props.registerData.email}
              onChange={(event) => props.onRegisterChange({ ...props.registerData, email: event.target.value })}
              required
            />
            <input
              placeholder="Senha"
              type="password"
              value={props.registerData.password}
              onChange={(event) => props.onRegisterChange({ ...props.registerData, password: event.target.value })}
              required
            />
            <input
              placeholder="Confirmar senha"
              type="password"
              value={props.registerData.confirmPassword}
              onChange={(event) => props.onRegisterChange({ ...props.registerData, confirmPassword: event.target.value })}
              required
            />
            <button type="submit" disabled={props.loading}>
              Criar conta
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
