import { useQuery } from '@tanstack/react-query'
import { queryKeys } from '../../api/queryKeys'
import { getPiece } from '../../api/repertoire'

export function usePiece(id: string) {
  return useQuery({
    queryKey: queryKeys.pieces.detail(id),
    queryFn: () => getPiece(id),
  })
}
