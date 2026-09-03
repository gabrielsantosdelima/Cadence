import { z } from 'zod'
import {
  PRACTICE_FOCUSES,
  type PracticeFocus,
} from '../../domain/enums'
import type { CreateSessionRequest } from '../../domain/types'

function requiredIntString(min: number, max: number, label: string) {
  return z
    .string()
    .trim()
    .min(1, `${label} is required`)
    .refine((value) => /^-?\d+$/.test(value), `${label} must be a whole number`)
    .refine((value) => Number(value) >= min, `${label} must be at least ${min}`)
    .refine((value) => Number(value) <= max, `${label} must be at most ${max}`)
}

function optionalIntString(min: number, max: number, label: string) {
  return z.union([
    z.literal(''),
    z
      .string()
      .trim()
      .refine((value) => /^-?\d+$/.test(value), `${label} must be a whole number`)
      .refine((value) => Number(value) >= min, `${label} must be at least ${min}`)
      .refine((value) => Number(value) <= max, `${label} must be at most ${max}`),
  ])
}

export const sessionSchema = z.object({
  startedAtUtc: z
    .string()
    .min(1, 'Start time is required')
    .refine((value) => new Date(value).getTime() <= Date.now(), {
      message: 'Start time cannot be in the future',
    }),
  durationMinutes: requiredIntString(1, 600, 'Duration'),
  tempo: optionalIntString(20, 300, 'Tempo'),
  focus: z.enum(PRACTICE_FOCUSES as [PracticeFocus, ...PracticeFocus[]]),
  qualityRating: requiredIntString(1, 5, 'Quality'),
  notes: z.string().trim().max(500, 'Notes must be 500 characters or fewer'),
})

export type SessionFormValues = z.infer<typeof sessionSchema>

export function toSessionRequest(
  pieceId: string,
  pieceTitle: string,
  values: SessionFormValues,
): CreateSessionRequest {
  return {
    pieceId,
    pieceTitle,
    startedAtUtc: new Date(values.startedAtUtc).toISOString(),
    durationMinutes: Number(values.durationMinutes),
    tempoBpm: values.tempo === '' ? null : Number(values.tempo),
    focus: values.focus,
    quality: Number(values.qualityRating),
    notes: values.notes === '' ? null : values.notes,
  }
}
