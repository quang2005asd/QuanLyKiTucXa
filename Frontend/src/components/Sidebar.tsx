import {
  BarChart3,
  Building2,
  FileText,
  Menu,
  Users,
  X,
} from 'lucide-react'
import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useLocation } from 'react-router-dom'
import clsx from 'clsx'

const navItems = [
  { icon: BarChart3, label: 'navigation.dashboard', path: '/dashboard' },
  { icon: Users, label: 'navigation.students', path: '/students' },
  { icon: Building2, label: 'navigation.rooms', path: '/rooms' },
  { icon: Building2, label: 'navigation.buildings', path: '/buildings' },
  { icon: FileText, label: 'navigation.contracts', path: '/contracts' },
  { icon: FileText, label: 'navigation.invoices', path: '/invoices' },
]

export const Sidebar = () => {
  const { t } = useTranslation()
  const [isOpen, setIsOpen] = useState(true)
  const location = useLocation()

  return (
    <>
      {/* Mobile Toggle */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="hidden md:hidden fixed bottom-6 right-6 z-50 p-3 bg-primary-500 text-white rounded-full shadow-lg"
      >
        {isOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
      </button>

      {/* Sidebar */}
      <aside
        className={clsx(
          'fixed left-0 top-0 h-screen w-64 bg-white dark:bg-secondary-800 border-r border-secondary-200 dark:border-secondary-700',
          'transition-transform duration-300 z-40',
          !isOpen && '-translate-x-full md:translate-x-0'
        )}
      >
        <div className="p-6 border-b border-secondary-200 dark:border-secondary-700">
          <h2 className="text-xl font-bold text-primary-600 dark:text-primary-400">KTXA</h2>
        </div>

        <nav className="p-4 space-y-2">
          {navItems.map((item) => {
            const Icon = item.icon
            const isActive = location.pathname === item.path
            return (
              <Link
                key={item.path}
                to={item.path}
                className={clsx(
                  'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
                  isActive
                    ? 'bg-primary-500 text-white'
                    : 'text-secondary-700 dark:text-secondary-300 hover:bg-secondary-100 dark:hover:bg-secondary-700'
                )}
              >
                <Icon className="w-5 h-5" />
                <span className="text-sm font-medium">{t(item.label)}</span>
              </Link>
            )
          })}
        </nav>
      </aside>

      {/* Overlay */}
      {isOpen && (
        <div
          className="hidden md:block fixed inset-0 bg-black/50 z-30"
          onClick={() => setIsOpen(false)}
        />
      )}
    </>
  )
}
