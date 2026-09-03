import { useToast } from './toastStore'

export function ToastHost() {
  const { toasts: current, dismissToast: dismiss } = useToast()

  if (current.length === 0) return null

  return (
    <div className="fixed bottom-4 right-4 z-50 flex flex-col gap-2">
      {current.map((toast) => (
        <div key={toast.id} role="alert" className="w-80 rounded-lg border border-red-300 bg-red-50 p-3 shadow-lg">
          <button
            type="button"
            onClick={() => dismiss(toast.id)}
            className="float-right text-red-500 hover:text-red-700"
            aria-label="Dismiss"
          >
            &times;
          </button>
          <p className="font-semibold text-red-800">{toast.title}</p>
          {toast.detail ? <p className="text-sm text-red-700">{toast.detail}</p> : null}
        </div>
      ))}
    </div>
  )
}
