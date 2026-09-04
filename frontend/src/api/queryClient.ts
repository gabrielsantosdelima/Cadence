import { MutationCache, QueryClient } from '@tanstack/react-query'
import { pushApiErrorToast, pushToast } from '../components/toastStore'
import { ApiError } from './problem'

declare module '@tanstack/react-query' {
  interface Register {
    mutationMeta: {
      suppressGlobalErrorToast?: boolean
    }
  }
}

function isNonRetryableStatus(status: number): boolean {
  return status >= 400 && status < 500
}

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: (failureCount, error) => {
        if (error instanceof ApiError && isNonRetryableStatus(error.status)) return false
        return failureCount < 2
      },
    },
    mutations: {
      retry: false,
    },
  },
  mutationCache: new MutationCache({
    onError: (error, _variables, _context, mutation) => {
      if (mutation.meta?.suppressGlobalErrorToast) return
      if (error instanceof ApiError) {
        pushApiErrorToast(error)
        return
      }
      pushToast('Unexpected error', error.message)
    },
  }),
})
