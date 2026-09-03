import { parseProblem } from './problem'

async function request<TResponse>(
  method: string,
  baseUrl: string,
  path: string,
  body?: unknown,
): Promise<TResponse> {
  const init: RequestInit = { method }
  if (body !== undefined) {
    init.headers = { 'Content-Type': 'application/json' }
    init.body = JSON.stringify(body)
  }
  const response = await fetch(`${baseUrl}${path}`, init)

  if (!response.ok) {
    throw await parseProblem(response)
  }

  if (response.status === 204) {
    return undefined as TResponse
  }

  return (await response.json()) as TResponse
}

export function httpGet<TResponse>(baseUrl: string, path: string): Promise<TResponse> {
  return request<TResponse>('GET', baseUrl, path)
}

export function httpPost<TResponse>(baseUrl: string, path: string, body?: unknown): Promise<TResponse> {
  return request<TResponse>('POST', baseUrl, path, body)
}

export function httpPut<TResponse>(baseUrl: string, path: string, body?: unknown): Promise<TResponse> {
  return request<TResponse>('PUT', baseUrl, path, body)
}

export function httpPatch<TResponse>(baseUrl: string, path: string, body?: unknown): Promise<TResponse> {
  return request<TResponse>('PATCH', baseUrl, path, body)
}

export function httpDelete<TResponse>(baseUrl: string, path: string): Promise<TResponse> {
  return request<TResponse>('DELETE', baseUrl, path)
}
