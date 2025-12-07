import clsx from 'clsx'
import { X } from 'lucide-react'
import React, { useEffect } from 'react'

interface ModalProps {
  isOpen: boolean
  onClose: () => void
  title?: string
  description?: string
  children?: React.ReactNode
  actions?: React.ReactNode
  size?: 'sm' | 'md' | 'lg'
}

export const Modal = ({
  isOpen,
  onClose,
  title,
  description,
  children,
  actions,
  size = 'md',
}: ModalProps) => {
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden'
    } else {
      document.body.style.overflow = 'auto'
    }
    return () => {
      document.body.style.overflow = 'auto'
    }
  }, [isOpen])

  if (!isOpen) return null

  const sizes = {
    sm: 'max-w-sm',
    md: 'max-w-md',
    lg: 'max-w-lg',
  }

  return (
    <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
      <div
        className={clsx(
          'bg-white dark:bg-secondary-800 rounded-lg shadow-xl w-full',
          sizes[size],
          'animate-in fade-in zoom-in-95'
        )}
      >
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-secondary-200 dark:border-secondary-700">
          <div>
            {title && <h3 className="text-lg font-semibold">{title}</h3>}
            {description && <p className="text-sm text-secondary-600 dark:text-secondary-400 mt-1">{description}</p>}
          </div>
          <button
            onClick={onClose}
            className="text-secondary-500 hover:text-secondary-700 dark:hover:text-secondary-300"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Content */}
        <div className="p-6">
          {children}
        </div>

        {/* Footer */}
        {actions && (
          <div className="flex justify-end gap-3 p-6 border-t border-secondary-200 dark:border-secondary-700">
            {actions}
          </div>
        )}
      </div>
    </div>
  )
}
