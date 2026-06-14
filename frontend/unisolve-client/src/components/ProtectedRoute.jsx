import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Spinner, Container } from 'react-bootstrap';

export default function ProtectedRoute({ children, adminOnly = false, verifiedOnly = false }) {
  const { user, loading, isAdmin, isVerified } = useAuth();
  const location = useLocation();

  if (loading) {
    return (
      <Container className="text-center py-5">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (!user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (adminOnly && !isAdmin) {
    return <Navigate to="/" replace />;
  }

  if (verifiedOnly && !isVerified) {
    return <Navigate to="/" replace />;
  }

  return children;
}
