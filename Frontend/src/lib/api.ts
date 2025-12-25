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
    api.post('/api/auth/login', { username, password }),
  logout: () => {
    localStorage.removeItem('authToken')
  },
}

// Students APIs
export const studentApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/api/students', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/api/students/${id}`),
  create: (data: any) => api.post('/api/students', data),
  update: (id: string, data: any) => api.put(`/api/students/${id}`, data),
  delete: (id: string) => api.delete(`/api/students/${id}`),
}

// Rooms APIs
export const roomApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/api/rooms', { params: { pageNumber, pageSize } }),
  getAvailableRooms: (pageNumber = 1, pageSize = 10) =>
    api.get('/api/rooms/available', { params: { pageNumber, pageSize } }),
  getRoomsByBuilding: (buildingId: number, pageNumber = 1, pageSize = 100) =>
    api.get(`/api/rooms/building/${buildingId}`, { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/api/rooms/${id}`),
  create: (data: any) => api.post('/api/rooms', data),
  update: (id: string, data: any) => api.put(`/api/rooms/${id}`, data),
  delete: (id: string) => api.delete(`/api/rooms/${id}`),
}

// Buildings APIs
export const buildingApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/api/buildings', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/api/buildings/${id}`),
  create: (data: any) => api.post('/api/buildings', data),
  update: (id: string, data: any) => api.put(`/api/buildings/${id}`, data),
  delete: (id: string) => api.delete(`/api/buildings/${id}`),
}

// Floors APIs
export const floorApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/api/floors', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/api/floors/${id}`),
  create: (data: any) => api.post('/api/floors', data),
  update: (id: string, data: any) => api.put(`/api/floors/${id}`, data),
  delete: (id: string) => api.delete(`/api/floors/${id}`),
}

// Contracts APIs
export const contractApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/api/contracts', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/api/contracts/${id}`),
  create: (data: any) => api.post('/api/contracts', data),
  update: (id: string, data: any) => api.put(`/api/contracts/${id}`, data),
  delete: (id: string) => api.delete(`/api/contracts/${id}`),
}

// Invoices APIs
export const invoiceApi = {
  getAll: (pageNumber = 1, pageSize = 10) =>
    api.get('/api/invoices', { params: { pageNumber, pageSize } }),
  get: (id: string) => api.get(`/api/invoices/${id}`),
  create: (data: any) => api.post('/api/invoices', data),
  update: (id: string, data: any) => api.put(`/api/invoices/${id}`, data),
  delete: (id: string) => api.delete(`/api/invoices/${id}`),
}

export default api
