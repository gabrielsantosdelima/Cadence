import { useMutation, useQueryClient } from '@tanstack/react-query'
import { queryKeys } from '../../api/queryKeys'
import { updatePiece } from '../../api/repertoire'
import type { UpdatePieceRequest } from '../../domain/types'

export function useUpdatePiece(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: UpdatePieceRequest) => updatePiece(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.pieces.detail(id) })
      queryClient.invalidateQueries({ queryKey: queryKeys.pieces.list() })
    },
  })
}
