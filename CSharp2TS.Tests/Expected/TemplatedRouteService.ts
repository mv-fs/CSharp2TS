// Auto-generated from TemplatedRouteController.cs

import { apiClient } from './apiClient';

export default {
  async get(): Promise<string> {
    const response = await apiClient.instance.get<string>(`api/templatedroute`);
    return response.data;
  },
};
