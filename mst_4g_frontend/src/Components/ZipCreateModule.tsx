import React, { useState, ChangeEvent, FormEvent } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config/api';

interface ZipCreateModuleProps {
  onBack: () => void;
  onSuccess: () => void;
}

interface CreateFormData {
  zipNo: string;
  zipName: string;
  effDateFrom: string;
  effDateTo: string;
  countyNo: string;
  zoNo: string;
  doNo: string;
}

interface FormFieldConfig {
  name: keyof CreateFormData;
  label: string;
  type?: string;
  placeholder?: string;
  required?: boolean;
}

const FORM_FIELDS: FormFieldConfig[] = [
  { name: 'zipNo', label: 'ZIP No', placeholder: 'e.g. 10001', required: true },
  { name: 'zipName', label: 'ZIP Name', placeholder: 'e.g. Zip1', required: true },
  { name: 'effDateFrom', label: 'Effective Date From', type: 'date', required: true },
  { name: 'effDateTo', label: 'Effective Date To', type: 'date', required: true },
  { name: 'countyNo', label: 'County No', placeholder: 'e.g. 48919245', required: true },
  { name: 'zoNo', label: 'ZO No', placeholder: 'e.g. Z101', required: true },
  { name: 'doNo', label: 'DO No', placeholder: 'e.g. D001', required: true },
];

export const ZipCreateModule = ({ onBack, onSuccess }: ZipCreateModuleProps) => {
  const [formData, setFormData] = useState<CreateFormData>({
    zipNo: '',
    zipName: '',
    effDateFrom: new Date().toISOString().slice(0, 10),
    effDateTo: '9999-12-31',
    countyNo: '',
    zoNo: '',
    doNo: '',
  });

  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);

    const payload = {
      ...formData,
      effDateFrom: `${formData.effDateFrom}T00:00:00.000Z`,
      effDateTo: `${formData.effDateTo}T00:00:00.000Z`,
    };

    try {
      await axios.post(`${API_BASE_URL}/create`, payload);
      onSuccess();
    } catch (err: any) {
      const serverMessage = err.response?.data?.message || err.response?.data;
      setError(
        typeof serverMessage === 'string'
          ? serverMessage
          : 'Failed to create new ZIP territory record.'
      );
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow-sm space-y-6">
      <div className="flex justify-between items-center border-b pb-4">
        <div>
          <button
            type="button"
            onClick={onBack}
            disabled={submitting}
            className="text-sm text-blue-600 hover:underline mb-1 inline-block font-medium disabled:opacity-50"
          >
            ← Back to Zip Maintenance
          </button>
          <h2 className="text-xl font-bold text-gray-800">Add New ZIP Territory</h2>
        </div>
      </div>

      {error && (
        <div className="p-3 bg-red-50 border border-red-200 text-red-600 rounded text-xs">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {FORM_FIELDS.map(({ name, label, type, placeholder, required }) => (
          <div key={name}>
            <label className="block text-xs font-semibold text-gray-600 mb-1">
              {label} {required && <span className="text-red-500">*</span>}
            </label>
            <input
              type={type || 'text'}
              name={name}
              value={formData[name]}
              onChange={handleChange}
              placeholder={placeholder}
              disabled={submitting}
              required={required}
              autoComplete="off"
              className="w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 outline-none transition bg-white"
            />
          </div>
        ))}

        <div className="md:col-span-2 flex justify-end space-x-2 pt-4 border-t">
          <button
            type="button"
            onClick={onBack}
            disabled={submitting}
            className="px-4 py-2 border rounded text-sm hover:bg-gray-50 disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={submitting}
            className="px-4 py-2 bg-blue-600 text-white rounded text-sm font-semibold hover:bg-blue-700 disabled:opacity-50 transition"
          >
            {submitting ? 'Creating...' : 'Create Record'}
          </button>
        </div>
      </form>
    </div>
  );
};