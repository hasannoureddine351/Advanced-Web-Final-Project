import { useState } from 'react';
import { Card, Button, Modal, Form, Alert } from 'react-bootstrap';
import { useAuth } from '../context/AuthContext';
import { solutionsApi, votesApi } from '../services/api';

export default function SolutionItem({ solution, onUpdate, onDelete }) {
  const { user, isVerified } = useAuth();
  const [showEdit, setShowEdit] = useState(false);
  const [content, setContent] = useState(solution.content);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const canEdit = user && (user.id === solution.authorId || ['Moderator', 'Admin'].includes(user.role));
  const canVote = user && user.id !== solution.authorId;

  const handleVote = async (value) => {
    if (!canVote) return;
    setLoading(true);
    try {
      const { data } = solution.userVote === value
        ? await votesApi.remove(solution.id)
        : await votesApi.vote(solution.id, value);
      onUpdate(data);
    } catch (err) {
      setError(err.response?.data?.message || 'Vote failed');
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    if (content.trim().length < 10) {
      setError('Solution must be at least 10 characters.');
      return;
    }
    setLoading(true);
    try {
      const { data } = await solutionsApi.update(solution.id, { content });
      onUpdate(data);
      setShowEdit(false);
      setError('');
    } catch (err) {
      setError(err.response?.data?.message || 'Update failed');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Delete this solution?')) return;
    try {
      await solutionsApi.delete(solution.id);
      onDelete(solution.id);
    } catch (err) {
      setError(err.response?.data?.message || 'Delete failed');
    }
  };

  return (
    <>
      <Card className="chegg-card mb-3">
        <Card.Body>
          {error && <Alert variant="danger" dismissible onClose={() => setError('')}>{error}</Alert>}
          <div className="d-flex">
            <div className="vote-widget me-3">
              <Button
                className={`vote-btn ${solution.userVote === 1 ? 'vote-btn-active-up' : ''}`}
                onClick={() => handleVote(1)}
                disabled={!canVote || loading}
                aria-label="Upvote"
              >
                ▲
              </Button>
              <div className="vote-score">{solution.score}</div>
              <Button
                className={`vote-btn ${solution.userVote === -1 ? 'vote-btn-active-down' : ''}`}
                onClick={() => handleVote(-1)}
                disabled={!canVote || loading}
                aria-label="Downvote"
              >
                ▼
              </Button>
            </div>
            <div className="flex-grow-1">
              <div className="d-flex justify-content-between mb-2">
                <strong className="solution-author">{solution.authorUsername}</strong>
                <small className="text-muted">
                  {new Date(solution.createdAt).toLocaleDateString()}
                </small>
              </div>
              <p className="mb-0 solution-content">{solution.content}</p>
              {canEdit && isVerified && (
                <div className="mt-3">
                  <Button variant="outline-secondary" size="sm" className="me-2" onClick={() => setShowEdit(true)}>
                    Edit
                  </Button>
                  <Button variant="outline-danger" size="sm" onClick={handleDelete}>
                    Delete
                  </Button>
                </div>
              )}
            </div>
          </div>
        </Card.Body>
      </Card>

      <Modal show={showEdit} onHide={() => setShowEdit(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Edit Solution</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Control
            as="textarea"
            rows={6}
            value={content}
            onChange={(e) => setContent(e.target.value)}
          />
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowEdit(false)}>Cancel</Button>
          <Button className="btn-chegg" onClick={handleSave} disabled={loading}>Save</Button>
        </Modal.Footer>
      </Modal>
    </>
  );
}
