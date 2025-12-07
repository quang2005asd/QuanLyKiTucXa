import clsx from 'clsx'

interface StatProps {
  label: string
  value: string | number
  change?: {
    value: number
    isPositive: boolean
  }
  icon?: React.ReactNode
  className?: string
}

export const Stat = ({ label, value, change, icon, className }: StatProps) => {
  return (
    <div
      className={clsx(
        'bg-white dark:bg-secondary-800 rounded-lg shadow-sm border border-secondary-200 dark:border-secondary-700 p-6',
        className
      )}
    >
      <div className="flex items-center justify-between">
        <div className="flex-1">
          <p className="text-sm font-medium text-secondary-600 dark:text-secondary-400">{label}</p>
          <p className="text-3xl font-bold text-secondary-900 dark:text-white mt-2">{value}</p>
          {change && (
            <p
              className={clsx(
                'text-sm font-medium mt-2',
                change.isPositive ? 'text-green-600' : 'text-red-600'
              )}
            >
              {change.isPositive ? '+' : '-'}{Math.abs(change.value)}%
            </p>
          )}
        </div>
        {icon && (
          <div className="text-primary-500 text-4xl opacity-20">
            {icon}
          </div>
        )}
      </div>
    </div>
  )
}
