import type { Difficulty, Genre, LearningStatus, PracticeFocus } from './enums'

export interface PracticeRecord {
  totalMinutes: number
  sessionCount: number
  lastPracticedAtUtc: string | null
  averageQuality: number | null
}

export interface PieceResponse {
  id: string
  title: string
  composer: string | null
  genre: Genre
  difficulty: Difficulty
  key: string | null
  referenceUrl: string | null
  status: LearningStatus
  record: PracticeRecord
  createdAtUtc: string
  updatedAtUtc: string
}

export interface SessionResponse {
  id: string
  pieceId: string
  pieceTitle: string
  startedAtUtc: string
  durationMinutes: number
  tempoBpm: number | null
  focus: PracticeFocus
  quality: number
  notes: string | null
  createdAtUtc: string
}

export interface CreatePieceRequest {
  title: string
  composer?: string | null
  genre: Genre
  difficulty: Difficulty
  key?: string | null
  referenceUrl?: string | null
}

export interface UpdatePieceRequest {
  title: string
  composer?: string | null
  genre: Genre
  difficulty: Difficulty
  key?: string | null
  referenceUrl?: string | null
}

export interface ChangeStatusRequest {
  status: LearningStatus
}

export interface CreateSessionRequest {
  pieceId: string
  pieceTitle: string
  startedAtUtc: string
  durationMinutes: number
  tempoBpm?: number | null
  focus: PracticeFocus
  quality: number
  notes?: string | null
}
