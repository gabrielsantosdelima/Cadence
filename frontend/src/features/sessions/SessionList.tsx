import { ApiError } from '../../api/problem'
import { EmptyState, ErrorState, Spinner } from '../../components/states'
import { focusLabel } from '../../domain/display'
import { useSessions } from './useSessions'

export interface SessionListProps {
  pieceId: string
}

export function SessionList({ pieceId }: SessionListProps) {
  const { data: sessions, isPending, error } = useSessions({ pieceId })

  if (isPending) {
    return (
      <div className="flex justify-center py-6">
        <Spinner />
      </div>
    )
  }

  if (error) {
    return (
      <ErrorState
        error={error instanceof ApiError ? error : new ApiError(0, error.message)}
      />
    )
  }

  if (sessions.length === 0) {
    return <EmptyState message="No sessions logged yet." />
  }

  return (
    <ul className="flex flex-col gap-2">
      {sessions.map((session) => (
        <li
          key={session.id}
          className="rounded-lg border border-slate-200 bg-white p-3 text-sm"
        >
          <div className="flex items-center justify-between gap-2">
            <span className="font-medium text-slate-900">
              {new Date(session.startedAtUtc).toLocaleString(undefined, {
                month: 'short',
                day: 'numeric',
                hour: 'numeric',
                minute: '2-digit',
              })}
            </span>
            <span className="text-slate-500">{session.durationMinutes} min</span>
          </div>
          <div className="mt-1 flex flex-wrap gap-x-3 text-slate-600">
            <span>{focusLabel(session.focus)}</span>
            <span>quality {session.quality}</span>
            {session.tempoBpm != null ? <span>{session.tempoBpm} bpm</span> : null}
          </div>
          {session.notes ? <p className="mt-1 text-slate-500">{session.notes}</p> : null}
        </li>
      ))}
    </ul>
  )
}
