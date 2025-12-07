import { useTranslation } from 'react-i18next'
import { Button, Card, Input, Modal, Table } from '../components'
import { Plus, Edit2, Trash2 } from 'lucide-react'
import { useEffect, useState } from 'react'
import { studentApi } from '../lib/api'

export default function StudentsPage() {
  const { t } = useTranslation()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [students, setStudents] = useState([])
  const [isLoading, setIsLoading] = useState(false)
  const [pageNumber, setPageNumber] = useState(1)
  const [formData, setFormData] = useState({ fullName: '', email: '', phoneNumber: '', studentId: '' })

  useEffect(() => {
    loadStudents()
  }, [pageNumber])

  const loadStudents = async () => {
    try {
      setIsLoading(true)
      const response = await studentApi.getAll(pageNumber, 10)
      setStudents(response.data.data || [])
    } catch (error) {
      console.error('Error loading students:', error)
    } finally {
      setIsLoading(false)
    }
  }

  const handleAddStudent = async () => {
    try {
      await studentApi.create(formData)
      setFormData({ fullName: '', email: '', phoneNumber: '', studentId: '' })
      setIsModalOpen(false)
      loadStudents()
    } catch (error) {
      console.error('Error adding student:', error)
    }
  }

  const handleDeleteStudent = async (id: string) => {
    if (window.confirm('Bạn chắc chắn muốn xóa sinh viên này?')) {
      try {
        await studentApi.delete(id)
        loadStudents()
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
          onClick={() => setIsModalOpen(true)}
        >
          <Plus className="w-4 h-4" />
          {t('students.addStudent')}
        </Button>
      </div>

      {/* Filters */}
      <Card>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Input placeholder={t('common.search')} />
          <Input placeholder="Trạng thái" />
          <div />
        </div>
      </Card>

      {/* Table */}
      <Card>
        <Table
          columns={[
            { key: 'fullName', label: t('students.fullName') },
            { key: 'email', label: t('students.email') },
            { key: 'phoneNumber', label: t('students.phoneNumber') },
            { key: 'studentId', label: t('students.studentId') },
            {
              key: 'status',
              label: t('common.status'),
              render: (value) => (
                <span className={`px-3 py-1 rounded-full text-xs font-medium ${
                  value === 'Active'
                    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-200'
                    : 'bg-secondary-100 text-secondary-800 dark:bg-secondary-700 dark:text-secondary-200'
                }`}>
                  {value === 'Active' ? 'Hoạt động' : 'Không hoạt động'}
                </span>
              ),
            },
            {
              key: 'actions',
              label: t('common.actions'),
              render: (_, row) => (
                <div className="flex gap-2">
                  <button className="text-primary-600 hover:text-primary-700">
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
          data={students}
          isLoading={isLoading}
        />
      </Card>

      {/* Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title={t('students.addStudent')}
        actions={
          <>
            <Button variant="secondary" onClick={() => setIsModalOpen(false)}>
              {t('common.cancel')}
            </Button>
            <Button variant="primary" onClick={handleAddStudent}>
              {t('common.save')}
            </Button>
          </>
        }
      >
        <div className="space-y-4">
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
          <Input
            label={t('students.studentId')}
            value={formData.studentId}
            onChange={(e) => setFormData({ ...formData, studentId: e.target.value })}
          />
        </div>
      </Modal>
    </div>
  )
}
