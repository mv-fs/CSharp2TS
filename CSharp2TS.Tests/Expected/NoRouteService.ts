// Auto-generated from NoRouteController.cs

import { apiClient } from './apiClient';

export default {
  async get(): Promise<string> {
    const response = await apiClient.instance.get<string>(`noroute`);
    return response.data;
  },
};
