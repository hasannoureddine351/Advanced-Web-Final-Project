import { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { Container, Badge, Spinner, Alert, Form, Button, Card } from 'react-bootstrap';
import { useAuth } from '../context/AuthContext';
import { problemsApi, solutionsApi } from '../services/api';
import SolutionItem from '../components/SolutionItem';

export default function ProblemDetailPage() {
  const { id } = useParams();
  const { user, isVerified } = useAuth();
  const navigate = useNavigate();

  const [problem, setProblem] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [solutionContent, setSolutionContent] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const loadProblem = () => {
    setLoading(true);
    problemsApi.getById(id)
      .then(({ data }) => setProblem(data))
      .catch(() => setError('Problem not found.'))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadProblem();
  }, [id]);

  const handleSolutionUpdate = (updated) => {
    setProblem((prev) => ({
      ...prev,
      solutions: prev.solutions.map((s) => (s.id === updated.id ? updated : s)),
    }));
  };

  const handleSolutionDelete = (solutionId) => {
    setProblem((prev) => ({
      ...prev,
      solutions: prev.solutions.filter((s) => s.id !== solutionId),
    }));
  };

  const handleSubmitSolution = async (e) => {
    e.preventDefault();
    if (solutionContent.trim().length < 10) {
      setError('Solution must be at least 10 characters.');
      return;
    }
    setSubmitting(true);
    setError('');
    try {
      const { data } = await solutionsApi.create(id, { content: solutionContent });
      setProblem((prev) => ({
        ...prev,
        solutions: [data, ...prev.solutions].sort((a, b) => b.score - a.score || new Date(b.createdAt) - new Date(a.createdAt)),
      }));
      setSolutionContent('');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to post solution.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteProblem = async () => {
    if (!window.confirm('Delete this problem?')) return;
    try {
      await problemsApi.delete(id);
      navigate('/problems');
    } catch {
      setError('Failed to delete problem.');
    }
  };

  if (loading) {
    return (
      <Container className="text-center py-5">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (!problem) {
    return (
      <Container>
        <Alert variant="danger">{error || 'Problem not found.'}</Alert>
        <Link to="/problems">Back to problems</Link>
      </Container>
    );
  }

  const canEdit = user && (user.id === problem.authorId || user.role === 'Admin');

  return (
    <Container>
      <div className="mb-4">
        <Link to="/problems" className="text-decoration-none">&larr; Back to problems</Link>
      </div>

      <Card className="chegg-card problem-header-card mb-4">
        <Card.Body>
          <div className="d-flex justify-content-between align-items-start">
            <div>
              <h2 className="page-title">{problem.title}</h2>
              <div className="mb-3">
                <Badge className="badge-subject me-2">{problem.subjectName}</Badge>
                {problem.bookReference && (
                  <Badge className="badge-meta me-2">{problem.bookReference}</Badge>
                )}
                {problem.problemNumber && (
                  <Badge className="badge-meta">#{problem.problemNumber}</Badge>
                )}
              </div>
            </div>
            {canEdit && (
              <div>
                <Button
                  variant="outline-primary"
                  size="sm"
                  className="me-2"
                  onClick={() => navigate(`/problems/${id}/edit`)}
                >
                  Edit
                </Button>
                <Button variant="outline-danger" size="sm" onClick={handleDeleteProblem}>
                  Delete
                </Button>
              </div>
            )}
          </div>
          <p style={{ whiteSpace: 'pre-wrap' }}>{problem.description}</p>
          <small className="text-muted">
            Posted by {problem.authorUsername} on {new Date(problem.createdAt).toLocaleDateString()}
          </small>
        </Card.Body>
      </Card>

      <h4 className="page-title">Expert Answers ({problem.solutions.length})</h4>

      {error && <Alert variant="danger" dismissible onClose={() => setError('')}>{error}</Alert>}

      {isVerified && (
        <Card className="chegg-card solution-form-card mb-4">
          <Card.Body>
            <h5>Post an Answer</h5>
            <Form onSubmit={handleSubmitSolution}>
              <Form.Group className="mb-3">
                <Form.Control
                  as="textarea"
                  rows={4}
                  placeholder="Write your step-by-step solution..."
                  value={solutionContent}
                  onChange={(e) => setSolutionContent(e.target.value)}
                />
              </Form.Group>
              <Button type="submit" className="btn-chegg" disabled={submitting}>
                {submitting ? 'Posting...' : 'Submit Answer'}
              </Button>
            </Form>
          </Card.Body>
        </Card>
      )}

      {!isVerified && (
        <Alert variant="warning">
          Only tutors can post answers. Contact an admin to get tutor access.
        </Alert>
      )}

      {problem.solutions.length === 0 ? (
        <Alert variant="secondary">No solutions yet. Be the first to help!</Alert>
      ) : (
        problem.solutions.map((s) => (
          <SolutionItem
            key={s.id}
            solution={s}
            onUpdate={handleSolutionUpdate}
            onDelete={handleSolutionDelete}
          />
        ))
      )}
    </Container>
  );
}
