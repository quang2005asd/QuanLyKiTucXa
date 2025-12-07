import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table, Alert } from '../components'
import { Plus, Edit2, Trash2 } from 'lucide-react'
import { useEffect, useState } from 'react'
import { studentApi } from '../lib/api'
import { useCrud } from '../hooks/useCrud'

interface StudentForm {
  fullName: string
  email: string
  phoneNumber: string
  studentCode: string
}

export default function StudentsPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [formData, setFormData] = useState<StudentForm>({
    fullName: '',
    email: '',
    phoneNumber: '',
    studentCode: '',
  })
  
  const crud = useCrud(studentApi, 10)

  useEffect(() => {
    crud.loadData(1, 10)
  }, [])

  const handleOpenModal = (student?: any) => {
    if (student) {
      setEditingId(student.id)
      setFormData({
        fullName: student.fullName,
        email: student.email,
        phoneNumber: student.phoneNumber,
        studentCode: student.studentCode,
      })
    } else {
      setEditingId(null)
      setFormData({ fullName: '', email: '', phoneNumber: '', studentCode: '' })
    }
    setIsModalOpen(true)
  }

  const handleSaveStudent = async () => {
    if (!formData.fullName || !formData.email || !formData.studentCode) {
      alert('Vui lòng điền đầy đủ thông tin')
      return
    }

    try {
      if (editingId) {
        await crud.update(editingId, formData)
      } else {
        await crud.create(formData)
      }
      setIsModalOpen(false)
      setFormData({ fullName: '', email: '', phoneNumber: '', studentCode: '' })
    } catch (error) {
      console.error('Error saving student:', error)
    }
  }

  const handleDeleteStudent = async (id: string) => {
    if (window.confirm('Bạn chắc chắn muốn xóa sinh viên này?')) {
      try {
        await crud.delete(id)
      } catch (error) {
        console.error('Error deleting student:', error)
      }
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-secondary-900 dark:text-white">{t('students.title')}</h1>
          <p className="text-secondary-600 dark:text-secondary-400 mt-2">Quản lý danh sách sinh viên</p>
        </div>
        <Button
          variant="primary"
          className="gap-2"
          onClick={() => handleOpenModal()}
        >
          <Plus className="w-4 h-4" />
          {t('students.addStudent')}
        </Button>
      </div>

      {/* Error Alert */}
      {crud.error && (
        <Alert type="error" className="mb-4">
          {crud.error}
          <button
            onClick={crud.clearError}
            className="ml-2 font-semibold hover:underline"
          >
            ✕
          </button>
        </Alert>
      )}

      {/* Table */}
      <Card>
        <Table
          columns={[
            { key: 'studentCode', label: 'Mã SV' },
            { key: 'fullName', label: t('students.fullName') },
            { key: 'email', label: t('students.email') },
            { key: 'phoneNumber', label: t('students.phoneNumber') },
            {
              key: 'actions',
              label: t('common.actions'),
              render: (_, row) => (
                <div className="flex gap-2">
                  <button
                    onClick={() => handleOpenModal(row)}
                    className="text-primary-600 hover:text-primary-700"
                  >
                    <Edit2 className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleDeleteStudent(row.id)}
                    className="text-danger hover:text-red-700"
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
        title={editingId ? 'Chỉnh sửa sinh viên' : t('students.addStudent')}
        actions={
          <>
            <Button variant="secondary" onClick={() => setIsModalOpen(false)}>
              {t('common.cancel')}
            </Button>
            <Button variant="primary" onClick={handleSaveStudent}>
              {t('common.save')}
            </Button>
          </>
        }
      >
        <div className="space-y-4">
          <Input
            label="Mã Sinh Viên *"
            value={formData.studentCode}
            onChange={(e) => setFormData({ ...formData, studentCode: e.target.value })}
            disabled={!!editingId}
          />
          <Input
            label={t('students.fullName')}
            value={formData.fullName}
            onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
          />
          <Input
            label={t('students.email')}
            type="email"
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
          />
          <Input
            label={t('students.phoneNumber')}
            value={formData.phoneNumber}
            onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })}
          />
        </div>
      </Modal>
    </div>
  )
}
