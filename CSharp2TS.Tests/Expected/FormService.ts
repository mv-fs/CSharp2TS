// Auto-generated from FormController.cs

import { apiClient, FormDataFactory } from './apiClient';
import TestClass from '../Models/TestClass';

export default {
  async postForm(form: FormData): Promise<void> {
    await apiClient.instance.post(`api/form`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  async postForm2(obj: TestClass): Promise<void> {
    const formData = FormDataFactory.Create(obj);

    await apiClient.instance.post(`api/form`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  async postForm3(str: string): Promise<void> {
    await apiClient.instance.post(`api/form`, str, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};
