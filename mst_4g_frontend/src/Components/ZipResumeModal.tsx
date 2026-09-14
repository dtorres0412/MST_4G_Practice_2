import React, { useState } from 'react';
import axios from 'axios';
import { ZipRecord } from './ZipDataTable';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7043/api/Zip';

interface ZipResumeModalProps {
  selectedZipRecord: ZipRecord;
  zipHistoryList: ZipRecord[];
  onClose: () => void;
  onRefresh: () => void;
}

export const ZipResumeModal = ({
  selectedZipRecord,
  zipHistoryList,
  onClose,
  onRefresh,
}: ZipResumeModalProps) => {
  const [formData, setFormData] = useState({
    zipNo: selectedZipRecord.zipNo ?? '',
    zipName: selectedZipRecord.zipName ?? '',
    effDateFrom: new Date().toISOString().substring(0, 10),
    countyNo: selectedZipRecord.countyNo ?? '',
    zoNo: '',
    doNo: '',
  });

  const [submitting, setSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setErrorMessage(null);

    const payload = {
      oriItem: selectedZipRecord,
      newItem: {
        zipNo: formData.zipNo,
        zipName: formData.zipName,
        effDateFrom: new Date(formData.effDateFrom).toISOString(),
        effDateTo: '9999-12-31T00:00:00.000Z',
        countyNo: formData.countyNo,
        zoNo: formData.zoNo,
        doNo: formData.doNo,
      },
      vitaeList: zipHistoryList,
    };

    try {
      await axios.post(`${API_BASE_URL}/ProcessResume`, payload);
      onRefresh();
    } catch (error) {
      setErrorMessage('Failed to process territory update. Please try again.');
      console.error('Process resume error:', error);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white p-6 rounded-lg shadow-xl w-full max-w-lg space-y-4">
        <div className="flex justify-between items-center border-b pb-2">
          <h2 className="text-lg font-bold text-gray-800">Process Resume Territory</h2>
          <button
            onClick={onClose}
            disabled={submitting}
            className="text-gray-400 hover:text-gray-600 font-bold p-1 disabled:opacity-50"
          >
            ✕
          </button>
        </div>

        {errorMessage && (
          <div className="p-3 bg-red-50 border border-red-200 text-red-600 rounded text-xs">
            {errorMessage}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-3">
          <div>
            <label className="block text-xs font-semibold text-gray-600 mb-1">ZIP No</label>
            <input
              name="zipNo"
              value={formData.zipNo}
              disabled
              className="w-full border p-2 rounded text-sm bg-gray-100 text-gray-500 cursor-not-allowed"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-gray-600 mb-1">ZIP Name</label>
            <input
              name="zipName"
              value={formData.zipName}
              onChange={handleChange}
              required
              disabled={submitting}
              className="w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 outline-none"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-gray-600 mb-1">Effective Date From</label>
            <input
              type="date"
              name="effDateFrom"
              value={formData.effDateFrom}
              onChange={handleChange}
              required
              disabled={submitting}
              className="w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 outline-none"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-gray-600 mb-1">County No</label>
            <input
              name="countyNo"
              value={formData.countyNo}
              onChange={handleChange}
              required
              disabled={submitting}
              className="w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 outline-none"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-gray-600 mb-1">ZO No</label>
            <input
              name="zoNo"
              value={formData.zoNo}
              onChange={handleChange}
              required
              disabled={submitting}
              placeholder="e.g. Z102"
              className="w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 outline-none"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-gray-600 mb-1">DO No</label>
            <input
              name="doNo"
              value={formData.doNo}
              onChange={handleChange}
              required
              disabled={submitting}
              placeholder="e.g. D002"
              className="w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 outline-none"
            />
          </div>

          <div className="flex justify-end space-x-2 pt-4 border-t">
            <button
              type="button"
              onClick={onClose}
              disabled={submitting}
              className="px-4 py-2 border rounded text-sm hover:bg-gray-50 disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={submitting}
              className="px-4 py-2 bg-blue-600 text-white rounded text-sm font-semibold hover:bg-blue-700 disabled:opacity-50"
            >
              {submitting ? 'Processing...' : 'Submit Resume'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};