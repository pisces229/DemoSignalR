const API_BASE_URL = 'http://localhost:5231'

export interface ApiResponse<T = unknown> {
    data?: T
    error?: string
    status: number
}

class ApiService {
    private baseUrl: string

    constructor(baseUrl: string = API_BASE_URL) {
        this.baseUrl = baseUrl
    }

    private async request<T>(
        endpoint: string,
        options: RequestInit = {}
    ): Promise<ApiResponse<T>> {
        try {
            const url = `${this.baseUrl}${endpoint}`
            const defaultHeaders: HeadersInit = {
                'Content-Type': 'application/json',
            }

            const response = await fetch(url, {
                ...options,
                headers: {
                    ...defaultHeaders,
                    ...options.headers,
                },
            })

            const data = response.ok ? await response.json().catch(() => ({})) : null

            return {
                data: data as T,
                status: response.status,
                error: response.ok ? undefined : `HTTP ${response.status}: ${response.statusText}`,
            }
        } catch (error) {
            return {
                status: 0,
                error: error instanceof Error ? error.message : '未知錯誤',
            }
        }
    }

    async get<T = unknown>(endpoint: string, headers?: HeadersInit): Promise<ApiResponse<T>> {
        return this.request<T>(endpoint, {
            method: 'GET',
            headers,
        })
    }

}

export default new ApiService()
