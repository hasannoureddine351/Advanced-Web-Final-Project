import { useEffect, useState } from 'react';
import { Container, Table, Button, Modal, Form, Alert, Spinner } from 'react-bootstrap';
import { subjectsApi } from '../services/api';

export default function AdminSubjectsPage() {
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null);
  const [name, setName] = useState('');
  const [code, setCode] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const loadSubjects = () => {
    setLoading(true);
    subjectsApi.getAll()
      .then(({ data }) => setSubjects(data))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadSubjects();
  }, []);

  const openCreate = () => {
    setEditing(null);
    setName('');
    setCode('');
    setShowModal(true);
  };

  const openEdit = (subject) => {
    setEditing(subject);
    setName(subject.name);
    setCode(subject.code);
    setShowModal(true);
  };

  const handleSave = async () => {
    if (!name.trim() || !code.trim()) {
      setError('Name and code are required.');
      return;
    }
    setSubmitting(true);
    setError('');
    try {
      if (editing) {
        await subjectsApi.update(editing.id, { name, code });
      } else {
        await subjectsApi.create({ name, code });
      }
      setShowModal(false);
      loadSubjects();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save subject.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this subject? It must have no associated problems.')) return;
    try {
      await subjectsApi.delete(id);
      loadSubjects();
    } catch (err) {
      alert(err.response?.data?.message || 'Cannot delete subject with associated problems.');
    }
  };

  return (
    <Container>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="page-title">Manage Subjects</h2>
        <Button className="btn-chegg" onClick={openCreate}>Add Subject</Button>
      </div>

      {error && <Alert variant="danger" dismissible onClose={() => setError('')}>{error}</Alert>}

      {loading ? (
        <div className="text-center py-4"><Spinner animation="border" /></div>
      ) : (
        <Table striped bordered hover responsive>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Code</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {subjects.map((s) => (
              <tr key={s.id}>
                <td>{s.id}</td>
                <td>{s.name}</td>
                <td>{s.code}</td>
                <td>
                  <Button variant="outline-primary" size="sm" className="me-2" onClick={() => openEdit(s)}>
                    Edit
                  </Button>
                  <Button variant="outline-danger" size="sm" onClick={() => handleDelete(s.id)}>
                    Delete
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>{editing ? 'Edit Subject' : 'Add Subject'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group className="mb-3">
            <Form.Label>Name</Form.Label>
            <Form.Control value={name} onChange={(e) => setName(e.target.value)} />
          </Form.Group>
          <Form.Group>
            <Form.Label>Code</Form.Label>
            <Form.Control value={code} onChange={(e) => setCode(e.target.value)} />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>Cancel</Button>
          <Button variant="primary" onClick={handleSave} disabled={submitting}>
            {submitting ? 'Saving...' : 'Save'}
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
}
