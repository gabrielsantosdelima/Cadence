import type { ReactNode } from 'react'
import { ApiError } from '../api/problem'

export function Spinner() {
  return (
    <div
      role="status"
      aria-label="Loading"
      className="h-6 w-6 animate-spin rounded-full border-2 border-slate-300 border-t-slate-600"
    />
  )
}

export interface EmptyStateProps {
  message: string
  action?: ReactNode
}

export function EmptyState({ message, action }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center gap-3 rounded-lg border border-dashed border-slate-300 p-8 text-center">
      <p className="text-slate-500">{message}</p>
      {action}
    </div>
  )
}

export interface ErrorStateProps {
  error: ApiError
}

export function ErrorState({ error }: ErrorStateProps) {
  return (
    <div role="alert" className="rounded-lg border border-red-300 bg-red-50 p-4">
      <p className="font-semibold text-red-800">{error.title}</p>
      {error.detail ? <p className="mt-1 text-sm text-red-700">{error.detail}</p> : null}
    </div>
  )
}
