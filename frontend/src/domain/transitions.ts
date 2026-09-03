import type { LearningStatus } from './enums'
import type { PracticeRecord } from './types'

export function allowedManualTargets(
  current: LearningStatus,
  record: PracticeRecord,
): LearningStatus[] {
  const targets: LearningStatus[] = []

  if (current === 'Learning') {
    targets.push('Polishing')
  }

  if (current === 'Polishing' && record.sessionCount > 0) {
    targets.push('Mastered')
  }

  if (current !== 'Shelved') {
    targets.push('Shelved')
  }

  return targets
}
