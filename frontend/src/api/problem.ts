export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
}

export class ApiError extends Error {
  readonly status: number
  readonly title: string
  readonly detail: string | undefined

  constructor(status: number, title: string, detail: string | undefined = undefined) {
    super(detail ?? title)
    this.name = 'ApiError'
    this.status = status
    this.title = title
    this.detail = detail
  }
}

export async function parseProblem(response: Response): Promise<ApiError> {
  const contentType = response.headers.get('content-type') ?? ''

  if (contentType.includes('application/problem+json')) {
    const body = (await response.json()) as ProblemDetails
    return new ApiError(response.status, body.title ?? response.statusText, body.detail)
  }

  const text = await response.text().catch(() => '')
  return new ApiError(response.status, response.statusText, text || undefined)
}
