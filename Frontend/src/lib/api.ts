import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5263'

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Handle response errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Auth APIs
export const authApi = {
  login: (username: string, password: string) =>
    api.post('/auth/login', { username, password }),
  logout: () => {
    localStorage.removeItem('authToken')
  },
}

// Students APIs
export const studentApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/students', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/students/${id}`),
  create: (data: any) => api.post('/students', data),
  update: (id: string, data: any) => api.put(`/students/${id}`, data),
  delete: (id: string) => api.delete(`/students/${id}`),
}

// Rooms APIs
export const roomApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/rooms', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/rooms/${id}`),
  create: (data: any) => api.post('/rooms', data),
  update: (id: string, data: any) => api.put(`/rooms/${id}`, data),
  delete: (id: string) => api.delete(`/rooms/${id}`),
}

// Buildings APIs
export const buildingApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/buildings', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/buildings/${id}`),
  create: (data: any) => api.post('/buildings', data),
  update: (id: string, data: any) => api.put(`/buildings/${id}`, data),
  delete: (id: string) => api.delete(`/buildings/${id}`),
}

// Contracts APIs
export const contractApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/contracts', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/contracts/${id}`),
  create: (data: any) => api.post('/contracts', data),
  update: (id: string, data: any) => api.put(`/contracts/${id}`, data),
  delete: (id: string) => api.delete(`/contracts/${id}`),
}

// Invoices APIs
export const invoiceApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/invoices', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/invoices/${id}`),
  create: (data: any) => api.post('/invoices', data),
  update: (id: string, data: any) => api.put(`/invoices/${id}`, data),
  delete: (id: string) => api.delete(`/invoices/${id}`),
}

export default api
