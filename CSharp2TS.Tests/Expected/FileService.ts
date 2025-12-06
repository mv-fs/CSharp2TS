// Auto-generated from FileController.cs

import { apiClient } from './apiClient';
import type { AxiosProgressEvent } from 'axios';

export default {
  async getFile(): Promise<File> {
    const response = await apiClient.instance.get<File>(`api/file`, {
      responseType: 'blob',
    });
    return response.data;
  },

  async postFile(file: File, onUploadProgress?: (event: AxiosProgressEvent) => void): Promise<void> {
    const formData = new FormData();
    formData.append('file', file);

    await apiClient.instance.post(`api/file`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
      onUploadProgress,
    });
  },

  async postFiles(files: File[], onUploadProgress?: (event: AxiosProgressEvent) => void): Promise<void> {
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      const f = files[i];
      formData.append('files[' + i + ']', f);
    }

    await apiClient.instance.post(`api/file`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
      onUploadProgress,
    });
  },

  async postAndReceiveFile(file: File, onUploadProgress?: (event: AxiosProgressEvent) => void): Promise<File> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiClient.instance.post<File>(`api/file`, formData, {
      responseType: 'blob',
      headers: { 'Content-Type': 'multipart/form-data' },
      onUploadProgress,
    });
    return response.data;
  },

  async emptyPostAndReceiveFile(): Promise<File> {
    const response = await apiClient.instance.post<File>(`api/file`, undefined, {
      responseType: 'blob',
    });
    return response.data;
  },
};
