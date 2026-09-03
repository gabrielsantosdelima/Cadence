import { useMutation, useQueryClient } from '@tanstack/react-query'
import { queryKeys } from '../../api/queryKeys'
import { changePieceStatus } from '../../api/repertoire'
import type { LearningStatus } from '../../domain/enums'

export function useChangeStatus(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (status: LearningStatus) => changePieceStatus(id, { status }),
    meta: { suppressGlobalErrorToast: true },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.pieces.detail(id) })
      queryClient.invalidateQueries({ queryKey: queryKeys.pieces.list() })
    },
  })
}
