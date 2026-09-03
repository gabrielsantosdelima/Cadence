import { createBrowserRouter } from 'react-router-dom'
import { AppLayout } from './AppLayout'
import { NotFoundPage } from './NotFoundPage'
import { CreatePiecePage } from '../features/pieces/CreatePiecePage'
import { EditPiecePage } from '../features/pieces/EditPiecePage'
import { PieceDetailPage } from '../features/pieces/PieceDetailPage'
import { PiecesListPage } from '../features/pieces/PiecesListPage'

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
