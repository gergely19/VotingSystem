/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from 'react';
import { PollResponseDto } from '@/api/models/PollResponseDto';
import { getClosedPolls } from '@/api/client/polls-client';
import { LoadingIndicator } from '@/components/LoadingIndicator';
import { PollCard } from '@/components/polls/PollCard';

export function ClosedPollsPage() {
  const [polls, setPolls] = useState<PollResponseDto[]>([]);
  const [filtered, setFiltered] = useState<PollResponseDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  // filter mezők állapota
  const [text, setText] = useState('');
  const [start, setStart] = useState('');
  const [end, setEnd] = useState('');

  const user = localStorage.getItem('user');
  const currentUserId = user ? JSON.parse(user).userId : null;

  // betöltés
  useEffect(() => {
    (async () => {
      setIsLoading(true);
      try {
        const loaded = await getClosedPolls();
        const sorted = loaded.sort(
          (a, b) => new Date(a.endDate).getTime() - new Date(b.endDate).getTime()
        );
        setPolls(sorted);
        setFiltered(sorted); // kezdetben minden látszik
      } catch (error: any) {
          if (error?.status === 401) {
              window.location.href = '/user/login';
          } else {
              console.error('API hiba:', error);
          }
      } finally {
        setIsLoading(false);
      }
    })();
  }, []);

  // szűrés minden input-változáskor
  useEffect(filter, [text, start, end, polls]);

  function filter() {
    let pollList = polls;

    if (text)
      pollList = pollList.filter(p =>
        p.question.toLowerCase().includes(text.toLowerCase())
      );

    if (start)
      pollList = pollList.filter(p => new Date(p.startDate) >= new Date(start));

    if (end)
      pollList = pollList.filter(p => new Date(p.endDate) <= new Date(end));

    setFiltered(pollList);
  }

  if (isLoading) return <LoadingIndicator />;

  return (
    <>
      {/* Szűrő mezők */}
      <div className="d-flex gap-2 mb-3">
        <input
          type="text"
          className="form-control"
          placeholder="Kérdés szűrése…"
          value={text}
          onChange={e => setText(e.target.value)}
        />
        <input
          type="date"
          className="form-control"
          value={start}
          onChange={e => setStart(e.target.value)}
        />
        <input
          type="date"
          className="form-control"
          value={end}
          onChange={e => setEnd(e.target.value)}
        />
      </div>

      {/* Eredmény */}
      {filtered.length === 0 ? (
        <div className="alert alert-warning">Nincs poll.</div>
      ) : (
        <div className="d-flex flex-column gap-3">
          {filtered.map(p => (
            <PollCard key={p.id} poll={p} currentUserId={currentUserId} result={false} />
          ))}
        </div>
      )}
    </>
  );
}
