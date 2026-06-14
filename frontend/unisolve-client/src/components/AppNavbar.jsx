import { Link, NavLink, useNavigate } from 'react-router-dom';
import { Navbar, Nav, Container, Button, NavDropdown } from 'react-bootstrap';
import { useAuth } from '../context/AuthContext';

export default function AppNavbar() {
  const { user, logout, isAdmin, isVerified } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (!user) {
    return (
      <Navbar expand="lg" className="chegg-navbar mb-0">
        <Container>
          <Navbar.Brand as={Link} to="/login" className="chegg-brand">UniSolve</Navbar.Brand>
        </Container>
      </Navbar>
    );
  }

  return (
    <Navbar expand="lg" className="chegg-navbar mb-0">
      <Container>
        <Navbar.Brand as={Link} to="/" className="chegg-brand">UniSolve</Navbar.Brand>
        <Navbar.Toggle aria-controls="navbar-nav" />
        <Navbar.Collapse id="navbar-nav">
          <Nav className="me-auto">
            <Nav.Link as={NavLink} to="/" end>Home</Nav.Link>
            <Nav.Link as={NavLink} to="/problems">Problems</Nav.Link>
            <Nav.Link as={NavLink} to="/problems/new">Post Problem</Nav.Link>
            <Nav.Link as={NavLink} to="/my-posts">My Posts</Nav.Link>
            {isAdmin && (
              <NavDropdown title="Admin" id="admin-nav-dropdown">
                <NavDropdown.Item as={NavLink} to="/admin/subjects">Subjects</NavDropdown.Item>
                <NavDropdown.Item as={NavLink} to="/admin/users">Users</NavDropdown.Item>
              </NavDropdown>
            )}
          </Nav>
          <Nav className="align-items-center">
            <Navbar.Text className="me-3 chegg-nav-user">
              {user.username}
              {isVerified && !isAdmin && (
                <span className="role-badge role-tutor ms-2">Tutor</span>
              )}
              {isAdmin && (
                <span className="role-badge role-admin ms-2">Admin</span>
              )}
            </Navbar.Text>
            <Button className="btn-chegg-outline" size="sm" onClick={handleLogout}>
              Logout
            </Button>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}
