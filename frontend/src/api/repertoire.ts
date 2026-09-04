import config from '../config'
import { difficultyToWireValue, genreToWireValue, type Genre, type LearningStatus } from '../domain/enums'
import type {
  ChangeStatusRequest,
  CreatePieceRequest,
  PieceResponse,
  UpdatePieceRequest,
} from '../domain/types'
import { httpDelete, httpGet, httpPatch, httpPost, httpPut } from './http'

export interface ListPiecesFilters {
  status?: LearningStatus
  genre?: Genre
}

function buildPiecesQuery(filters: ListPiecesFilters): string {
  const params = new URLSearchParams()
  if (filters.status) params.set('status', filters.status)
  if (filters.genre) params.set('genre', filters.genre)
  const query = params.toString()
  return query ? `?${query}` : ''
}

export function listPieces(filters: ListPiecesFilters = {}): Promise<PieceResponse[]> {
  return httpGet(config.repertoireUrl, `/pieces${buildPiecesQuery(filters)}`)
}

export function getPiece(id: string): Promise<PieceResponse> {
  return httpGet(config.repertoireUrl, `/pieces/${id}`)
}

export function createPiece(request: CreatePieceRequest): Promise<PieceResponse> {
  return httpPost(config.repertoireUrl, '/pieces', {
    ...request,
    genre: genreToWireValue(request.genre),
    difficulty: difficultyToWireValue(request.difficulty),
  })
}

export function updatePiece(id: string, request: UpdatePieceRequest): Promise<PieceResponse> {
  return httpPut(config.repertoireUrl, `/pieces/${id}`, {
    ...request,
    genre: genreToWireValue(request.genre),
    difficulty: difficultyToWireValue(request.difficulty),
  })
}

export function changePieceStatus(id: string, request: ChangeStatusRequest): Promise<PieceResponse> {
  return httpPatch(config.repertoireUrl, `/pieces/${id}/status`, request)
}

export function deletePiece(id: string): Promise<void> {
  return httpDelete(config.repertoireUrl, `/pieces/${id}`)
}
