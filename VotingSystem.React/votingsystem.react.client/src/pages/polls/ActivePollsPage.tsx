import { useEffect, useState } from 'react';
import { PollResponseDto } from '@/api/models/PollResponseDto';
import { getActivePolls } from '@/api/client/polls-client';
import { LoadingIndicator } from '@/components/LoadingIndicator';
import { PollCard } from '@/components/polls/PollCard';

export function PollsPage() {
  const [polls, setPolls] = useState<PollResponseDto[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const user = localStorage.getItem('user');
  const currentUserId = user ? JSON.parse(user).userId : null;

  useEffect(() => {
    async function loadContent() {
      setIsLoading(true);
      try {
        const loadedPolls = await getActivePolls();
        const sortedPolls = [...loadedPolls].sort(
          (a, b) =>
            new Date(a.endDate).getTime() - new Date(b.endDate).getTime()
        );
        setPolls(sortedPolls);
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
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
    loadContent();
  }, []);

  if (isLoading) {
    return <LoadingIndicator />;
  }

  if (polls.length === 0) {
    return <div className="alert alert-warning">Nincs poll.</div>;
  }

  return (
    <div className="d-flex flex-column gap-3">
      {polls.map((poll) => (
        <PollCard
          key={poll.id}
          poll={poll}
          currentUserId={currentUserId}
          result={true}
        />
      ))}
    </div>
  );
}
