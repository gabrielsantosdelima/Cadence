import {
  GENRES,
  LEARNING_STATUSES,
  type Genre,
  type LearningStatus,
} from '../../domain/enums'
import { statusLabel } from '../../domain/display'
import type { ListPiecesFilters } from '../../api/repertoire'

export interface PieceFiltersProps {
  filters: ListPiecesFilters
  onChange: (filters: ListPiecesFilters) => void
}

const ALL_VALUE = ''

function withoutKey<Filters extends object, Key extends keyof Filters>(
  filters: Filters,
  key: Key,
): Omit<Filters, Key> {
  const remainingEntries = Object.entries(filters).filter(
    ([entryKey]) => entryKey !== key,
  )
  return Object.fromEntries(remainingEntries) as Omit<Filters, Key>
}

export function PieceFilters({ filters, onChange }: PieceFiltersProps) {
  const hasActiveFilter = Boolean(filters.status || filters.genre)

  return (
    <div className="flex flex-wrap items-center gap-3">
      <label className="flex items-center gap-2 text-sm text-slate-600">
        Status
        <select
          className="rounded-md border border-slate-300 px-2 py-1 text-sm"
          value={filters.status ?? ALL_VALUE}
          onChange={(event) => {
            const value = event.target.value as
              LearningStatus | typeof ALL_VALUE
            const rest = withoutKey(filters, 'status')
            onChange(value === ALL_VALUE ? rest : { ...rest, status: value })
          }}
        >
          <option value={ALL_VALUE}>All</option>
          {LEARNING_STATUSES.map((status) => (
            <option key={status} value={status}>
              {statusLabel(status)}
            </option>
          ))}
        </select>
      </label>

      <label className="flex items-center gap-2 text-sm text-slate-600">
        Genre
        <select
          className="rounded-md border border-slate-300 px-2 py-1 text-sm"
          value={filters.genre ?? ALL_VALUE}
          onChange={(event) => {
            const value = event.target.value as Genre | typeof ALL_VALUE
            const rest = withoutKey(filters, 'genre')
            onChange(value === ALL_VALUE ? rest : { ...rest, genre: value })
          }}
        >
          <option value={ALL_VALUE}>All</option>
          {GENRES.map((genre) => (
            <option key={genre} value={genre}>
              {genre}
            </option>
          ))}
        </select>
      </label>

      {hasActiveFilter ? (
        <button
          type="button"
          onClick={() => onChange({})}
          className="text-sm font-medium text-slate-500 hover:text-slate-700"
        >
          Clear filters
        </button>
      ) : null}
    </div>
  )
}
