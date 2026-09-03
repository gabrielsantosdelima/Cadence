import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { queryKeys } from '../../api/queryKeys'
import { createPiece } from '../../api/repertoire'
import type { CreatePieceRequest } from '../../domain/types'

export function useCreatePiece() {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (request: CreatePieceRequest) => createPiece(request),
    onSuccess: (piece) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.pieces.list() })
      navigate(`/pieces/${piece.id}`)
    },
  })
}
