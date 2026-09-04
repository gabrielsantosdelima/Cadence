import { useQuery } from '@tanstack/react-query'
import { listSessions, type ListSessionsFilters } from '../../api/practice'
import { queryKeys } from '../../api/queryKeys'

export function useSessions(filters: ListSessionsFilters) {
  return useQuery({
    queryKey: queryKeys.sessions.list(filters),
    queryFn: () => listSessions(filters),
  })
}
