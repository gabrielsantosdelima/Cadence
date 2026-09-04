import type { ListPiecesFilters } from './repertoire'
import type { ListSessionsFilters } from './practice'

export const queryKeys = {
  pieces: {
    list: (filters: ListPiecesFilters = {}) => ['pieces', 'list', filters] as const,
    detail: (id: string) => ['pieces', 'detail', id] as const,
  },
  sessions: {
    list: (filters: ListSessionsFilters = {}) => ['sessions', 'list', filters] as const,
  },
} as const
