const config = {
  repertoireUrl: import.meta.env.VITE_REPERTOIRE_URL ?? 'http://localhost:5001',
  practiceUrl: import.meta.env.VITE_PRACTICE_URL ?? 'http://localhost:5002',
} as const

export default config
