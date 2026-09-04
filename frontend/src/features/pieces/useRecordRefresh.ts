import { useCallback, useRef, useState } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { queryKeys } from '../../api/queryKeys'
import type { PieceResponse } from '../../domain/types'

const POLL_DELAYS_MS = [800, 1600, 3000]

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => window.setTimeout(resolve, ms))
}

export function useRecordRefresh(pieceId: string) {
  const queryClient = useQueryClient()
  const [isRefreshing, setIsRefreshing] = useState(false)
  const runIdRef = useRef(0)

  const refresh = useCallback(async () => {
    const runId = ++runIdRef.current
    setIsRefreshing(true)

    const detailKey = queryKeys.pieces.detail(pieceId)
    const baselineCount = queryClient.getQueryData<PieceResponse>(detailKey)?.record.sessionCount

    for (const delayMs of POLL_DELAYS_MS) {
      await delay(delayMs)
      if (runIdRef.current !== runId) return

      await queryClient.refetchQueries({ queryKey: detailKey })
      const updated = queryClient.getQueryData<PieceResponse>(detailKey)

      if (baselineCount == null || (updated && updated.record.sessionCount > baselineCount)) {
        break
      }
    }

    if (runIdRef.current === runId) {
      setIsRefreshing(false)
    }
  }, [pieceId, queryClient])

  return { refresh, isRefreshing }
}
