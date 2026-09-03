import { useNavigate, useParams } from 'react-router-dom'
import { ApiError } from '../../api/problem'
import { ErrorState, Spinner } from '../../components/states'
import type { PieceResponse } from '../../domain/types'
import { PieceForm } from './PieceForm'
import { toPieceRequest, type PieceFormValues } from './pieceSchema'
import { usePiece } from './usePiece'
import { useUpdatePiece } from './useUpdatePiece'

function toFormValues(piece: PieceResponse): PieceFormValues {
  return {
    title: piece.title,
    composer: piece.composer ?? '',
    genre: piece.genre,
    difficulty: piece.difficulty,
    key: piece.key ?? '',
    referenceUrl: piece.referenceUrl ?? '',
  }
}

export function EditPiecePage() {
  const { pieceId } = useParams<{ pieceId: string }>()
  const navigate = useNavigate()
  const { data: piece, isPending, error } = usePiece(pieceId ?? '')
  const updatePiece = useUpdatePiece(pieceId ?? '')

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
          <h1 className="text-xl font-semibold text-slate-900">
            Piece not found
          </h1>
          <p className="text-slate-600">This piece may have been deleted.</p>
        </div>
      )
    }
    return (
      <ErrorState
        error={
          error instanceof ApiError ? error : new ApiError(0, error.message)
        }
      />
    )
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-semibold text-slate-900">Edit piece</h1>
      <PieceForm
        submitLabel="Save changes"
        defaultValues={toFormValues(piece)}
        onSubmit={async (values) => {
          await updatePiece.mutateAsync(toPieceRequest(values))
          navigate(`/pieces/${pieceId}`)
        }}
      />
      <button
        type="button"
        onClick={() => navigate(`/pieces/${pieceId}`)}
        className="self-start text-sm font-medium text-slate-500 hover:text-slate-700"
      >
        Cancel
      </button>
    </div>
  )
}
