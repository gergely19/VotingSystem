/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { PollResponseDto } from "@/api/models/PollResponseDto";
import { getPoll } from "@/api/client/polls-client";
import { vote } from "@/api/client/vote-client";
import { ErrorAlert } from "@/components/alerts/ErrorAlert";
import { LoadingIndicator } from "@/components/LoadingIndicator";
import { PollDetails } from "@/components/polls/PollDetails"; // vagy az elérési út

export function ClosedPollPage() {
  const { pollId } = useParams();
  const [poll, setPoll] = useState<PollResponseDto | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isVoting, setIsVoting] = useState(false);
  const [selectedOptionId, setSelectedOptionId] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    async function loadPoll() {
      if (!pollId) return;

      setIsLoading(true);
      setError(null);

      try {
        const loadedPoll = await getPoll(pollId);
        setPoll(loadedPoll);
        setSelectedOptionId(null);
      } catch (error: any) {
          if (error?.status === 401) {
              window.location.href = '/user/login';
          } else {
              console.error('API hiba:', error);
          }
      } finally {
        setIsLoading(false);
      }
    }

    loadPoll();
  }, [pollId]);

  const user = localStorage.getItem("user");
  const currentUserId = user ? JSON.parse(user).userId : null;

  const hasVoted =
    poll?.userPolls?.some(
      (up) => up.userId === currentUserId && up.hasVoted
    ) ?? false;

  async function handleVoteSubmit() {
    if (!pollId || isVoting || hasVoted || !selectedOptionId) return;

    setIsVoting(true);
    setError(null);

    try {
      await vote({ optionId: selectedOptionId });
      // Újratöltjük a pollt a friss adatokért
      const updatedPoll = await getPoll(pollId);
      setPoll(updatedPoll);
      setSelectedOptionId(null);
      navigate("/polls");
    } catch (e) {
      setError(e instanceof Error ? e.message : "Hiba történt szavazás közben.");
    } finally {
      setIsVoting(false);
    }
  }

  if (isLoading) return <LoadingIndicator />;

  return (
    <>
      {error && <ErrorAlert message={error} />}
      {poll ? (
        <PollDetails
          poll={poll}
          currentUserId={currentUserId}
          selectedOptionId={selectedOptionId}
          setSelectedOptionId={setSelectedOptionId}
          hasVoted={hasVoted}
          isVoting={isVoting}
          onVoteSubmit={handleVoteSubmit}
          closed={true}
        />
      ) : (
        !error && <div className="alert alert-warning">Nem található a szavazás.</div>
      )}
    </>
  );
}
