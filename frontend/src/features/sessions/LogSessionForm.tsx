import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { ApiError } from '../../api/problem'
import { focusLabel } from '../../domain/display'
import { PRACTICE_FOCUSES } from '../../domain/enums'
import {
  sessionSchema,
  toSessionRequest,
  type SessionFormValues,
} from './sessionSchema'
import { useCreateSession } from './useCreateSession'

export interface LogSessionFormProps {
  pieceId: string
  pieceTitle: string
}

function nowLocalDateTime(): string {
  const now = new Date()
  const offsetMs = now.getTimezoneOffset() * 60_000
  return new Date(now.getTime() - offsetMs).toISOString().slice(0, 16)
}

function defaultValues(): SessionFormValues {
  return {
    startedAtUtc: nowLocalDateTime(),
    durationMinutes: '30',
    tempo: '',
    focus: PRACTICE_FOCUSES[0]!,
    qualityRating: '3',
    notes: '',
  }
}

const FIELD_CLASS =
  'rounded-md border border-slate-300 px-2 py-1.5 text-sm text-slate-900'
const LABEL_CLASS = 'flex flex-col gap-1 text-sm text-slate-600'
const FIELD_ERROR_CLASS = 'text-sm text-red-600'

export function LogSessionForm({ pieceId, pieceTitle }: LogSessionFormProps) {
  const createSession = useCreateSession(pieceId)
  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<SessionFormValues>({
    resolver: zodResolver(sessionSchema),
    defaultValues: defaultValues(),
  })

  async function submit(values: SessionFormValues): Promise<void> {
    try {
      await createSession.mutateAsync(
        toSessionRequest(pieceId, pieceTitle, values),
      )
      reset(defaultValues())
    } catch (error) {
      if (error instanceof ApiError) {
        setError('root', { message: error.detail ?? error.title })
        return
      }
      throw error
    }
  }

  return (
    <form
      onSubmit={handleSubmit(submit)}
      className="flex max-w-md flex-col gap-4"
    >
      <label className={LABEL_CLASS}>
        Started at
        <input
          type="datetime-local"
          className={FIELD_CLASS}
          {...register('startedAtUtc')}
        />
        {errors.startedAtUtc ? (
          <span className={FIELD_ERROR_CLASS}>{errors.startedAtUtc.message}</span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Duration (minutes)
        <input
          type="number"
          className={FIELD_CLASS}
          {...register('durationMinutes')}
        />
        {errors.durationMinutes ? (
          <span className={FIELD_ERROR_CLASS}>
            {errors.durationMinutes.message}
          </span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Tempo (BPM)
        <input
          type="number"
          placeholder="optional"
          className={FIELD_CLASS}
          {...register('tempo')}
        />
        {errors.tempo ? (
          <span className={FIELD_ERROR_CLASS}>{errors.tempo.message}</span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Focus
        <select className={FIELD_CLASS} {...register('focus')}>
          {PRACTICE_FOCUSES.map((focus) => (
            <option key={focus} value={focus}>
              {focusLabel(focus)}
            </option>
          ))}
        </select>
        {errors.focus ? (
          <span className={FIELD_ERROR_CLASS}>{errors.focus.message}</span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Quality (1-5)
        <input
          type="number"
          min={1}
          max={5}
          className={FIELD_CLASS}
          {...register('qualityRating')}
        />
        {errors.qualityRating ? (
          <span className={FIELD_ERROR_CLASS}>
            {errors.qualityRating.message}
          </span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Notes
        <textarea className={FIELD_CLASS} {...register('notes')} />
        {errors.notes ? (
          <span className={FIELD_ERROR_CLASS}>{errors.notes.message}</span>
        ) : null}
      </label>

      {errors.root ? (
        <p className={FIELD_ERROR_CLASS}>{errors.root.message}</p>
      ) : null}

      <button
        type="submit"
        disabled={isSubmitting}
        className="self-start rounded-md bg-slate-900 px-3 py-1.5 text-sm font-medium text-white hover:bg-slate-700 disabled:opacity-50"
      >
        Log session
      </button>
    </form>
  )
}
