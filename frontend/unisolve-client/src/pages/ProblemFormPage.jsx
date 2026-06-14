import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Container, Card, Form, Button, Alert, Spinner } from 'react-bootstrap';
import { problemsApi, subjectsApi } from '../services/api';

export default function ProblemFormPage() {
  const { id } = useParams();
  const isEdit = Boolean(id);
  const navigate = useNavigate();

  const [subjects, setSubjects] = useState([]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [bookReference, setBookReference] = useState('');
  const [problemNumber, setProblemNumber] = useState('');
  const [subjectId, setSubjectId] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(isEdit);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    subjectsApi.getAll().then(({ data }) => {
      setSubjects(data);
      if (!isEdit && data.length > 0) setSubjectId(String(data[0].id));
    });
  }, [isEdit]);

  useEffect(() => {
    if (!isEdit) return;
    problemsApi.getById(id)
      .then(({ data }) => {
        setTitle(data.title);
        setDescription(data.description);
        setBookReference(data.bookReference || '');
        setProblemNumber(data.problemNumber || '');
        setSubjectId(String(data.subjectId));
      })
      .catch(() => setError('Problem not found.'))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const validate = () => {
    if (!title.trim()) return 'Title is required.';
    if (description.trim().length < 10) return 'Description must be at least 10 characters.';
    if (!subjectId) return 'Please select a subject.';
    return '';
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }

    setSubmitting(true);
    setError('');

    const payload = {
      title: title.trim(),
      description: description.trim(),
      bookReference: bookReference.trim() || null,
      problemNumber: problemNumber.trim() || null,
      subjectId: parseInt(subjectId, 10),
    };

    try {
      if (isEdit) {
        await problemsApi.update(id, payload);
        navigate(`/problems/${id}`);
      } else {
        const { data } = await problemsApi.create(payload);
        navigate(`/problems/${data.id}`);
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save problem.');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <Container className="text-center py-5">
        <Spinner animation="border" />
      </Container>
    );
  }

  return (
    <Container style={{ maxWidth: 720 }}>
      <h2 className="mb-4">{isEdit ? 'Edit Problem' : 'Post a Problem'}</h2>
      <Card className="shadow-sm">
        <Card.Body>
          {error && <Alert variant="danger">{error}</Alert>}
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Title</Form.Label>
              <Form.Control
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                maxLength={200}
                required
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Description</Form.Label>
              <Form.Control
                as="textarea"
                rows={5}
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Describe the problem in detail..."
                required
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Subject</Form.Label>
              <Form.Select value={subjectId} onChange={(e) => setSubjectId(e.target.value)} required>
                <option value="">Select subject...</option>
                {subjects.map((s) => (
                  <option key={s.id} value={s.id}>{s.name} ({s.code})</option>
                ))}
              </Form.Select>
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Book Reference (optional)</Form.Label>
              <Form.Control
                value={bookReference}
                onChange={(e) => setBookReference(e.target.value)}
                placeholder="e.g. Stewart Calculus 8th Ed"
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Problem Number (optional)</Form.Label>
              <Form.Control
                value={problemNumber}
                onChange={(e) => setProblemNumber(e.target.value)}
                placeholder="e.g. 4.2 #15"
              />
            </Form.Group>
            <div className="d-flex gap-2">
              <Button type="submit" variant="primary" disabled={submitting}>
                {submitting ? 'Saving...' : isEdit ? 'Update Problem' : 'Post Problem'}
              </Button>
              <Button variant="secondary" onClick={() => navigate(-1)}>Cancel</Button>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
}
