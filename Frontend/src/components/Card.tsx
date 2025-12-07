import clsx from 'clsx'
import React from 'react'

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
  title?: string
  description?: string
}

export const Card = ({ title, description, children, className, ...props }: CardProps) => {
  return (
    <div
      className={clsx(
        'bg-white dark:bg-secondary-800 rounded-lg shadow-sm border border-secondary-200 dark:border-secondary-700 p-6',
        className
      )}
      {...props}
    >
      {title && <h3 className="text-lg font-semibold text-secondary-900 dark:text-white mb-2">{title}</h3>}
      {description && <p className="text-secondary-600 dark:text-secondary-400 mb-4">{description}</p>}
      {children}
    </div>
  )
}
