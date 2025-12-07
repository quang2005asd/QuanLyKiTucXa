import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate } from 'react-router-dom'
import { Button, Input, Card, Alert } from '../components'
import { useAuthStore } from '../store/authStore'

export default function LoginPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const [username, setUsername] = useState('admin')
  const [password, setPassword] = useState('admin')
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const { login } = useAuthStore()

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')
    setIsLoading(true)

    try {
      await login(username, password)
      navigate('/dashboard')
    } catch (err: any) {
      setError(err.response?.data?.message || 'Đăng nhập thất bại. Kiểm tra lại tên đăng nhập và mật khẩu.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary-500 via-primary-600 to-primary-700 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo */}
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-white mb-2">{t('auth.title')}</h1>
          <p className="text-primary-100">{t('auth.loginTitle')}</p>
        </div>

        {/* Form Card */}
        <Card className="bg-white dark:bg-secondary-800 shadow-2xl">
          {error && (
            <Alert type="error" className="mb-4">
              {error}
            </Alert>
          )}

          <form onSubmit={handleLogin} className="space-y-4">
            <Input
              label={t('auth.username')}
              type="text"
              placeholder={t('auth.username')}
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />

            <Input
              label={t('auth.password')}
              type="password"
              placeholder={t('auth.password')}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />

            <div className="flex items-center justify-between text-sm">
              <label className="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" className="rounded" />
                <span>{t('auth.rememberMe')}</span>
              </label>
              <a href="#" className="text-primary-600 hover:text-primary-700 font-medium">
                {t('auth.forgotPassword')}
              </a>
            </div>

            <Button type="submit" variant="primary" size="lg" className="w-full" isLoading={isLoading}>
              {t('auth.signIn')}
            </Button>
          </form>

          <div className="mt-6 pt-6 border-t border-secondary-200 dark:border-secondary-700 text-center text-sm text-secondary-600 dark:text-secondary-400">
            <p className="mb-2">Demo credentials:</p>
            <p>Username: <span className="font-mono bg-secondary-100 dark:bg-secondary-700 px-2 py-1 rounded">admin</span></p>
            <p>Password: <span className="font-mono bg-secondary-100 dark:bg-secondary-700 px-2 py-1 rounded">admin</span></p>
          </div>
        </Card>

        {/* Footer */}
        <p className="text-center text-primary-100 text-sm mt-6">
          © 2025 Quản Lý Ký Túc Xá. Tất cả các quyền được bảo lưu.
        </p>
      </div>
    </div>
  )
}
