import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Toast } from './components/Toast';
import { AuthProvider } from './contexts/AuthContext';
import { AdminPage } from './pages/AdminPage';
import { AnimeDetailsPage } from './pages/AnimeDetailsPage';
import { AuthPage } from './pages/AuthPage';
import { HomePage } from './pages/HomePage';
import { ProfilePage } from './pages/ProfilePage';
import { SearchPage } from './pages/SearchPage';
import { StudioPage } from './pages/StudioPage';

export function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Layout>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/auth" element={<AuthPage />} />
            <Route path="/profile" element={<ProfilePage />} />
            <Route path="/search" element={<SearchPage />} />
            <Route path="/anime/:id" element={<AnimeDetailsPage />} />
            <Route path="/studio/:id" element={<StudioPage />} />
            <Route path="/admin" element={<AdminPage />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
          <Toast message="" />
        </Layout>
      </BrowserRouter>
    </AuthProvider>
  );
}
