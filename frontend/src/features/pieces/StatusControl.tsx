import { ApiError } from '../../api/problem'
import { statusLabel } from '../../domain/display'
import { allowedManualTargets } from '../../domain/transitions'
import type { PieceResponse } from '../../domain/types'
import { useChangeStatus } from './useChangeStatus'

export interface StatusControlProps {
  piece: PieceResponse
}

export function StatusControl({ piece }: StatusControlProps) {
  const changeStatus = useChangeStatus(piece.id)
  const targets = allowedManualTargets(piece.status, piece.record)

  if (targets.length === 0) {
    return null
  }

  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-wrap gap-2">
        {targets.map((target) => (
          <button
            key={target}
            type="button"
            disabled={changeStatus.isPending}
            onClick={() => changeStatus.mutate(target)}
            className="rounded-md border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
          >
            Move to {statusLabel(target)}
          </button>
        ))}
      </div>
      {changeStatus.error ? (
        <p role="alert" className="text-sm text-red-600">
          {changeStatus.error instanceof ApiError
            ? (changeStatus.error.detail ?? changeStatus.error.title)
            : changeStatus.error.message}
        </p>
      ) : null}
    </div>
  )
}
