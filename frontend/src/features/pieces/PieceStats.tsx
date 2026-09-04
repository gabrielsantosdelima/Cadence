import type { PracticeRecord } from '../../domain/types'

export interface PieceStatsProps {
  record: PracticeRecord
}

function formatLastPracticed(lastPracticedAtUtc: string): string {
  const lastPracticedDate = new Date(lastPracticedAtUtc)
  const daysAgo = Math.floor(
    (Date.now() - lastPracticedDate.getTime()) / (1000 * 60 * 60 * 24),
  )

  if (daysAgo <= 0) return 'today'
  if (daysAgo === 1) return 'yesterday'
  if (daysAgo < 30) return `${daysAgo}d ago`

  return lastPracticedDate.toLocaleDateString(undefined, {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  })
}

export function PieceStats({ record }: PieceStatsProps) {
  if (record.sessionCount === 0 || record.averageQuality == null) {
    return <p className="text-sm text-slate-400">Not practiced yet</p>
  }

  return (
    <div className="flex flex-wrap gap-x-4 gap-y-1 text-sm text-slate-600">
      <span>{record.totalMinutes} min</span>
      <span>
        {record.sessionCount} session{record.sessionCount === 1 ? '' : 's'}
      </span>
      <span>avg quality {record.averageQuality.toFixed(1)}</span>
      {record.lastPracticedAtUtc ? (
        <span>last {formatLastPracticed(record.lastPracticedAtUtc)}</span>
      ) : null}
    </div>
  )
}
