import { Link, Outlet } from 'react-router-dom'
import { ToastHost } from '../components/Toast'

export function AppLayout() {
  return (
    <div className="min-h-screen bg-slate-50">
      <header className="border-b border-slate-200 bg-white px-6 py-4">
        <Link to="/" className="text-lg font-semibold text-slate-900">
          Cadence
        </Link>
      </header>
      <main className="mx-auto max-w-5xl px-6 py-8">
        <Outlet />
      </main>
      <ToastHost />
    </div>
  )
}
