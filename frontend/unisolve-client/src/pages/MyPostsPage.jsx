import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Container, Spinner, Badge, Button, Alert } from 'react-bootstrap';
import { problemsApi } from '../services/api';
import ProblemCard from '../components/ProblemCard';

export default function MyPostsPage() {
  const navigate = useNavigate();
  const [problems, setProblems] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    problemsApi.getMine()
      .then(({ data }) => setProblems(data))
      .finally(() => setLoading(false));
  }, []);

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this problem and all its solutions?')) return;
    try {
      await problemsApi.delete(id);
      setProblems((prev) => prev.filter((p) => p.id !== id));
    } catch {
      alert('Failed to delete problem.');
    }
  };

  return (
    <Container>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>My Posts</h2>
        <Button as={Link} to="/problems/new" variant="primary">New Problem</Button>
      </div>

      {loading ? (
        <div className="text-center py-4"><Spinner animation="border" /></div>
      ) : problems.length === 0 ? (
        <Alert variant="info">
          You haven't posted any problems yet.{' '}
          <Link to="/problems/new">Post your first problem</Link>
        </Alert>
      ) : (
        problems.map((p) => (
          <div key={p.id} className="position-relative">
            <ProblemCard problem={p} />
            <div className="position-absolute top-0 end-0 m-3">
              <Button
                variant="outline-secondary"
                size="sm"
                className="me-2"
                onClick={() => navigate(`/problems/${p.id}/edit`)}
              >
                Edit
              </Button>
              <Button variant="outline-danger" size="sm" onClick={() => handleDelete(p.id)}>
                Delete
              </Button>
            </div>
          </div>
        ))
      )}
    </Container>
  );
}
