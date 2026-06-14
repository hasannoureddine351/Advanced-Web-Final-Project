import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { Container, Row, Col, Button, Spinner } from 'react-bootstrap';
import ProblemCard from '../components/ProblemCard';
import { problemsApi } from '../services/api';

export default function HomePage() {
  const [problems, setProblems] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    problemsApi.getAll({ page: 1, pageSize: 5 })
      .then(({ data }) => setProblems(data.items))
      .finally(() => setLoading(false));
  }, []);

  return (
    <Container>
      <Row className="align-items-center mb-4">
        <Col>
          <h1 className="page-title">UniSolve</h1>
          <p className="lead text-muted">
            Get step-by-step help with your university problems. Top answers rise to the top.
          </p>
        </Col>
        <Col xs="auto">
          <Button as={Link} to="/problems/new" className="btn-chegg">Ask a Question</Button>
        </Col>
      </Row>

      <h4>Recent Problems</h4>
      {loading ? (
        <div className="text-center py-4"><Spinner animation="border" /></div>
      ) : problems.length === 0 ? (
        <p className="text-muted">No problems yet. Be the first to post!</p>
      ) : (
        problems.map((p) => <ProblemCard key={p.id} problem={p} />)
      )}

      <div className="text-center mt-3">
        <Button as={Link} to="/problems" variant="outline-primary">Browse All Questions</Button>
      </div>
    </Container>
  );
}
