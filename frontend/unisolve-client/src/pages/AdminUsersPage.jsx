import { useEffect, useState } from 'react';
import { Container, Table, Form, Alert, Spinner } from 'react-bootstrap';
import { usersApi } from '../services/api';

const ROLE_OPTIONS = [
  { value: 'Student', label: 'Student' },
  { value: 'Verified', label: 'Tutor' },
  { value: 'Admin', label: 'Admin' },
];

function roleLabel(role) {
  if (role === 'Verified') return 'Tutor';
  return role;
}

export default function AdminUsersPage() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [updatingId, setUpdatingId] = useState(null);

  const loadUsers = () => {
    setLoading(true);
    usersApi.getAll()
      .then(({ data }) => setUsers(data))
      .catch(() => setError('Failed to load users.'))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadUsers();
  }, []);

  const handleRoleChange = async (userId, role) => {
    setUpdatingId(userId);
    setError('');
    try {
      const { data } = await usersApi.updateRole(userId, role);
      setUsers((prev) => prev.map((u) => (u.id === userId ? data : u)));
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to update role.');
    } finally {
      setUpdatingId(null);
    }
  };

  return (
    <Container>
      <h2 className="page-title mb-4">Manage Users</h2>
      <p className="text-muted mb-4">
        Assign roles: <strong>Student</strong> (default), <strong>Tutor</strong> (can post solutions), or <strong>Admin</strong>.
      </p>

      {error && <Alert variant="danger" dismissible onClose={() => setError('')}>{error}</Alert>}

      {loading ? (
        <div className="text-center py-4"><Spinner animation="border" /></div>
      ) : (
        <Table striped bordered hover responsive className="bg-white">
          <thead>
            <tr>
              <th>Username</th>
              <th>Email</th>
              <th>Joined</th>
              <th>Role</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id}>
                <td>{u.username}</td>
                <td>{u.email}</td>
                <td>{new Date(u.createdAt).toLocaleDateString()}</td>
                <td style={{ minWidth: 160 }}>
                  <Form.Select
                    size="sm"
                    value={u.role}
                    disabled={updatingId === u.id}
                    onChange={(e) => handleRoleChange(u.id, e.target.value)}
                  >
                    {ROLE_OPTIONS.map((opt) => (
                      <option key={opt.value} value={opt.value}>{opt.label}</option>
                    ))}
                  </Form.Select>
                  <small className="text-muted">Current: {roleLabel(u.role)}</small>
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </Container>
  );
}
