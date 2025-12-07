import { useState, useCallback } from 'react'

export interface CrudState<T> {
  data: T[]
  isLoading: boolean
  error: string | null
  totalCount: number
  pageNumber: number
  pageSize: number
}

export function useCrud<T>(api: any, initialPageSize: number = 10) {
  const [state, setState] = useState<CrudState<T>>({
    data: [],
    isLoading: false,
    error: null,
    totalCount: 0,
    pageNumber: 1,
    pageSize: initialPageSize,
  })

  const loadData = useCallback(
    async (pageNumber = 1, pageSize = initialPageSize) => {
      setState((prev) => ({ ...prev, isLoading: true, error: null }))
      try {
        const response = await api.getAll(pageNumber, pageSize)
        setState((prev) => ({
          ...prev,
          data: response.data.data || [],
          totalCount: response.data.pagination?.totalCount || 0,
          pageNumber,
          pageSize,
          isLoading: false,
        }))
      } catch (error: any) {
        setState((prev) => ({
          ...prev,
          error: error.response?.data?.message || 'Lỗi khi tải dữ liệu',
          isLoading: false,
        }))
      }
    },
    [api, initialPageSize]
  )

  const create = useCallback(
    async (data: any) => {
      try {
        const response = await api.create(data)
        await loadData(state.pageNumber, state.pageSize)
        return response
      } catch (error: any) {
        const message = error.response?.data?.message || 'Lỗi khi tạo'
        setState((prev) => ({ ...prev, error: message }))
        throw error
      }
    },
    [api, loadData, state.pageNumber, state.pageSize]
  )

  const update = useCallback(
    async (id: string, data: any) => {
      try {
        const response = await api.update(id, data)
        await loadData(state.pageNumber, state.pageSize)
        return response
      } catch (error: any) {
        const message = error.response?.data?.message || 'Lỗi khi cập nhật'
        setState((prev) => ({ ...prev, error: message }))
        throw error
      }
    },
    [api, loadData, state.pageNumber, state.pageSize]
  )

  const delete_ = useCallback(
    async (id: string) => {
      try {
        const response = await api.delete(id)
        await loadData(state.pageNumber, state.pageSize)
        return response
      } catch (error: any) {
        const message = error.response?.data?.message || 'Lỗi khi xóa'
        setState((prev) => ({ ...prev, error: message }))
        throw error
      }
    },
    [api, loadData, state.pageNumber, state.pageSize]
  )

  const changePage = useCallback(
    async (pageNumber: number) => {
      await loadData(pageNumber, state.pageSize)
    },
    [loadData, state.pageSize]
  )

  const clearError = useCallback(() => {
    setState((prev) => ({ ...prev, error: null }))
  }, [])

  return {
    ...state,
    loadData,
    create,
    update,
    delete: delete_,
    changePage,
    clearError,
  }
}
