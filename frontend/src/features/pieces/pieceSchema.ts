import { z } from 'zod'
import {
  DIFFICULTIES,
  GENRES,
  type Difficulty,
  type Genre,
} from '../../domain/enums'
import type { CreatePieceRequest } from '../../domain/types'

export const pieceSchema = z.object({
  title: z
    .string()
    .trim()
    .min(1, 'Title is required')
    .max(120, 'Title must be 120 characters or fewer'),
  composer: z
    .string()
    .trim()
    .max(120, 'Composer must be 120 characters or fewer'),
  genre: z.enum(GENRES as [Genre, ...Genre[]]),
  difficulty: z.enum(DIFFICULTIES as [Difficulty, ...Difficulty[]]),
  key: z.string().trim(),
  referenceUrl: z.union([
    z.literal(''),
    z.url('Reference URL must be an absolute URL'),
  ]),
})

export type PieceFormValues = z.infer<typeof pieceSchema>

export function toPieceRequest(values: PieceFormValues): CreatePieceRequest {
  return {
    title: values.title,
    composer: values.composer === '' ? null : values.composer,
    genre: values.genre,
    difficulty: values.difficulty,
    key: values.key === '' ? null : values.key,
    referenceUrl: values.referenceUrl === '' ? null : values.referenceUrl,
  }
}
