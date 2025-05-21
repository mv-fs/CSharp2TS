// Auto-generated from FileController.cs

import { apiClient } from './apiClient';

export default {
  async getFile(): Promise<File> {
    const response = await apiClient.instance.get<File>(`api/file`, {
      responseType: 'blob',
    });
    return response.data;
  },

  async postFile(file: File): Promise<void> {
    const formData = new FormData();
    formData.append('file', file);

    await apiClient.instance.post(`api/file`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  async postFiles(files: File[]): Promise<void> {
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      const f = files[i];
      formData.append('files[' + i + ']', f);
    }

    await apiClient.instance.post(`api/file`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  async postAndReceiveFile(file: File): Promise<File> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiClient.instance.post<File>(`api/file`, formData, {
      responseType: 'blob',
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },
};
