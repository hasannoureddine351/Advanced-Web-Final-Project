import { Link } from 'react-router-dom';
import { Card, Badge } from 'react-bootstrap';

export default function ProblemCard({ problem }) {
  return (
    <Card className="chegg-card mb-3">
      <Card.Body>
        <div className="d-flex justify-content-between align-items-start">
          <div>
            <Card.Title as={Link} to={`/problems/${problem.id}`} className="text-decoration-none">
              {problem.title}
            </Card.Title>
            <Card.Text className="text-muted small mb-2">
              {problem.description.length > 150
                ? `${problem.description.substring(0, 150)}...`
                : problem.description}
            </Card.Text>
            <div className="d-flex flex-wrap gap-2">
              <Badge className="badge-subject">{problem.subjectName}</Badge>
              {problem.bookReference && (
                <Badge className="badge-meta">{problem.bookReference}</Badge>
              )}
              {problem.problemNumber && (
                <Badge className="badge-meta">#{problem.problemNumber}</Badge>
              )}
            </div>
          </div>
          <div className="text-end text-muted small ms-3">
            <div>{problem.solutionCount} answer{problem.solutionCount !== 1 ? 's' : ''}</div>
            <div>by {problem.authorUsername}</div>
            <div>{new Date(problem.createdAt).toLocaleDateString()}</div>
          </div>
        </div>
      </Card.Body>
    </Card>
  );
}
