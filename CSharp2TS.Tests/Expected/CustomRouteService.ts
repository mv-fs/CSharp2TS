// Auto-generated from CustomRouteController.cs

import { apiClient } from './apiClient';

export default {
  async get(): Promise<string> {
    const response = await apiClient.instance.get<string>(`api/custom-route`);
    return response.data;
  },
};
