import clsx from 'clsx'

interface BadgeProps {
  variant?: 'primary' | 'success' | 'warning' | 'danger' | 'secondary'
  children: React.ReactNode
  className?: string
}

const variants = {
  primary: 'bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-200',
  success: 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-200',
  warning: 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-200',
  danger: 'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-200',
  secondary: 'bg-secondary-100 dark:bg-secondary-700 text-secondary-800 dark:text-secondary-200',
}

export const Badge = ({ variant = 'secondary', children, className }: BadgeProps) => {
  return (
    <span
      className={clsx(
        'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
        variants[variant],
        className
      )}
    >
      {children}
    </span>
  )
}
