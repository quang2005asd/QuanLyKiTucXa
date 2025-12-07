import { BarChart3, Users, Building2, FileText } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { useEffect, useState } from 'react'
import { Card, Stat, Table } from '../components'
import { studentApi, roomApi, contractApi, invoiceApi, buildingApi } from '../lib/api'

export default function DashboardPage() {
  const { t } = useTranslation()
  const [stats, setStats] = useState({
    totalStudents: 0,
    totalRooms: 0,
    totalBuildings: 0,
    revenue: 0,
  })
  const [recentContracts, setRecentContracts] = useState([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const loadDashboardData = async () => {
      try {
        setIsLoading(true)
        const [students, rooms, buildings, contracts] = await Promise.all([
          studentApi.getAll(1, 1000),
          roomApi.getAll(1, 1000),
          buildingApi.getAll(1, 1000),
          contractApi.getAll(1, 10),
        ])

        setStats({
          totalStudents: students.data.data?.length || 0,
          totalRooms: rooms.data.data?.length || 0,
          totalBuildings: buildings.data.data?.length || 0,
          revenue: 0, // Calculate from invoices if needed
        })

        setRecentContracts(contracts.data.data?.slice(0, 5) || [])
      } catch (error) {
        console.error('Error loading dashboard data:', error)
      } finally {
        setIsLoading(false)
      }
    }

    loadDashboardData()
  }, [])

  const statItems = [
    {
      label: t('dashboard.totalStudents'),
      value: stats.totalStudents,
      change: { value: 12, isPositive: true },
      icon: <Users />,
    },
    {
      label: t('dashboard.totalRooms'),
      value: stats.totalRooms,
      change: { value: 2, isPositive: false },
      icon: <Building2 />,
    },
    {
      label: t('dashboard.totalBuildings'),
      value: stats.totalBuildings,
      change: { value: 0, isPositive: true },
      icon: <Building2 />,
    },
    {
      label: t('dashboard.revenueThisMonth'),
      value: '24.5M',
      change: { value: 8, isPositive: true },
      icon: <FileText />,
    },
  ]

  return (
    <div className="space-y-8">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-secondary-900 dark:text-white">{t('dashboard.title')}</h1>
        <p className="text-secondary-600 dark:text-secondary-400 mt-2">{t('dashboard.overview')}</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {statItems.map((stat, idx) => (
          <Stat key={idx} {...stat} />
        ))}
      </div>

      {/* Charts & Tables */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Chart Placeholder */}
        <Card title={t('dashboard.occupancyRate')} className="lg:col-span-2">
          <div className="h-64 bg-gradient-to-br from-primary-50 to-blue-50 dark:from-secondary-700 dark:to-secondary-600 rounded-lg flex items-center justify-center">
            <BarChart3 className="w-16 h-16 text-primary-200 dark:text-secondary-500" />
          </div>
        </Card>

        {/* Quick Stats */}
        <Card title={t('dashboard.pendingInvoices')}>
          <div className="space-y-4">
            <div className="flex justify-between items-center p-3 bg-secondary-50 dark:bg-secondary-700 rounded-lg">
              <span className="text-sm font-medium">Chưa thanh toán</span>
              <span className="text-xl font-bold text-danger">12</span>
            </div>
            <div className="flex justify-between items-center p-3 bg-secondary-50 dark:bg-secondary-700 rounded-lg">
              <span className="text-sm font-medium">Quá hạn</span>
              <span className="text-xl font-bold text-warning">3</span>
            </div>
            <div className="flex justify-between items-center p-3 bg-secondary-50 dark:bg-secondary-700 rounded-lg">
              <span className="text-sm font-medium">Đã thanh toán</span>
              <span className="text-xl font-bold text-success">145</span>
            </div>
          </div>
        </Card>
      </div>

      {/* Recent Contracts */}
      <Card title={t('dashboard.recentContracts')} description="Hợp đồng mới nhất">
        <Table
          columns={[
            { key: 'student', label: 'Sinh Viên' },
            { key: 'room', label: 'Phòng' },
            { key: 'startDate', label: 'Ngày Bắt Đầu' },
            {
              key: 'status',
              label: 'Trạng Thái',
              render: (value) => (
                <span className={`px-3 py-1 rounded-full text-xs font-medium ${
                  value === 'Active'
                    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-200'
                    : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-200'
                }`}>
                  {value === 'Active' ? 'Đang Hoạt Động' : 'Chờ Xử Lý'}
                </span>
              ),
            },
          ]}
          data={recentContracts}
          isLoading={isLoading}
        />
      </Card>
    </div>
  )
}
