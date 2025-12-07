import clsx from 'clsx'
import React from 'react'

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
  helperText?: string
}

export const Input = ({ label, error, helperText, className, ...props }: InputProps) => {
  return (
    <div className="w-full">
      {label && (
        <label className="block text-sm font-medium text-secondary-700 dark:text-secondary-300 mb-2">
          {label}
        </label>
      )}
      <input
        className={clsx(
          'w-full px-4 py-2.5 border rounded-lg bg-white dark:bg-secondary-800 text-secondary-900 dark:text-white',
          'border-secondary-300 dark:border-secondary-600',
          'placeholder-secondary-400 dark:placeholder-secondary-500',
          'focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent',
          'disabled:bg-secondary-100 dark:disabled:bg-secondary-700 disabled:text-secondary-500',
          error && 'border-danger focus:ring-danger',
          className
        )}
        {...props}
      />
      {error && <p className="mt-1 text-sm text-danger">{error}</p>}
      {helperText && <p className="mt-1 text-sm text-secondary-500 dark:text-secondary-400">{helperText}</p>}
    </div>
  )
}
