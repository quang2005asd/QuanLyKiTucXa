import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table } from '../components'
import { Plus, Edit2, Trash2 } from 'lucide-react'
import { useState } from 'react'

export default function RoomsPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [rooms] = useState([
    { id: 1, roomNumber: '101', building: 'Tòa A', floor: '1', capacity: 4, occupiedBeds: 4, price: 2000000, status: 'Full' },
    { id: 2, roomNumber: '102', building: 'Tòa A', floor: '1', capacity: 4, occupiedBeds: 2, price: 2000000, status: 'Available' },
    { id: 3, roomNumber: '201', building: 'Tòa B', floor: '2', capacity: 4, occupiedBeds: 0, price: 2000000, status: 'Available' },
  ])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-secondary-900 dark:text-white">{t('rooms.title')}</h1>
          <p className="text-secondary-600 dark:text-secondary-400 mt-2">Quản lý danh sách phòng</p>
        </div>
        <Button
          variant="primary"
          className="gap-2"
          onClick={() => setIsModalOpen(true)}
        >
          <Plus className="w-4 h-4" />
          {t('rooms.addRoom')}
        </Button>
      </div>

      <Card>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Input placeholder={t('common.search')} />
          <Input placeholder="Tòa nhà" />
          <Input placeholder="Trạng thái" />
        </div>
      </Card>

      <Card>
        <Table
          columns={[
            { key: 'roomNumber', label: t('rooms.roomNumber') },
            { key: 'building', label: t('rooms.building') },
            { key: 'capacity', label: t('rooms.capacity') },
            { key: 'occupiedBeds', label: t('rooms.occupiedBeds') },
            { key: 'price', label: t('rooms.price'), render: (value) => value.toLocaleString() + ' đ' },
            {
              key: 'status',
              label: t('common.status'),
              render: (value) => (
                <span className={`px-3 py-1 rounded-full text-xs font-medium ${
                  value === 'Available'
                    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-200'
                    : value === 'Full'
                    ? 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-200'
                    : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-200'
                }`}>
                  {value === 'Available' ? 'Có sẵn' : value === 'Full' ? 'Đầy' : 'Bảo trì'}
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
          data={rooms}
        />
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={t('rooms.addRoom')}
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
          <Input label={t('rooms.roomNumber')} />
          <Input label={t('rooms.building')} />
          <Input label={t('rooms.capacity')} type="number" />
          <Input label={t('rooms.price')} type="number" />
        </div>
      </Modal>
    </div>
  )
}
