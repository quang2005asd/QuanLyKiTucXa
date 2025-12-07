import { create } from 'zustand'

export interface Student {
  id: string
  fullName: string
  email: string
  phoneNumber: string
  studentId: string
  status: 'Active' | 'Inactive'
  roomId?: string
  createdAt: string
}

export interface Room {
  id: string
  roomNumber: string
  floorId: string
  capacity: number
  occupiedBeds: number
  price: number
  status: 'Available' | 'Full' | 'Maintenance'
}

export interface Building {
  id: string
  name: string
  address: string
  floorCount: number
  totalRooms: number
}

export interface Contract {
  id: string
  studentId: string
  roomId: string
  startDate: string
  endDate: string
  status: 'Active' | 'Expired' | 'Cancelled'
  monthlyPrice: number
}

export interface Invoice {
  id: string
  contractId: string
  month: number
  year: number
  amount: number
  status: 'Pending' | 'Paid' | 'Overdue'
  dueDate: string
}

export interface AppState {
  students: Student[]
  rooms: Room[]
  buildings: Building[]
  contracts: Contract[]
  invoices: Invoice[]
  
  setStudents: (students: Student[]) => void
  setRooms: (rooms: Room[]) => void
  setBuildings: (buildings: Building[]) => void
  setContracts: (contracts: Contract[]) => void
  setInvoices: (invoices: Invoice[]) => void
}

export const useAppStore = create<AppState>((set) => ({
  students: [],
  rooms: [],
  buildings: [],
  contracts: [],
  invoices: [],

  setStudents: (students) => set({ students }),
  setRooms: (rooms) => set({ rooms }),
  setBuildings: (buildings) => set({ buildings }),
  setContracts: (contracts) => set({ contracts }),
  setInvoices: (invoices) => set({ invoices }),
}))
