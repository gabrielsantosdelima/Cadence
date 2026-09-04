import config from '../config'
import { practiceFocusToWireValue } from '../domain/enums'
import type { CreateSessionRequest, SessionResponse } from '../domain/types'
import { httpGet, httpPost } from './http'

export interface ListSessionsFilters {
  pieceId?: string
  from?: string
  to?: string
}

function buildSessionsQuery(filters: ListSessionsFilters): string {
  const params = new URLSearchParams()
  if (filters.pieceId) params.set('pieceId', filters.pieceId)
  if (filters.from) params.set('from', filters.from)
  if (filters.to) params.set('to', filters.to)
  const query = params.toString()
  return query ? `?${query}` : ''
}

export function listSessions(filters: ListSessionsFilters = {}): Promise<SessionResponse[]> {
  return httpGet(config.practiceUrl, `/sessions${buildSessionsQuery(filters)}`)
}

export function getSession(id: string): Promise<SessionResponse> {
  return httpGet(config.practiceUrl, `/sessions/${id}`)
}

export function createSession(request: CreateSessionRequest): Promise<SessionResponse> {
  return httpPost(config.practiceUrl, '/sessions', {
    ...request,
    focus: practiceFocusToWireValue(request.focus),
  })
}
