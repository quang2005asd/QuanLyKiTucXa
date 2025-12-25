import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table, Alert } from '../components'
import { Plus, Edit2, Trash2, X, Search } from 'lucide-react'
import { useEffect, useState } from 'react'
import { contractApi, studentApi, roomApi } from '../lib/api'
import { useCrud } from '../hooks/useCrud'

interface ContractForm {
  contractNumber: string
  studentIds: number[]
  roomId: number
  startDate: string
  endDate: string
}

// Bảng giá phòng theo loại
const ROOM_PRICES = {
  2: 3500000,  // Phòng 2: 3.5 triệu
  4: 5000000,  // Phòng 4: 5 triệu
  6: 6000000,  // Phòng 6: 6 triệu
  8: 6500000,  // Phòng 8: 6.5 triệu
}

export default function ContractsPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [students, setStudents] = useState<any[]>([])
  const [availableRooms, setAvailableRooms] = useState<any[]>([])
  const [selectedStudents, setSelectedStudents] = useState<number[]>([])
  const [searchStudent, setSearchStudent] = useState('')
  const [formData, setFormData] = useState<ContractForm>({
    contractNumber: '',
    studentIds: [],
    roomId: 0,
    startDate: '',
    endDate: '',
  })
  
  const crud = useCrud(contractApi, 10)

  useEffect(() => {
    crud.loadData(1, 10)
    loadStudentsAndRooms()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const loadStudentsAndRooms = async () => {
    try {
      const [studentsRes, roomsRes] = await Promise.all([
        studentApi.getAll(1, 100),
        roomApi.getAvailableRooms(1, 100),
      ])
      setStudents(studentsRes.data.data || [])
      setAvailableRooms(roomsRes.data.data || [])
    } catch (error) {
      console.error('Error loading students and rooms:', error)
    }
  }

  const handleOpenModal = (contract?: any) => {
    if (contract) {
      setEditingId(contract.id)
      setSelectedStudents(contract.students?.map((s: any) => s.id) || [])
      setFormData({
        contractNumber: contract.contractNumber,
        studentIds: contract.students?.map((s: any) => s.id) || [],
        roomId: contract.roomId,
        startDate: contract.startDate?.split('T')[0] || '',
        endDate: contract.endDate?.split('T')[0] || '',
      })
    } else {
      setEditingId(null)
      setSelectedStudents([])
      setSearchStudent('')
      setFormData({
        contractNumber: '',
        studentIds: [],
        roomId: 0,
        startDate: '',
        endDate: '',
      })
    }
    setIsModalOpen(true)
  }

  const handleToggleStudent = (studentId: number) => {
    setSelectedStudents((prev) => {
      if (prev.includes(studentId)) {
        return prev.filter(id => id !== studentId)
      } else {
        return [...prev, studentId]
      }
    })
  }

  const handleRemoveStudent = (studentId: number) => {
    setSelectedStudents((prev) => prev.filter(id => id !== studentId))
  }

  const handleSaveContract = async () => {
    if (!formData.contractNumber || selectedStudents.length === 0 || !formData.roomId || !formData.startDate || !formData.endDate) {
      alert('Vui lòng điền đầy đủ thông tin và chọn ít nhất 1 sinh viên')
      return
    }

    const selectedRoom = availableRooms.find(r => r.id === formData.roomId)
    const monthlyRent = selectedRoom ? (ROOM_PRICES[selectedRoom.capacity as keyof typeof ROOM_PRICES] || 0) : 0

    const dataToSubmit = {
      ...formData,
      studentIds: selectedStudents,
      depositAmount: monthlyRent, // Đặt cọc = 1 tháng tiền phòng
      monthlyRent: monthlyRent,
    }

    try {
      if (editingId) {
        await crud.update(editingId, dataToSubmit)
      } else {
        await crud.create(dataToSubmit)
      }
      setIsModalOpen(false)
      setSelectedStudents([])
      setSearchStudent('')
      await loadStudentsAndRooms()
    } catch (error) {
      console.error('Error saving contract:', error)
    }
  }

  const handleDeleteContract = async (id: string) => {
    if (window.confirm('Bạn chắc chắn muốn xóa hợp đồng này?')) {
      try {
        await crud.delete(id)
        await loadStudentsAndRooms()
      } catch (error) {
        console.error('Error deleting contract:', error)
      }
    }
  }

  // Filter students based on search (top 10 matches)
  const filteredStudents = students
    .filter(student => 
      searchStudent === '' ||
      student.fullName.toLowerCase().includes(searchStudent.toLowerCase()) ||
      student.studentCode.toLowerCase().includes(searchStudent.toLowerCase()) ||
      student.email.toLowerCase().includes(searchStudent.toLowerCase())
    )
    .slice(0, 10)

  const selectedRoom = availableRooms.find(r => r.id === formData.roomId)
  const calculatedPrice = selectedRoom ? (ROOM_PRICES[selectedRoom.capacity as keyof typeof ROOM_PRICES] || 0) : 0

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-secondary-900 dark:text-white">{t('contracts.title')}</h1>
          <p className="text-secondary-600 dark:text-secondary-400 mt-2">Quản lý danh sách hợp đồng</p>
        </div>
        <Button
          variant="primary"
          className="gap-2"
          onClick={() => handleOpenModal()}
        >
          <Plus className="w-4 h-4" />
          {t('contracts.addContract')}
        </Button>
      </div>

      {/* Error Alert */}
      {crud.error && (
        <Alert type="error" className="mb-4">
          {crud.error}
          <button
            onClick={crud.clearError}
            className="ml-4 text-sm underline"
          >
            Đóng
          </button>
        </Alert>
      )}

      {/* Table */}
      <Card>
        <Table
          columns={[
            { key: 'contractNumber', label: t('contracts.contractNumber') },
            {
              key: 'students',
              label: 'Sinh viên',
              render: (students: any[]) => (
                <div className="flex flex-wrap gap-1">
                  {students?.map((student, idx) => (
                    <span
                      key={idx}
                      className="px-2 py-1 bg-blue-100 text-blue-800 rounded text-xs"
                    >
                      {student.fullName}
                    </span>
                  ))}
                </div>
              ),
            },
            { key: 'roomNumber', label: t('contracts.room') },
            {
              key: 'startDate',
              label: t('contracts.startDate'),
              render: (value) => new Date(value).toLocaleDateString('vi-VN'),
            },
            {
              key: 'endDate',
              label: t('contracts.endDate'),
              render: (value) => new Date(value).toLocaleDateString('vi-VN'),
            },
            {
              key: 'monthlyRent',
              label: t('contracts.monthlyRent'),
              render: (value) => value?.toLocaleString() + ' đ',
            },
            {
              key: 'status',
              label: t('common.status'),
              render: (value) => (
                <span
                  className={`px-3 py-1 rounded-full text-xs font-medium ${
                    value === 'Active'
                      ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-200'
                      : value === 'Completed'
                      ? 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-200'
                      : 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-200'
                  }`}
                >
                  {value === 'Active' ? 'Đang hoạt động' : value === 'Completed' ? 'Hoàn thành' : value}
                </span>
              ),
            },
            {
              key: 'actions',
              label: t('common.actions'),
              render: (_, contract) => (
                <div className="flex gap-2">
                  <button
                    className="text-primary-600 hover:text-primary-700"
                    onClick={() => handleOpenModal(contract)}
                  >
                    <Edit2 className="w-4 h-4" />
                  </button>
                  <button
                    className="text-danger hover:text-red-700"
                    onClick={() => handleDeleteContract(contract.id)}
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              ),
            },
          ]}
          data={crud.data}
          isLoading={crud.isLoading}
        />
      </Card>

      {/* Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={editingId ? t('contracts.editContract') : t('contracts.addContract')}
        actions={
          <>
            <Button variant="secondary" onClick={() => setIsModalOpen(false)}>
              {t('common.cancel')}
            </Button>
            <Button variant="primary" onClick={handleSaveContract}>
              {t('common.save')}
            </Button>
          </>
        }
      >
        <div className="space-y-4">
          <Input
            label={t('contracts.contractNumber')}
            value={formData.contractNumber}
            onChange={(e) =>
              setFormData({ ...formData, contractNumber: e.target.value })
            }
            disabled={!!editingId}
          />

          {/* Student Search & Multi-Select */}
          <div>
            <label className="block text-sm font-medium text-secondary-700 dark:text-secondary-300 mb-2">
              {t('contracts.selectStudents')} <span className="text-red-500">*</span>
            </label>
            
            {/* Search Box */}
            <div className="relative mb-2">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-secondary-400" />
              <input
                type="text"
                placeholder={t('contracts.searchStudents')}
                value={searchStudent}
                onChange={(e) => setSearchStudent(e.target.value)}
                className="w-full pl-10 pr-3 py-2 border border-secondary-300 rounded-lg focus:ring-2 focus:ring-primary-500 dark:bg-secondary-800 dark:border-secondary-600"
              />
            </div>

            {/* Selected Students */}
            {selectedStudents.length > 0 && (
              <div className="mb-2 p-2 border rounded-lg bg-blue-50 dark:bg-blue-900/20">
                <div className="flex flex-wrap gap-2">
                  {students
                    .filter(s => selectedStudents.includes(s.id))
                    .map((student) => (
                      <span
                        key={student.id}
                        className="inline-flex items-center gap-1 px-3 py-1 bg-blue-600 text-white rounded-full text-sm"
                      >
                        {student.fullName}
                        <button
                          onClick={() => handleRemoveStudent(student.id)}
                          className="hover:bg-blue-700 rounded-full p-0.5"
                        >
                          <X className="w-3 h-3" />
                        </button>
                      </span>
                    ))}
                </div>
              </div>
            )}

            {/* Student List - Only 10 matches */}
            <div className="border rounded-lg max-h-48 overflow-y-auto">
              {filteredStudents.length > 0 ? (
                filteredStudents.map((student) => (
                  <label
                    key={student.id}
                    className="flex items-center gap-3 p-3 hover:bg-secondary-50 dark:hover:bg-secondary-800 cursor-pointer border-b last:border-b-0"
                  >
                    <input
                      type="checkbox"
                      checked={selectedStudents.includes(student.id)}
                      onChange={() => handleToggleStudent(student.id)}
                      className="w-4 h-4 text-primary-600 rounded focus:ring-primary-500"
                    />
                    <div className="flex-1">
                      <div className="font-medium">{student.fullName}</div>
                      <div className="text-sm text-secondary-500">{student.studentCode} - {student.email}</div>
                    </div>
                  </label>
                ))
              ) : (
                <div className="p-4 text-center text-secondary-500">
                  {searchStudent ? 'Không tìm thấy sinh viên' : 'Nhập tên để tìm kiếm'}
                </div>
              )}
            </div>
            {filteredStudents.length === 10 && searchStudent && (
              <p className="text-xs text-secondary-500 mt-1">
                Hiển thị 10 kết quả đầu tiên. Nhập chi tiết hơn để thu hẹp kết quả.
              </p>
            )}
          </div>

          {/* Room Select with Price Info */}
          <div>
            <label className="block text-sm font-medium text-secondary-700 dark:text-secondary-300 mb-2">
              {t('contracts.selectRoom')} <span className="text-red-500">*</span>
            </label>
            <select
              value={formData.roomId}
              onChange={(e) =>
                setFormData({ ...formData, roomId: Number(e.target.value) })
              }
              className="w-full px-3 py-2 border border-secondary-300 rounded-lg focus:ring-2 focus:ring-primary-500 dark:bg-secondary-800 dark:border-secondary-600"
            >
              <option value={0}>-- Chọn phòng trống --</option>
              {availableRooms.map((room) => {
                const price = ROOM_PRICES[room.capacity as keyof typeof ROOM_PRICES] || 0
                return (
                  <option key={room.id} value={room.id}>
                    Phòng {room.roomNumber} - {t(`contracts.roomType${room.capacity}`)} - {price.toLocaleString()} đ/tháng
                  </option>
                )
              })}
            </select>
            {formData.roomId > 0 && (
              <div className="mt-2 p-3 bg-green-50 dark:bg-green-900/20 rounded-lg">
                <div className="text-sm font-medium text-green-800 dark:text-green-200">
                  Giá phòng: {calculatedPrice.toLocaleString()} đ/tháng
                </div>
                <div className="text-xs text-green-600 dark:text-green-300 mt-1">
                  Tiền đặt cọc (1 tháng): {calculatedPrice.toLocaleString()} đ
                </div>
              </div>
            )}
            <p className="text-xs text-secondary-500 mt-1">
              {availableRooms.length} phòng trống có sẵn
            </p>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <Input
              label={t('contracts.startDate')}
              type="date"
              value={formData.startDate}
              onChange={(e) =>
                setFormData({ ...formData, startDate: e.target.value })
              }
            />
            <Input
              label={t('contracts.endDate')}
              type="date"
              value={formData.endDate}
              onChange={(e) =>
                setFormData({ ...formData, endDate: e.target.value })
              }
            />
          </div>
        </div>
      </Modal>
    </div>
  )
}
