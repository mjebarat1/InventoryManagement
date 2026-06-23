const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim();

if (!configuredBaseUrl) {
  throw new Error('VITE_API_BASE_URL is required. Copy .env.example to configure the client.');
}

export const API_BASE_URL = configuredBaseUrl.replace(/\/$/, '');

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string,
    public readonly code?: string,
    public readonly parameters?: Record<string, unknown>
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

export async function apiRequest<T>(path: string, init?: RequestInit): Promise<T> {
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;
  const headers = new Headers(init?.headers);

  if (init?.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  const response = await fetch(`${API_BASE_URL}${normalizedPath}`, {
    ...init,
    headers,
  });

  if (!response.ok) {
    const problem = await response.json().catch(() => null) as {
      detail?: string;
      code?: string;
      parameters?: Record<string, unknown>;
    } | null;
    throw new ApiError(
      response.status,
      problem?.detail || response.statusText || 'API request failed',
      problem?.code,
      problem?.parameters
    );
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}
