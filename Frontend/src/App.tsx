import { BrowserRouter, Navigate, Routes, Route } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useEffect } from 'react'
import { Header, Sidebar } from './components'
import { useAuthStore } from './store/authStore'
import LoginPage from './pages/LoginPage'
import DashboardPage from './pages/DashboardPage'
import StudentsPage from './pages/StudentsPage'
import RoomsPage from './pages/RoomsPage'
import BuildingsPage from './pages/BuildingsPage'
import ContractsPage from './pages/ContractsPage'
import InvoicesPage from './pages/InvoicesPage'

function App() {
  console.log('App component rendering')
  const { t } = useTranslation()
  const { user } = useAuthStore()
  
  useEffect(() => {
    console.log('App mounted, user:', user)
  }, [])

  return (
    <BrowserRouter>
      {!user ? (
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="*" element={<Navigate to="/login" />} />
        </Routes>
      ) : (
        <div className="flex h-screen bg-secondary-50 dark:bg-secondary-950">
          <Sidebar />
          <div className="flex-1 md:ml-64 flex flex-col">
            <Header />
            <main className="flex-1 overflow-auto p-6">
              <Routes>
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/students" element={<StudentsPage />} />
                <Route path="/rooms" element={<RoomsPage />} />
                <Route path="/buildings" element={<BuildingsPage />} />
                <Route path="/contracts" element={<ContractsPage />} />
                <Route path="/invoices" element={<InvoicesPage />} />
                <Route path="*" element={<Navigate to="/dashboard" />} />
              </Routes>
            </main>
          </div>
        </div>
      )}
    </BrowserRouter>
  )
}

export default App
