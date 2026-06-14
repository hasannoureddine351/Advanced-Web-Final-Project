import { useEffect, useState } from 'react';
import { Container, Form, Row, Col, Spinner, Pagination } from 'react-bootstrap';
import ProblemCard from '../components/ProblemCard';
import { problemsApi, subjectsApi } from '../services/api';

export default function ProblemsPage() {
  const [problems, setProblems] = useState([]);
  const [subjects, setSubjects] = useState([]);
  const [subjectId, setSubjectId] = useState('');
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const pageSize = 10;

  useEffect(() => {
    subjectsApi.getAll().then(({ data }) => setSubjects(data));
  }, []);

  useEffect(() => {
    setLoading(true);
    const params = { page, pageSize };
    if (subjectId) params.subjectId = subjectId;
    if (search.trim()) params.search = search.trim();

    problemsApi.getAll(params)
      .then(({ data }) => {
        setProblems(data.items);
        setTotalCount(data.totalCount);
      })
      .finally(() => setLoading(false));
  }, [subjectId, search, page]);

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <Container>
      <h2 className="page-title mb-4">All Questions</h2>

      <Row className="mb-4 g-3">
        <Col md={4}>
          <Form.Select
            value={subjectId}
            onChange={(e) => { setSubjectId(e.target.value); setPage(1); }}
          >
            <option value="">All Subjects</option>
            {subjects.map((s) => (
              <option key={s.id} value={s.id}>{s.name} ({s.code})</option>
            ))}
          </Form.Select>
        </Col>
        <Col md={8}>
          <Form.Control
            type="search"
            placeholder="Search by title or description..."
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
          />
        </Col>
      </Row>

      {loading ? (
        <div className="text-center py-4"><Spinner animation="border" /></div>
      ) : problems.length === 0 ? (
        <p className="text-muted">No problems found.</p>
      ) : (
        problems.map((p) => <ProblemCard key={p.id} problem={p} />)
      )}

      {totalPages > 1 && (
        <Pagination className="justify-content-center">
          <Pagination.Prev disabled={page === 1} onClick={() => setPage(page - 1)} />
          {[...Array(totalPages)].map((_, i) => (
            <Pagination.Item
              key={i + 1}
              active={page === i + 1}
              onClick={() => setPage(i + 1)}
            >
              {i + 1}
            </Pagination.Item>
          ))}
          <Pagination.Next disabled={page === totalPages} onClick={() => setPage(page + 1)} />
        </Pagination>
      )}
    </Container>
  );
}
