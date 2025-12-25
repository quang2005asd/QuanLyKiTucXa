import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table } from '../components'
import { Plus, Edit2, Trash2, Building2 } from 'lucide-react'
import { useState, useEffect } from 'react'
import { roomApi, buildingApi, floorApi } from '../lib/api'
import { useCrud } from '../hooks/useCrud'

interface Building {
  id: number
  name: string
  address: string
  floorCount: number
}

interface Floor {
  id: number
  floorNumber: number
  buildingId: number
  building?: { name: string }
}

interface Room {
  id: number
  roomNumber: string
  capacity: number
  occupiedBeds: number
  status: string
  floorId: number
  floor?: {
    floorNumber: number
    building?: { name: string }
  }
}

const STATUS_OPTIONS = [
  { value: '', label: 'allStatuses' },
  { value: 'Available', label: 'available' },
  { value: 'Occupied', label: 'occupied' },
  { value: 'Maintenance', label: 'maintenance' }
]

// Giá phòng theo sức chứa
const ROOM_PRICES: { [key: number]: number } = {
  2: 3500000,
  4: 5000000,
  6: 6000000,
  8: 6500000
}

export default function RoomsPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [selectedBuildingId, setSelectedBuildingId] = useState<number | null>(null)
  const [statusFilter, setStatusFilter] = useState<string>('')
  const [buildings, setBuildings] = useState<Building[]>([])
  const [floors, setFloors] = useState<Floor[]>([])
  const [formData, setFormData] = useState({
    roomNumber: '',
    capacity: 2,
    floorId: 0
  })

  const {
    data: rooms,
    isLoading: loading,
    loadData: fetchRooms,
    create: createItem,
    update: updateItem,
    delete: deleteItem
  } = useCrud<Room>(roomApi, 100)

  const setRooms = (newRooms: Room[]) => {
    // Helper function to manually update rooms if needed
    // Note: useCrud manages state internally
  }

  // Load buildings on mount
  useEffect(() => {
    const loadBuildings = async () => {
      try {
        const response = await buildingApi.getAll(1, 100)
        setBuildings(response.data.data || [])
      } catch (error) {
        console.error('Failed to load buildings:', error)
      }
    }
    loadBuildings()
  }, [])

  // Load floors when building is selected
  useEffect(() => {
    const loadFloors = async () => {
      if (selectedBuildingId) {
        try {
          const response = await floorApi.getAll(1, 100)
          const buildingFloors = response.data.data.filter(
            (floor: Floor) => floor.buildingId === selectedBuildingId
          )
          setFloors(buildingFloors)
        } catch (error) {
          console.error('Failed to load floors:', error)
        }
      } else {
        setFloors([])
      }
    }
    loadFloors()
  }, [selectedBuildingId])

  // Load rooms by building
  useEffect(() => {
    const loadRooms = async () => {
      if (selectedBuildingId) {
        try {
          const response = await roomApi.getRoomsByBuilding(selectedBuildingId, 1, 100)
          // Note: rooms are managed by useCrud, we'll fetch all rooms first
          fetchRooms(1, 100)
        } catch (error) {
          console.error('Failed to load rooms:', error)
        }
      } else {
        fetchRooms(1, 100)
      }
    }
    loadRooms()
  }, [selectedBuildingId, fetchRooms])

  // Filter rooms by status and building
  const filteredRooms = (rooms || []).filter(room => {
    // Filter by building if selected
    if (selectedBuildingId && room.floor?.building) {
      const buildingMatches = room.floor.building.name === buildings.find(b => b.id === selectedBuildingId)?.name
      if (!buildingMatches) return false
    }
    
    // Filter by status if selected
    if (statusFilter && room.status !== statusFilter) {
      return false
    }
    
    return true
  })

  const handleSubmit = async () => {
    try {
      if (editingId) {
        await updateItem(String(editingId), formData)
      } else {
        await createItem(formData)
      }
      setIsModalOpen(false)
      resetForm()
      
      // Reload rooms
      fetchRooms(1, 100)
    } catch (error) {
      console.error('Failed to save room:', error)
    }
  }

  const handleEdit = (room: Room) => {
    setEditingId(room.id)
    setFormData({
      roomNumber: room.roomNumber,
      capacity: room.capacity,
      floorId: room.floorId
    })
    setIsModalOpen(true)
  }

  const handleDelete = async (id: number) => {
    if (window.confirm(t('rooms.confirmDelete'))) {
      await deleteItem(String(id))
      fetchRooms(1, 100)
    }
  }

  const resetForm = () => {
    setEditingId(null)
    setFormData({
      roomNumber: '',
      capacity: 2,
      floorId: 0
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
          <h1 className="text-3xl font-bold text-secondary-900 dark:text-white">{t('rooms.title')}</h1>
          <p className="text-secondary-600 dark:text-secondary-400 mt-2">Quản lý danh sách phòng theo tòa nhà</p>
        </div>
        <Button
          variant="primary"
          className="gap-2"
          onClick={openAddModal}
          disabled={!selectedBuildingId}
        >
          <Plus className="w-4 h-4" />
          {t('rooms.addRoom')}
        </Button>
      </div>

      {/* Building and Status Filter */}
      <Card>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Building Selector */}
          <div>
            <label className="flex items-center text-sm font-semibold text-secondary-900 dark:text-white mb-3">
              <Building2 className="w-5 h-5 mr-2 text-primary-600" />
              {t('rooms.selectBuilding')}
            </label>
            <div className="relative group">
              <select
                value={selectedBuildingId || ''}
                onChange={(e) => setSelectedBuildingId(e.target.value ? Number(e.target.value) : null)}
                className="w-full px-4 py-3.5 pr-12 text-base border-2 border-secondary-300 dark:border-secondary-600 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 bg-white dark:bg-secondary-800 text-secondary-900 dark:text-white font-medium shadow-sm hover:border-primary-400 hover:shadow-md transition-all duration-200 cursor-pointer appearance-none"
              >
                <option value="" className="py-3 bg-white dark:bg-secondary-800 text-secondary-500">{t('rooms.allBuildings')}</option>
                {buildings.map((building) => (
                  <option 
                    key={building.id} 
                    value={building.id} 
                    className="py-3 bg-white dark:bg-secondary-800 hover:bg-primary-50"
                  >
                    {building.name} • {building.address}
                  </option>
                ))}
              </select>
              <div className="absolute inset-y-0 right-0 flex items-center pr-4 pointer-events-none">
                <svg className="w-5 h-5 text-secondary-400 group-hover:text-primary-500 transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>
            </div>
            {selectedBuildingId && (
              <div className="mt-3 flex items-center gap-2 px-3 py-2 bg-primary-50 dark:bg-primary-900/20 border border-primary-200 dark:border-primary-800 rounded-lg">
                <svg className="w-4 h-4 text-primary-600 dark:text-primary-400" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                </svg>
                <span className="text-sm text-primary-700 dark:text-primary-300 font-medium">
                  Đang xem: <strong>{buildings.find(b => b.id === selectedBuildingId)?.name}</strong>
                </span>
              </div>
            )}
          </div>

          {/* Status Filter */}
          <div>
            <label className="flex items-center text-sm font-semibold text-secondary-900 dark:text-white mb-3">
              <svg className="w-5 h-5 mr-2 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
              </svg>
              {t('rooms.filterByStatus')}
            </label>
            <div className="relative group">
              <select
                value={statusFilter}
                onChange={(e) => setStatusFilter(e.target.value)}
                className="w-full px-4 py-3.5 pr-12 text-base border-2 border-secondary-300 dark:border-secondary-600 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 bg-white dark:bg-secondary-800 text-secondary-900 dark:text-white font-medium shadow-sm hover:border-primary-400 hover:shadow-md transition-all duration-200 cursor-pointer appearance-none"
              >
                {STATUS_OPTIONS.map((option) => (
                  <option 
                    key={option.value} 
                    value={option.value} 
                    className="py-3 bg-white dark:bg-secondary-800"
                  >
                    {option.value === '' && '📋 '}
                    {option.value === 'Available' && '✅ '}
                    {option.value === 'Occupied' && '🔴 '}
                    {option.value === 'Maintenance' && '🔧 '}
                    {t(`rooms.${option.label}`)}
                  </option>
                ))}
              </select>
              <div className="absolute inset-y-0 right-0 flex items-center pr-4 pointer-events-none">
                <svg className="w-5 h-5 text-secondary-400 group-hover:text-primary-500 transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                </svg>
              </div>
            </div>
            {statusFilter && (
              <div className="mt-3 flex items-center gap-2 px-3 py-2 bg-primary-50 dark:bg-primary-900/20 border border-primary-200 dark:border-primary-800 rounded-lg">
                <svg className="w-4 h-4 text-primary-600 dark:text-primary-400" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                </svg>
                <span className="text-sm text-primary-700 dark:text-primary-300 font-medium">
                  Lọc: <strong>{t(`rooms.${STATUS_OPTIONS.find(o => o.value === statusFilter)?.label}`)}</strong>
                </span>
              </div>
            )}
          </div>
        </div>
      </Card>

      {/* Rooms Table */}
      <Card>
        {loading ? (
          <div className="text-center py-8">{t('common.loading')}</div>
        ) : filteredRooms.length === 0 ? (
          <div className="text-center py-8 text-secondary-500">
            {selectedBuildingId 
              ? 'Không có phòng trong tòa nhà này' 
              : 'Vui lòng chọn tòa nhà để xem danh sách phòng'}
          </div>
        ) : (
          <Table
            columns={[
              { key: 'roomNumber', label: t('rooms.roomNumber') },
              {
                key: 'floor',
                label: t('rooms.building') + ' / ' + t('rooms.floor'),
                render: (_, room) => (
                  <span>
                    {room.floor?.building?.name || 'N/A'} / Tầng {room.floor?.floorNumber || 'N/A'}
                  </span>
                )
              },
              {
                key: 'capacity',
                label: t('rooms.capacity'),
                render: (value) => `${value} người`
              },
              {
                key: 'occupiedBeds',
                label: t('rooms.occupiedBeds'),
                render: (value, room) => `${value}/${room.capacity}`
              },
              {
                key: 'price',
                label: t('rooms.price'),
                render: (_, room) => {
                  const price = ROOM_PRICES[room.capacity] || 0
                  return price.toLocaleString() + ' đ/tháng'
                }
              },
              {
                key: 'status',
                label: t('common.status'),
                render: (value) => (
                  <span className={`px-3 py-1 rounded-full text-xs font-medium ${
                    value === 'Available'
                      ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-200'
                      : value === 'Occupied'
                      ? 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-200'
                      : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-200'
                  }`}>
                    {value === 'Available' && t('rooms.available')}
                    {value === 'Occupied' && t('rooms.occupied')}
                    {value === 'Maintenance' && t('rooms.maintenance')}
                  </span>
                )
              },
              {
                key: 'actions',
                label: t('common.actions'),
                render: (_, room) => (
                  <div className="flex gap-2">
                    <button
                      onClick={() => handleEdit(room)}
                      className="text-primary-600 hover:text-primary-700"
                    >
                      <Edit2 className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleDelete(room.id)}
                      className="text-danger hover:text-red-700"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                )
              }
            ]}
            data={filteredRooms}
          />
        )}
      </Card>

      {/* Add/Edit Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false)
          resetForm()
        }}
        title={editingId ? t('rooms.editRoom') : t('rooms.addRoom')}
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
              disabled={!formData.roomNumber || !formData.floorId}
            >
              {t('common.save')}
            </Button>
          </>
        }
      >
        <div className="space-y-4">
          <Input
            label={t('rooms.roomNumber')}
            value={formData.roomNumber}
            onChange={(e) => setFormData({ ...formData, roomNumber: e.target.value })}
            placeholder="Ví dụ: 101, 102, 201..."
          />

          <div>
            <label className="block text-sm font-medium text-secondary-700 dark:text-secondary-300 mb-2">
              {t('rooms.floor')}
            </label>
            <select
              value={formData.floorId}
              onChange={(e) => setFormData({ ...formData, floorId: Number(e.target.value) })}
              className="w-full px-4 py-2 border border-secondary-300 dark:border-secondary-600 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent bg-white dark:bg-secondary-800 text-secondary-900 dark:text-white"
            >
              <option value={0}>Chọn tầng</option>
              {floors.map((floor) => (
                <option key={floor.id} value={floor.id}>
                  Tầng {floor.floorNumber}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-secondary-700 dark:text-secondary-300 mb-2">
              {t('rooms.capacity')} (Loại phòng)
            </label>
            <select
              value={formData.capacity}
              onChange={(e) => setFormData({ ...formData, capacity: Number(e.target.value) })}
              className="w-full px-4 py-2 border border-secondary-300 dark:border-secondary-600 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent bg-white dark:bg-secondary-800 text-secondary-900 dark:text-white"
            >
              <option value={2}>Phòng 2 người - 3,500,000 đ/tháng</option>
              <option value={4}>Phòng 4 người - 5,000,000 đ/tháng</option>
              <option value={6}>Phòng 6 người - 6,000,000 đ/tháng</option>
              <option value={8}>Phòng 8 người - 6,500,000 đ/tháng</option>
            </select>
          </div>

          {/* Display calculated price */}
          <div className="bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg p-4">
            <p className="text-sm text-green-800 dark:text-green-200">
              <strong>Giá phòng:</strong> {ROOM_PRICES[formData.capacity]?.toLocaleString() || 0} đ/tháng
            </p>
          </div>
        </div>
      </Modal>
    </div>
  )
}
