import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table } from '../components'
import { Plus, Edit2, Trash2 } from 'lucide-react'
import { useState } from 'react'

export default function BuildingsPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [buildings] = useState([
    { id: 1, name: 'Tòa A', address: '123 Đường ABC', floorCount: 5, totalRooms: 20 },
    { id: 2, name: 'Tòa B', address: '456 Đường XYZ', floorCount: 4, totalRooms: 16 },
    { id: 3, name: 'Tòa C', address: '789 Đường 123', floorCount: 3, totalRooms: 12 },
  ])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-secondary-900 dark:text-white">{t('buildings.title')}</h1>
          <p className="text-secondary-600 dark:text-secondary-400 mt-2">Quản lý danh sách tòa nhà</p>
        </div>
        <Button
          variant="primary"
          className="gap-2"
          onClick={() => setIsModalOpen(true)}
        >
          <Plus className="w-4 h-4" />
          {t('buildings.addBuilding')}
        </Button>
      </div>

      <Card>
        <Input placeholder={t('common.search')} />
      </Card>

      <Card>
        <Table
          columns={[
            { key: 'name', label: t('buildings.name') },
            { key: 'address', label: t('buildings.address') },
            { key: 'floorCount', label: t('buildings.floorCount') },
            { key: 'totalRooms', label: t('buildings.totalRooms') },
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
          data={buildings}
        />
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={t('buildings.addBuilding')}
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
          <Input label={t('buildings.name')} />
          <Input label={t('buildings.address')} />
          <Input label={t('buildings.floorCount')} type="number" />
        </div>
      </Modal>
    </div>
  )
}
