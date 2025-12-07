import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table, Badge } from '../components'
import { Plus, Edit2, Trash2 } from 'lucide-react'
import { useState } from 'react'

export default function InvoicesPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [invoices] = useState([
    { id: 1, contract: 'HĐ001', month: 12, year: 2024, amount: 2000000, status: 'Paid', dueDate: '2024-12-10' },
    { id: 2, contract: 'HĐ002', month: 12, year: 2024, amount: 2000000, status: 'Pending', dueDate: '2024-12-15' },
    { id: 3, contract: 'HĐ003', month: 11, year: 2024, amount: 2000000, status: 'Overdue', dueDate: '2024-11-15' },
  ])

  const getStatusVariant = (status: string) => {
    switch (status) {
      case 'Paid':
        return 'success'
      case 'Pending':
        return 'warning'
      case 'Overdue':
        return 'danger'
      default:
        return 'secondary'
    }
  }

  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'Paid':
        return 'Đã thanh toán'
      case 'Pending':
        return 'Chờ thanh toán'
      case 'Overdue':
        return 'Quá hạn'
      default:
        return status
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-secondary-900 dark:text-white">{t('invoices.title')}</h1>
          <p className="text-secondary-600 dark:text-secondary-400 mt-2">Quản lý danh sách hóa đơn</p>
        </div>
        <Button
          variant="primary"
          className="gap-2"
          onClick={() => setIsModalOpen(true)}
        >
          <Plus className="w-4 h-4" />
          {t('invoices.addInvoice')}
        </Button>
      </div>

      <Card>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Input placeholder={t('common.search')} />
          <Input placeholder="Trạng thái" />
          <Input placeholder="Tháng/Năm" />
        </div>
      </Card>

      <Card>
        <Table
          columns={[
            { key: 'contract', label: t('invoices.contract') },
            { key: 'month', label: t('invoices.month') },
            { key: 'year', label: t('invoices.year') },
            { key: 'amount', label: t('invoices.amount'), render: (value) => value.toLocaleString() + ' đ' },
            { key: 'dueDate', label: t('invoices.dueDate') },
            {
              key: 'status',
              label: t('common.status'),
              render: (value) => (
                <Badge variant={getStatusVariant(value)}>
                  {getStatusLabel(value)}
                </Badge>
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
          data={invoices}
        />
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={t('invoices.addInvoice')}
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
          <Input label={t('invoices.contract')} />
          <Input label={t('invoices.month')} type="number" />
          <Input label={t('invoices.year')} type="number" />
          <Input label={t('invoices.amount')} type="number" />
          <Input label={t('invoices.dueDate')} type="date" />
        </div>
      </Modal>
    </div>
  )
}
