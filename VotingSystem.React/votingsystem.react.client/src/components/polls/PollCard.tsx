import { PollResponseDto } from '@/api/models/PollResponseDto';
import { Link } from "react-router-dom";

interface PollCardProps {
  poll: PollResponseDto;
  currentUserId: string | null;
  result: boolean;
}

export function PollCard({ poll, currentUserId, result }: PollCardProps) {
  const hasVoted = poll.userPolls?.some(up => up.userId === currentUserId && up.hasVoted) ?? false;

  return (
    <div className="card shadow">
      <Link className="stretched-link text-dark text-underline-hover" to={`${poll.id}`}>
        <div className="card-body">
          <h2 className="card-title h4 d-flex justify-content-between align-items-center">
            {poll.question}
            {hasVoted && result && (
              <span className="badge bg-success ms-2">Már szavaztál</span>
            )}
          </h2>
          <p>
            <strong>Kezdés:</strong>{' '}
            {new Date(poll.startDate).toLocaleString('hu-HU', {
              year: 'numeric',
              month: 'long',
              day: 'numeric',
              hour: '2-digit',
              minute: '2-digit',
            })}
          </p>
          <p>
            <strong>Vége:</strong>{' '}
            {new Date(poll.endDate).toLocaleString('hu-HU', {
              year: 'numeric',
              month: 'long',
              day: 'numeric',
              hour: '2-digit',
              minute: '2-digit',
            })}
          </p>
        </div>
      </Link>
    </div>
  );
}
