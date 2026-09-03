import { Link } from 'react-router-dom'
import type { PieceResponse } from '../../domain/types'
import { statusLabel, statusTone } from '../../domain/display'
import { PieceStats } from './PieceStats'

export interface PieceCardProps {
  piece: PieceResponse
}

export function PieceCard({ piece }: PieceCardProps) {
  return (
    <Link
      to={`/pieces/${piece.id}`}
      className="flex flex-col gap-2 rounded-lg border border-slate-200 bg-white p-4 shadow-sm transition hover:border-slate-300 hover:shadow"
    >
      <div className="flex items-start justify-between gap-2">
        <div>
          <h3 className="font-semibold text-slate-900">{piece.title}</h3>
          {piece.composer ? (
            <p className="text-sm text-slate-500">{piece.composer}</p>
          ) : null}
        </div>
        <span
          className={`shrink-0 rounded-full px-2 py-0.5 text-xs font-medium ${statusTone(piece.status)}`}
        >
          {statusLabel(piece.status)}
        </span>
      </div>
      <p className="text-sm text-slate-500">{piece.difficulty}</p>
      <PieceStats record={piece.record} />
    </Link>
  )
}
