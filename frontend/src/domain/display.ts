import type { LearningStatus, PracticeFocus } from './enums'

const STATUS_LABELS: Record<LearningStatus, string> = {
  Backlog: 'Backlog',
  Learning: 'Learning',
  Polishing: 'Polishing',
  Mastered: 'Mastered',
  Shelved: 'Shelved',
}

const STATUS_TONES: Record<LearningStatus, string> = {
  Backlog: 'bg-slate-100 text-slate-700',
  Learning: 'bg-blue-100 text-blue-700',
  Polishing: 'bg-amber-100 text-amber-700',
  Mastered: 'bg-green-100 text-green-700',
  Shelved: 'bg-slate-100 text-slate-400',
}

const FOCUS_LABELS: Record<PracticeFocus, string> = {
  SightReading: 'Sight Reading',
  Technique: 'Technique',
  Memorization: 'Memorization',
  Interpretation: 'Interpretation',
  Repertoire: 'Repertoire',
}

export function statusLabel(status: LearningStatus): string {
  return STATUS_LABELS[status]
}

export function statusTone(status: LearningStatus): string {
  return STATUS_TONES[status]
}

export function focusLabel(focus: PracticeFocus): string {
  return FOCUS_LABELS[focus]
}
