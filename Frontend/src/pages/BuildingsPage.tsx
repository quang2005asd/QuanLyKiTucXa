import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table } from '../components'
import { Plus, Edit2, Trash2, Building2 } from 'lucide-react'
import { useState, useEffect } from 'react'
import { buildingApi } from '../lib/api'
import { useCrud } from '../hooks/useCrud'

interface Building {
  id: number
  name: string
  address: string
  floorCount: number
}

export default function BuildingsPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({
    name: '',
    address: '',
    totalFloors: 1
  })

  const {
    data: buildings,
    isLoading: loading,
    loadData: fetchItems,
    create: createItem,
    update: updateItem,
    delete: deleteItem
  } = useCrud<Building>(buildingApi, 50)

  // Load buildings on mount
  useEffect(() => {
    fetchItems(1, 50)
  }, [fetchItems])

  const handleSubmit = async () => {
    try {
      const payload = {
        name: formData.name,
        address: formData.address,
        totalFloors: formData.totalFloors
      }
      console.log('=== SUBMITTING BUILDING ===')
      console.log('Payload:', JSON.stringify(payload, null, 2))
      console.log('EditingId:', editingId)
      
      if (editingId) {
        const response = await updateItem(String(editingId), payload)
        console.log('Update response:', response)
      } else {
        const response = await createItem(payload)
        console.log('Create response:', response)
      }
      setIsModalOpen(false)
      resetForm()
      fetchItems(1, 50)
    } catch (error: any) {
      console.error('=== ERROR ===')
      console.error('Error:', error)
      console.error('Response data:', JSON.stringify(error.response?.data, null, 2))
      console.error('Status:', error.response?.status)
      alert(`Lỗi: ${error.response?.data?.message || error.response?.data?.title || 'Không thể lưu'}\n\n${JSON.stringify(error.response?.data?.errors || {}, null, 2)}`)
    }
  }

  const handleEdit = (building: Building) => {
    setEditingId(building.id)
    setFormData({
      name: building.name,
      address: building.address,
      totalFloors: building.floorCount
    })
    setIsModalOpen(true)
  }

  const handleDelete = async (id: number) => {
    if (window.confirm(t('buildings.confirmDelete'))) {
      try {
        console.log('Deleting building with id:', id)
        await deleteItem(String(id))
        console.log('Delete successful')
        fetchItems(1, 50)
      } catch (error: any) {
        console.error('Delete error:', error)
        console.error('Error response:', error.response?.data)
        alert(`Không thể xóa: ${error.response?.data?.message || error.message}`)
      }
    }
  }

  const resetForm = () => {
    setEditingId(null)
    setFormData({
      name: '',
      address: '',
      totalFloors: 1
    })
  }

  const openAddModal = () => {
    resetForm()
    setIsModalOpen(true)
  }

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
          onClick={openAddModal}
        >
          <Plus className="w-4 h-4" />
          {t('buildings.addBuilding')}
        </Button>
      </div>

      <Card>
        {loading ? (
          <div className="text-center py-8">{t('common.loading')}</div>
        ) : buildings.length === 0 ? (
          <div className="text-center py-8 text-secondary-500">{t('buildings.noBuildings')}</div>
        ) : (
        <Table
          columns={[
            { key: 'name', label: t('buildings.name') },
            { key: 'address', label: t('buildings.address') },
            { key: 'floorCount', label: t('buildings.floorCount') },
            { key: 'totalRooms', label: t('buildings.totalRooms') },
            {
              key: 'actions',
              label: t('common.actions'),
              render: (_, building) => (
                <div className="flex gap-2">
                  <button
                    onClick={() => handleEdit(building)}
                    className="text-primary-600 hover:text-primary-700"
                  >
                    <Edit2 className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleDelete(building.id)}
                    className="text-danger hover:text-red-700"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              ),
            },
          ]}
          data={buildings}
        />
        )}
      </Card>

      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false)
          resetForm()
        }}
        title={editingId ? t('buildings.editBuilding') : t('buildings.addBuilding')}
        actions={
          <>
            <Button
              variant="secondary"
              onClick={() => {
                setIsModalOpen(false)
                resetForm()
              }}
            >
              {t('common.cancel')}
            </Button>
            <Button
              variant="primary"
              onClick={handleSubmit}
              disabled={!formData.name || formData.name.length < 2 || !formData.address || formData.address.length < 5 || formData.totalFloors < 1}
            >
              {t('common.save')}
            </Button>
          </>
        }
      >
        <div className="space-y-4">
          <Input
            label={t('buildings.name')}
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="Ví dụ: Tòa A, Tòa B..."
            required
            minLength={2}
          />
          <Input
            label={t('buildings.address')}
            value={formData.address}
            onChange={(e) => setFormData({ ...formData, address: e.target.value })}
            placeholder="Địa chỉ tòa nhà (tối thiểu 5 ký tự)"
            required
            minLength={5}
          />
          <Input
            label={t('buildings.floorCount')}
            type="number"
            min="1"
            max="50"
            value={formData.totalFloors}
            onChange={(e) => setFormData({ ...formData, totalFloors: Number(e.target.value) })}
            required
          />
        </div>
      </Modal>
    </div>
  )
}
