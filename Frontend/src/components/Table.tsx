import clsx from 'clsx'

interface TableColumn {
  key: string
  label: string
  sortable?: boolean
  render?: (value: any, row: any) => React.ReactNode
}

interface TableProps {
  columns: TableColumn[]
  data: any[]
  isLoading?: boolean
  onRowClick?: (row: any) => void
}

export const Table = ({ columns, data, isLoading = false, onRowClick }: TableProps) => {
  if (isLoading) {
    return (
      <div className="w-full h-64 flex items-center justify-center">
        <div className="text-center">
          <div className="w-12 h-12 border-4 border-primary-200 border-t-primary-500 rounded-full animate-spin mx-auto mb-2" />
          <p className="text-secondary-600 dark:text-secondary-400">Đang tải...</p>
        </div>
      </div>
    )
  }

  if (data.length === 0) {
    return (
      <div className="w-full h-64 flex items-center justify-center">
        <p className="text-secondary-500 dark:text-secondary-400">Không có dữ liệu</p>
      </div>
    )
  }

  return (
    <div className="overflow-x-auto rounded-lg border border-secondary-200 dark:border-secondary-700">
      <table className="w-full">
        <thead>
          <tr className="bg-secondary-50 dark:bg-secondary-700 border-b border-secondary-200 dark:border-secondary-600">
            {columns.map((col) => (
              <th
                key={col.key}
                className="px-6 py-3 text-left text-sm font-semibold text-secondary-700 dark:text-secondary-300"
              >
                {col.label}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {data.map((row, idx) => (
            <tr
              key={idx}
              onClick={() => onRowClick?.(row)}
              className={clsx(
                'border-b border-secondary-200 dark:border-secondary-700',
                'hover:bg-secondary-50 dark:hover:bg-secondary-700/50',
                onRowClick && 'cursor-pointer'
              )}
            >
              {columns.map((col) => (
                <td key={col.key} className="px-6 py-4 text-sm text-secondary-900 dark:text-secondary-100">
                  {col.render ? col.render(row[col.key], row) : row[col.key]}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
