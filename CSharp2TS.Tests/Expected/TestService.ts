// this line is ignored in tests

import { apiClient } from './apiClient';
import TestClass from '../Models/TestClass';
import TestClass2 from '../Models/TestClass2';
import GenericClass1 from '../Models/GenericClass1';

export default {
  async get(): Promise<string> {
    const response = await apiClient.instance.get<string>(`api/Test`);
    return response.data;
  },

  async actionResultGet(): Promise<string> {
    const response = await apiClient.instance.get<string>(`api/Test`);
    return response.data;
  },

  async actionResultGetAsync(): Promise<string> {
    const response = await apiClient.instance.get<string>(`api/Test`);
    return response.data;
  },

  async tSEndpointOverride(): Promise<string> {
    const response = await apiClient.instance.get<string>(`api/Test`);
    return response.data;
  },

  async get2(id: number): Promise<TestClass> {
    const response = await apiClient.instance.get<TestClass>(`api/Test/${id}`);
    return response.data;
  },

  async get3(id: number, externalId: number): Promise<TestClass> {
    const response = await apiClient.instance.get<TestClass>(`api/Test/${id}?externalId=${externalId}`);
    return response.data;
  },

  async generic(): Promise<GenericClass1<TestClass2>> {
    const response = await apiClient.instance.get<GenericClass1<TestClass2>>(`api/Test`);
    return response.data;
  },

  async getFiltered(filter: string, limit: number): Promise<TestClass[]> {
    const response = await apiClient.instance.get<TestClass[]>(`api/Test/filtered?filter=${filter ?? ''}&limit=${limit}`);
    return response.data;
  },

  async create(testClass: TestClass): Promise<TestClass> {
    const response = await apiClient.instance.post<TestClass>(`api/Test`, testClass);
    return response.data;
  },

  async createFromBody(model: string): Promise<string> {
    const response = await apiClient.instance.post<string>(`api/Test`, model);
    return response.data;
  },

  async update(id: number, testClass: TestClass): Promise<TestClass> {
    const response = await apiClient.instance.put<TestClass>(`api/Test/${id}`, testClass);
    return response.data;
  },

  async partialUpdate(id: number, model: TestClass): Promise<TestClass> {
    const response = await apiClient.instance.patch<TestClass>(`api/Test/${id}`, model);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await apiClient.instance.delete(`api/Test/${id}`);
  },

  async getWithTypedParam(id: number): Promise<void> {
    await apiClient.instance.get(`api/Test/${id}`);
  },
};
