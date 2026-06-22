import { apiRequest } from './http-client';

export type PingResponse = {
  status: string;
};

export function pingApi(signal?: AbortSignal) {
  return apiRequest<PingResponse>('/api/ping', { signal });
}
