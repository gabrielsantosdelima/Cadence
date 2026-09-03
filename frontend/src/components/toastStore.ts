import { useEffect, useState } from 'react'
import { ApiError } from '../api/problem'

export interface ToastMessage {
  id: number
  title: string
  detail: string | undefined
}

type Listener = (toasts: ToastMessage[]) => void

let toasts: ToastMessage[] = []
let nextId = 0
const listeners = new Set<Listener>()

function notify(): void {
  for (const listener of listeners) listener(toasts)
}

export function dismissToast(id: number): void {
  toasts = toasts.filter((toast) => toast.id !== id)
  notify()
}

export function pushToast(title: string, detail?: string): void {
  const toast: ToastMessage = { id: nextId++, title, detail }
  toasts = [...toasts, toast]
  notify()
  window.setTimeout(() => dismissToast(toast.id), 5000)
}

export function pushApiErrorToast(error: ApiError): void {
  pushToast(error.title, error.detail)
}

export function useToast(): { toasts: ToastMessage[]; dismissToast: (id: number) => void } {
  const [current, setCurrent] = useState(toasts)

  useEffect(() => {
    listeners.add(setCurrent)
    return () => {
      listeners.delete(setCurrent)
    }
  }, [])

  return { toasts: current, dismissToast }
}
