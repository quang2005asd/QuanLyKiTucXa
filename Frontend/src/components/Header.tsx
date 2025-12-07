import { LogOut, Menu, Settings, User, X } from 'lucide-react'
import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'

export const Header = () => {
  const { t } = useTranslation()
  const { user, logout } = useAuthStore()
  const [showMenu, setShowMenu] = useState(false)

  return (
    <header className="bg-white dark:bg-secondary-800 border-b border-secondary-200 dark:border-secondary-700 sticky top-0 z-40">
      <div className="flex items-center justify-between px-6 py-4">
        <div className="flex items-center gap-3">
          <h1 className="text-2xl font-bold bg-gradient-to-r from-primary-500 to-primary-700 bg-clip-text text-transparent">
            KTXA
          </h1>
        </div>

        <div className="flex items-center gap-4">
          {/* User Menu */}
          <div className="relative">
            <button
              onClick={() => setShowMenu(!showMenu)}
              className="flex items-center gap-2 px-3 py-2 rounded-lg hover:bg-secondary-100 dark:hover:bg-secondary-700"
            >
              <User className="w-5 h-5" />
              <span className="text-sm font-medium">{user?.username}</span>
            </button>

            {showMenu && (
              <div className="absolute right-0 mt-2 w-48 bg-white dark:bg-secondary-800 rounded-lg shadow-lg border border-secondary-200 dark:border-secondary-700">
                <Link
                  to="/profile"
                  className="flex items-center gap-2 px-4 py-2 text-sm hover:bg-secondary-100 dark:hover:bg-secondary-700"
                >
                  <User className="w-4 h-4" />
                  {t('navigation.profile')}
                </Link>
                <Link
                  to="/settings"
                  className="flex items-center gap-2 px-4 py-2 text-sm hover:bg-secondary-100 dark:hover:bg-secondary-700"
                >
                  <Settings className="w-4 h-4" />
                  {t('navigation.settings')}
                </Link>
                <button
                  onClick={() => {
                    logout()
                    setShowMenu(false)
                  }}
                  className="w-full flex items-center gap-2 px-4 py-2 text-sm text-danger hover:bg-red-50 dark:hover:bg-red-900/20"
                >
                  <LogOut className="w-4 h-4" />
                  {t('common.logout')}
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  )
}
