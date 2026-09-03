import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { queryKeys } from '../../api/queryKeys'
import { deletePiece } from '../../api/repertoire'

const CONFIRM_MESSAGE =
  'Delete this piece? Its past practice sessions stay in your Practice history and are not removed.'

export function useDeletePiece(id: string) {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  const mutation = useMutation({
    mutationFn: () => deletePiece(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.pieces.list() })
      navigate('/')
    },
  })

  function confirmAndDelete(): void {
    if (window.confirm(CONFIRM_MESSAGE)) {
      mutation.mutate()
    }
  }

  return { confirmAndDelete, isPending: mutation.isPending }
}
