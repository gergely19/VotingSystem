import { PollResponseDto } from "@/api/models/PollResponseDto";

interface PollDetailsProps {
  poll: PollResponseDto;
  currentUserId: string | null;
  selectedOptionId: string | null;
  setSelectedOptionId: (optionId: string | null) => void;
  hasVoted: boolean;
  isVoting: boolean;
  onVoteSubmit: () => void;
  closed: boolean;
}

export function PollDetails({
  poll,
  currentUserId,
  selectedOptionId,
  setSelectedOptionId,
  hasVoted,
  isVoting,
  onVoteSubmit,
  closed
}: PollDetailsProps) {
  // Összes szavazat számítása
  const totalVotes = poll.options?.reduce((sum, option) => sum + option.voteCount, 0) ?? 0;

  return (
    <div className="row">
      <div className="col-md-12">
        <div className="card shadow mb-4">
          <div className="card-body">
            <h2 className="card-title h4">{poll.question}</h2>
            <ul className="list-unstyled mb-4">
              <li>
                <strong>Kezdés:</strong>{" "}
                {new Date(poll.startDate).toLocaleString("hu-HU")}
              </li>
              <li>
                <strong>Vége:</strong>{" "}
                {new Date(poll.endDate).toLocaleString("hu-HU")}
              </li>
            </ul>

            <h5>Válaszlehetőségek:</h5>
            <ul className="list-group">
              {poll.options?.map((option) => {
                // Százalék számítása, ha van szavazat, különben 0
                const percent = totalVotes > 0 ? (option.voteCount / totalVotes) * 100 : 0;
                return (
                    <li
                    key={option.id}
                    className={`list-group-item d-flex align-items-center ${
                        selectedOptionId === option.id ? "active" : ""
                    }`}
                    style={{ cursor: hasVoted ? "default" : "pointer" }}
                    onClick={() => !hasVoted && setSelectedOptionId(option.id)}
                    >
                    <span>{option.text}</span>
                    {closed && (
                        <div className="ms-auto d-flex align-items-center">
                        <span className="badge bg-primary me-2">
                            {option.voteCount} szavazat
                        </span>
                        <span className="badge bg-success" style={{ minWidth: "50px", textAlign: "center", display: "inline-block" }}>
                            {percent.toFixed(1)}%
                        </span>
                        </div>
                    )}
                    </li>

                );
              })}
            </ul>

            {!hasVoted && !closed && (
              <button
                className="btn btn-primary mt-3"
                onClick={onVoteSubmit}
                disabled={isVoting || !selectedOptionId}
              >
                Szavazat elküldése
              </button>
            )}

            {hasVoted && !closed && (
              <div className="alert alert-info mt-3">
                Már szavaztál erre a szavazásra.
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
