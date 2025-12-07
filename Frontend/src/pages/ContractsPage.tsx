import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table } from '../components'
import { Plus, Edit2, Trash2 } from 'lucide-react'
import { useState } from 'react'

export default function ContractsPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [contracts] = useState([
    { id: 1, student: 'Nguyễn Văn A', room: '101', startDate: '2024-01-01', endDate: '2024-12-31', monthlyPrice: 2000000, status: 'Active' },
    { id: 2, student: 'Trần Thị B', room: '102', startDate: '2024-02-01', endDate: '2025-01-31', monthlyPrice: 2000000, status: 'Active' },
    { id: 3, student: 'Lê Văn C', room: '201', startDate: '2023-06-01', endDate: '2024-05-31', monthlyPrice: 2000000, status: 'Expired' },
  ])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-secondary-900 dark:text-white">{t('contracts.title')}</h1>
          <p className="text-secondary-600 dark:text-secondary-400 mt-2">Quản lý danh sách hợp đồng</p>
        </div>
        <Button
          variant="primary"
          className="gap-2"
          onClick={() => setIsModalOpen(true)}
        >
          <Plus className="w-4 h-4" />
          {t('contracts.addContract')}
        </Button>
      </div>

      <Card>
        <Input placeholder={t('common.search')} />
      </Card>

      <Card>
        <Table
          columns={[
            { key: 'student', label: t('contracts.student') },
            { key: 'room', label: t('contracts.room') },
            { key: 'startDate', label: t('contracts.startDate') },
            { key: 'endDate', label: t('contracts.endDate') },
            { key: 'monthlyPrice', label: t('contracts.monthlyPrice'), render: (value) => value.toLocaleString() + ' đ' },
            {
              key: 'status',
              label: t('common.status'),
              render: (value) => (
                <span className={`px-3 py-1 rounded-full text-xs font-medium ${
                  value === 'Active'
                    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-200'
                    : 'bg-secondary-100 text-secondary-800 dark:bg-secondary-700 dark:text-secondary-200'
                }`}>
                  {value === 'Active' ? 'Đang hoạt động' : 'Hết hạn'}
                </span>
              ),
            },
            {
              key: 'actions',
              label: t('common.actions'),
              render: () => (
                <div className="flex gap-2">
                  <button className="text-primary-600 hover:text-primary-700">
                    <Edit2 className="w-4 h-4" />
                  </button>
                  <button className="text-danger hover:text-red-700">
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              ),
            },
          ]}
          data={contracts}
        />
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={t('contracts.addContract')}
        actions={
          <>
            <Button variant="secondary" onClick={() => setIsModalOpen(false)}>
              {t('common.cancel')}
            </Button>
            <Button variant="primary">
              {t('common.save')}
            </Button>
          </>
        }
      >
        <div className="space-y-4">
          <Input label={t('contracts.student')} />
          <Input label={t('contracts.room')} />
          <Input label={t('contracts.startDate')} type="date" />
          <Input label={t('contracts.endDate')} type="date" />
          <Input label={t('contracts.monthlyPrice')} type="number" />
        </div>
      </Modal>
    </div>
  )
}
