import { Link, useParams } from 'react-router-dom'
import { ApiError } from '../../api/problem'
import { ErrorState, Spinner } from '../../components/states'
import { statusLabel, statusTone } from '../../domain/display'
import { LogSessionForm } from '../sessions/LogSessionForm'
import { SessionList } from '../sessions/SessionList'
import { PieceStats } from './PieceStats'
import { StatusControl } from './StatusControl'
import { useDeletePiece } from './useDeletePiece'
import { usePiece } from './usePiece'
import { useRecordRefresh } from './useRecordRefresh'

export function PieceDetailPage() {
  const { pieceId } = useParams<{ pieceId: string }>()
  const { data: piece, isPending, error } = usePiece(pieceId ?? '')
  const { confirmAndDelete, isPending: isDeleting } = useDeletePiece(pieceId ?? '')
  const { refresh: refreshRecord, isRefreshing: isRefreshingRecord } = useRecordRefresh(
    pieceId ?? '',
  )

  if (!pieceId) {
    return null
  }

  if (isPending) {
    return (
      <div className="flex justify-center py-12">
        <Spinner />
      </div>
    )
  }

  if (error) {
    if (error instanceof ApiError && error.status === 404) {
      return (
        <div className="flex flex-col gap-2">
          <h1 className="text-xl font-semibold text-slate-900">Piece not found</h1>
          <p className="text-slate-600">This piece may have been deleted.</p>
          <Link to="/" className="self-start text-sm font-medium text-slate-600 underline hover:text-slate-800">
            Back to pieces
          </Link>
        </div>
      )
    }
    return (
      <ErrorState
        error={error instanceof ApiError ? error : new ApiError(0, error.message)}
      />
    )
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold text-slate-900">{piece.title}</h1>
          {piece.composer ? <p className="text-slate-500">{piece.composer}</p> : null}
          <div className="mt-2 flex flex-wrap items-center gap-2 text-sm text-slate-600">
            <span className={`rounded-full px-2 py-0.5 text-xs font-medium ${statusTone(piece.status)}`}>
              {statusLabel(piece.status)}
            </span>
            <span>{piece.difficulty}</span>
            {piece.key ? <span>{piece.key}</span> : null}
            {piece.referenceUrl ? (
              <a
                href={piece.referenceUrl}
                target="_blank"
                rel="noreferrer"
                className="text-slate-600 underline hover:text-slate-800"
              >
                Reference
              </a>
            ) : null}
          </div>
        </div>
        <div className="flex shrink-0 gap-2">
          <Link
            to={`/pieces/${pieceId}/edit`}
            className="rounded-md border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Edit
          </Link>
          <button
            type="button"
            disabled={isDeleting}
            onClick={confirmAndDelete}
            className="rounded-md border border-red-300 px-3 py-1.5 text-sm font-medium text-red-700 hover:bg-red-50 disabled:opacity-50"
          >
            Delete
          </button>
        </div>
      </div>

      <div className="flex flex-col gap-2 rounded-lg border border-slate-200 bg-white p-4">
        <div className="flex items-center justify-between gap-2">
          <h2 className="text-sm font-semibold text-slate-900">Practice record</h2>
          <button
            type="button"
            onClick={() => void refreshRecord()}
            disabled={isRefreshingRecord}
            className="text-xs font-medium text-slate-500 hover:text-slate-700 disabled:opacity-50"
          >
            {isRefreshingRecord ? 'Updating…' : 'Refresh'}
          </button>
        </div>
        <PieceStats record={piece.record} />
      </div>

      <StatusControl piece={piece} />

      <div className="flex flex-col gap-2 rounded-lg border border-slate-200 bg-white p-4">
        <h2 className="text-sm font-semibold text-slate-900">Log a session</h2>
        <LogSessionForm pieceId={pieceId} pieceTitle={piece.title} />
      </div>

      <div className="flex flex-col gap-2">
        <h2 className="text-sm font-semibold text-slate-900">Session history</h2>
        <SessionList pieceId={pieceId} />
      </div>
    </div>
  )
}
