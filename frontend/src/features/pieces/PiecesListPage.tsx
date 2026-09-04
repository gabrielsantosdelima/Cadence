import { useState } from 'react'
import { Link } from 'react-router-dom'
import type { ListPiecesFilters } from '../../api/repertoire'
import { Spinner, EmptyState, ErrorState } from '../../components/states'
import { ApiError } from '../../api/problem'
import { usePieces } from './usePieces'
import { PieceFilters } from './PieceFilters'
import { PieceCard } from './PieceCard'

export function PiecesListPage() {
  const [filters, setFilters] = useState<ListPiecesFilters>({})
  const hasActiveFilter = Boolean(filters.status || filters.genre)
  const { data: pieces, isPending, error } = usePieces(filters)

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4">
        <h1 className="text-xl font-semibold text-slate-900">Pieces</h1>
        <Link
          to="/pieces/new"
          className="rounded-md bg-slate-900 px-3 py-1.5 text-sm font-medium text-white hover:bg-slate-700"
        >
          New piece
        </Link>
      </div>

      <PieceFilters filters={filters} onChange={setFilters} />

      {isPending ? (
        <div className="flex justify-center py-12">
          <Spinner />
        </div>
      ) : error ? (
        <ErrorState
          error={
            error instanceof ApiError ? error : new ApiError(0, error.message)
          }
        />
      ) : pieces.length === 0 ? (
        hasActiveFilter ? (
          <EmptyState
            message="No pieces match these filters."
            action={
              <button
                type="button"
                onClick={() => setFilters({})}
                className="text-sm font-medium text-slate-600 underline hover:text-slate-800"
              >
                Clear filters
              </button>
            }
          />
        ) : (
          <EmptyState
            message="Your catalogue is empty. Add a piece to start tracking your practice."
            action={
              <Link
                to="/pieces/new"
                className="rounded-md bg-slate-900 px-3 py-1.5 text-sm font-medium text-white hover:bg-slate-700"
              >
                Add your first piece
              </Link>
            }
          />
        )
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {pieces.map((piece) => (
            <PieceCard key={piece.id} piece={piece} />
          ))}
        </div>
      )}
    </div>
  )
}
