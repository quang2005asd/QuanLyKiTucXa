import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import { ErrorBoundary } from './components/ErrorBoundary'
import './index.css'
import './i18n'

console.log('main.tsx loaded')
console.log('root element:', document.getElementById('root'))

const root = document.getElementById('root')
if (!root) {
  console.error('Root element not found!')
} else {
  console.log('Rendering app...')
  ReactDOM.createRoot(root).render(
    <React.StrictMode>
      <ErrorBoundary>
        <App />
      </ErrorBoundary>
    </React.StrictMode>,
  )
  console.log('App rendered')
}
