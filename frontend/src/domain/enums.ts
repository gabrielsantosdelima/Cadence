export type LearningStatus = 'Backlog' | 'Learning' | 'Polishing' | 'Mastered' | 'Shelved'

export const LEARNING_STATUSES: readonly LearningStatus[] = [
  'Backlog',
  'Learning',
  'Polishing',
  'Mastered',
  'Shelved',
]

export type Genre =
  | 'Baroque'
  | 'Classical'
  | 'Romantic'
  | 'Modern'
  | 'Contemporary'
  | 'Jazz'
  | 'Folk'
  | 'Other'

export const GENRES: readonly Genre[] = [
  'Baroque',
  'Classical',
  'Romantic',
  'Modern',
  'Contemporary',
  'Jazz',
  'Folk',
  'Other',
]

export type Difficulty = 'Easy' | 'Medium' | 'Hard'

export const DIFFICULTIES: readonly Difficulty[] = ['Easy', 'Medium', 'Hard']

export type PracticeFocus =
  | 'SightReading'
  | 'Technique'
  | 'Memorization'
  | 'Interpretation'
  | 'Repertoire'

export const PRACTICE_FOCUSES: readonly PracticeFocus[] = [
  'SightReading',
  'Technique',
  'Memorization',
  'Interpretation',
  'Repertoire',
]

const GENRE_WIRE_VALUES: Record<Genre, number> = {
  Baroque: 0,
  Classical: 1,
  Romantic: 2,
  Modern: 3,
  Contemporary: 4,
  Jazz: 5,
  Folk: 6,
  Other: 7,
}

const DIFFICULTY_WIRE_VALUES: Record<Difficulty, number> = {
  Easy: 0,
  Medium: 1,
  Hard: 2,
}

const PRACTICE_FOCUS_WIRE_VALUES: Record<PracticeFocus, number> = {
  SightReading: 0,
  Technique: 1,
  Memorization: 2,
  Interpretation: 3,
  Repertoire: 4,
}

export function genreToWireValue(genre: Genre): number {
  return GENRE_WIRE_VALUES[genre]
}

export function difficultyToWireValue(difficulty: Difficulty): number {
  return DIFFICULTY_WIRE_VALUES[difficulty]
}

export function practiceFocusToWireValue(focus: PracticeFocus): number {
  return PRACTICE_FOCUS_WIRE_VALUES[focus]
}
