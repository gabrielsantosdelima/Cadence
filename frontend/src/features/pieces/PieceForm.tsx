import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { ApiError } from '../../api/problem'
import { DIFFICULTIES, GENRES } from '../../domain/enums'
import { pieceSchema, type PieceFormValues } from './pieceSchema'

export interface PieceFormProps {
  defaultValues?: PieceFormValues
  onSubmit: (values: PieceFormValues) => Promise<void>
  submitLabel: string
}

const EMPTY_VALUES: PieceFormValues = {
  title: '',
  composer: '',
  genre: GENRES[0]!,
  difficulty: DIFFICULTIES[0]!,
  key: '',
  referenceUrl: '',
}

const FIELD_CLASS =
  'rounded-md border border-slate-300 px-2 py-1.5 text-sm text-slate-900'
const LABEL_CLASS = 'flex flex-col gap-1 text-sm text-slate-600'
const FIELD_ERROR_CLASS = 'text-sm text-red-600'

export function PieceForm({
  defaultValues,
  onSubmit,
  submitLabel,
}: PieceFormProps) {
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<PieceFormValues>({
    resolver: zodResolver(pieceSchema),
    defaultValues: defaultValues ?? EMPTY_VALUES,
  })

  async function submit(values: PieceFormValues): Promise<void> {
    try {
      await onSubmit(values)
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
        Title
        <input type="text" className={FIELD_CLASS} {...register('title')} />
        {errors.title ? (
          <span className={FIELD_ERROR_CLASS}>{errors.title.message}</span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Composer
        <input type="text" className={FIELD_CLASS} {...register('composer')} />
        {errors.composer ? (
          <span className={FIELD_ERROR_CLASS}>{errors.composer.message}</span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Genre
        <select className={FIELD_CLASS} {...register('genre')}>
          {GENRES.map((genre) => (
            <option key={genre} value={genre}>
              {genre}
            </option>
          ))}
        </select>
        {errors.genre ? (
          <span className={FIELD_ERROR_CLASS}>{errors.genre.message}</span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Difficulty
        <select className={FIELD_CLASS} {...register('difficulty')}>
          {DIFFICULTIES.map((difficulty) => (
            <option key={difficulty} value={difficulty}>
              {difficulty}
            </option>
          ))}
        </select>
        {errors.difficulty ? (
          <span className={FIELD_ERROR_CLASS}>{errors.difficulty.message}</span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Key
        <input
          type="text"
          placeholder="e.g. C major"
          className={FIELD_CLASS}
          {...register('key')}
        />
        {errors.key ? (
          <span className={FIELD_ERROR_CLASS}>{errors.key.message}</span>
        ) : null}
      </label>

      <label className={LABEL_CLASS}>
        Reference URL
        <input
          type="text"
          placeholder="https://..."
          className={FIELD_CLASS}
          {...register('referenceUrl')}
        />
        {errors.referenceUrl ? (
          <span className={FIELD_ERROR_CLASS}>
            {errors.referenceUrl.message}
          </span>
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
        {submitLabel}
      </button>
    </form>
  )
}
