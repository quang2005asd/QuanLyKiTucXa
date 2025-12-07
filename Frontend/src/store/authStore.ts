import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import { authApi } from '../lib/api'

export type UserRole = 'Admin' | 'Manager' | 'Staff' | 'Student'

export interface User {
  id: string
  username: string
  email: string
  phoneNumber: string
  role: UserRole
  isActive: boolean
}

export interface AuthState {
  user: User | null
  token: string | null
  isLoading: boolean
  error: string | null
  login: (username: string, password: string) => Promise<void>
  logout: () => void
  setUser: (user: User) => void
  clearError: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      isLoading: false,
      error: null,
      
      login: async (username: string, password: string) => {
        set({ isLoading: true, error: null })
        try {
          const response = await authApi.login(username, password)
          const loginResponse = response.data.data
          const { user, token } = loginResponse
          
          set({
            user: user,
            token: token,
            isLoading: false,
          })
          localStorage.setItem('authToken', token)
        } catch (error: any) {
          set({
            error: error.response?.data?.message || 'Đăng nhập thất bại',
            isLoading: false,
          })
          throw error
        }
      },

      logout: () => {
        set({ user: null, token: null })
        localStorage.removeItem('authToken')
      },

      setUser: (user: User) => {
        set({ user })
      },

      clearError: () => {
        set({ error: null })
      },
    }),
    {
      name: 'auth-store',
    }
  )
)
