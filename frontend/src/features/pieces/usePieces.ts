import { useQuery } from '@tanstack/react-query'
import { listPieces, type ListPiecesFilters } from '../../api/repertoire'
import { queryKeys } from '../../api/queryKeys'

export function usePieces(filters: ListPiecesFilters = {}) {
  return useQuery({
    queryKey: queryKeys.pieces.list(filters),
    queryFn: () => listPieces(filters),
  })
}
