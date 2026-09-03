import { createBrowserRouter } from 'react-router-dom'
import { AppLayout } from './AppLayout'
import { CreatePiecePage } from '../features/pieces/CreatePiecePage'
import { EditPiecePage } from '../features/pieces/EditPiecePage'
import { PieceDetailPage } from '../features/pieces/PieceDetailPage'
import { PiecesListPage } from '../features/pieces/PiecesListPage'

function NotFoundPage() {
  return (
    <div>
      <h1 className="text-xl font-semibold text-slate-900">Page not found</h1>
      <p className="mt-2 text-slate-600">
        The page you're looking for doesn't exist.
      </p>
    </div>
  )
}

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      { index: true, element: <PiecesListPage /> },
      { path: 'pieces/new', element: <CreatePiecePage /> },
      {
        path: 'pieces/:pieceId',
        element: <PieceDetailPage />,
      },
      {
        path: 'pieces/:pieceId/edit',
        element: <EditPiecePage />,
      },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
])
