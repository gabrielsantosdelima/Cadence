import { useMutation, useQueryClient } from '@tanstack/react-query'
import { createSession } from '../../api/practice'
import { queryKeys } from '../../api/queryKeys'
import type { CreateSessionRequest } from '../../domain/types'
import { useRecordRefresh } from '../pieces/useRecordRefresh'

export function useCreateSession(pieceId: string) {
  const queryClient = useQueryClient()
  const { refresh, isRefreshing } = useRecordRefresh(pieceId)

  const mutation = useMutation({
    mutationFn: (request: CreateSessionRequest) => createSession(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.sessions.list({ pieceId }) })
      void refresh()
    },
  })

  return { ...mutation, isRefreshingRecord: isRefreshing }
}
