// this line is ignored in tests

import { apiClient } from './apiClient';
import CustomType from '../types/customType';

export default {
  async get(): Promise<CustomType> {
    const response = await apiClient.instance.get<CustomType>(`api/import`);
    return response.data;
  },
};
