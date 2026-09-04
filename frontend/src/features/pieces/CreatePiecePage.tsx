import { useNavigate } from 'react-router-dom'
import { PieceForm } from './PieceForm'
import { toPieceRequest } from './pieceSchema'
import { useCreatePiece } from './useCreatePiece'

export function CreatePiecePage() {
  const navigate = useNavigate()
  const createPiece = useCreatePiece()

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-xl font-semibold text-slate-900">New piece</h1>
      <PieceForm
        submitLabel="Create piece"
        onSubmit={async (values) => {
          await createPiece.mutateAsync(toPieceRequest(values))
        }}
      />
      <button
        type="button"
        onClick={() => navigate('/')}
        className="self-start text-sm font-medium text-slate-500 hover:text-slate-700"
      >
        Cancel
      </button>
    </div>
  )
}
