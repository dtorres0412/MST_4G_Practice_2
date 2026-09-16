import { useState, ChangeEvent, FormEvent } from 'react';
import axios from 'axios';
import { ZipRecord } from './ZipDataTable';
import { API_BASE_URL } from '../config/api';

interface ZipEditModuleProps {
  record: ZipRecord;
  onBack: () => void;
  onSaveSuccess: () => void;
}

interface EditableFields {
  zipNo: string;
  zipName: string;
  countyNo: string;
  zoNo: string;
  doNo: string;
  effDateFrom: string;
  effDateTo: string;
}

interface FormField {
  name: keyof EditableFields;
  label: string;
  type?: string;
  disabled?: boolean;
}

const FORM_FIELDS: FormField[] = [
  { name: 'zipNo', label: 'ZIP No', disabled: true },
  { name: 'zipName', label: 'ZIP Name' },
  { name: 'effDateFrom', label: 'Effective Date From', type: 'date' },
  { name: 'effDateTo', label: 'Effective Date To', type: 'date' },
  { name: 'countyNo', label: 'County No' },
  { name: 'zoNo', label: 'ZO No' },
  { name: 'doNo', label: 'DO No' },
];

const formatDateForInput = (dateStr?: string) => (dateStr ? dateStr.split('T')[0] : '');

export const ZipEditModule = ({ record, onBack, onSaveSuccess }: ZipEditModuleProps) => {
  const [formData, setFormData] = useState<EditableFields>({
    zipNo: record.zipNo,
    zipName: record.zipName,
    effDateFrom: formatDateForInput(record.effDateFrom),
    effDateTo: formatDateForInput(record.effDateTo),
    countyNo: record.countyNo,
    zoNo: record.zoNo,
    doNo: record.doNo,
  });

  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError(null);

    const payload = {
      ...record,
      ...formData,
      effDateFrom: formData.effDateFrom ? `${formData.effDateFrom}T00:00:00.000Z` : record.effDateFrom,
      effDateTo: formData.effDateTo ? `${formData.effDateTo}T00:00:00.000Z` : record.effDateTo,
    };

    try {
      await axios.put(`${API_BASE_URL}/update`, payload);
      onSaveSuccess();
    } catch (err: any) {
      const serverMessage = err.response?.data?.message || err.response?.data;
      setError(typeof serverMessage === 'string' ? serverMessage : 'Failed to update ZIP territory details.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    const confirmed = window.confirm(
      `Are you sure you want to delete ZIP Territory #${record.zipNo}?`
    );

    if (!confirmed) return;

    setDeleting(true);
    setError(null);

    try {
      await axios.delete(`${API_BASE_URL}/${record.zipId}`);
      onSaveSuccess();
    } catch (err: any) {
      const serverMessage = err.response?.data?.message || err.response?.data;
      setError(
        typeof serverMessage === 'string'
          ? serverMessage
          : 'Failed to delete ZIP territory record.'
      );
    } finally {
      setDeleting(false);
    }
  };

  const isProcessing = saving || deleting;

  return (
    <div className="bg-white p-6 rounded-lg shadow-sm space-y-6">
      <div className="flex justify-between items-center border-b pb-4">
        <div>
          <button
            type="button"
            onClick={onBack}
            disabled={isProcessing}
            className="text-sm text-blue-600 hover:underline mb-1 inline-block font-medium disabled:opacity-50"
          >
            ← Back to Zip Maintenance
          </button>
          <h2 className="text-xl font-bold text-gray-800">
            Edit ZIP Territory #{formData.zipNo}
          </h2>
        </div>
      </div>

      {error && (
        <div className="p-3 bg-red-50 border border-red-200 text-red-600 rounded text-xs">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {FORM_FIELDS.map(({ name, label, type, disabled }) => (
          <div key={name}>
            <label className="block text-xs font-semibold text-gray-600 mb-1">{label}</label>
            <input
              type={type || 'text'}
              name={name}
              value={formData[name]}
              onChange={handleChange}
              disabled={disabled || isProcessing}
              required={!disabled}
              className={`w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 outline-none transition ${
                disabled ? 'bg-gray-100 text-gray-500 cursor-not-allowed' : 'bg-white'
              }`}
            />
          </div>
        ))}

        <div className="md:col-span-2 flex justify-between items-center pt-4 border-t">
          <button
            type="button"
            onClick={handleDelete}
            disabled={isProcessing}
            className="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded text-sm font-semibold disabled:opacity-50 transition"
          >
            {deleting ? 'Deleting...' : 'Delete Territory'}
          </button>

          <div className="flex space-x-2">
            <button
              type="button"
              onClick={onBack}
              disabled={isProcessing}
              className="px-4 py-2 border rounded text-sm hover:bg-gray-50 disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isProcessing}
              className="px-4 py-2 bg-blue-600 text-white rounded text-sm font-semibold hover:bg-blue-700 disabled:opacity-50 transition"
            >
              {saving ? 'Saving...' : 'Save Changes'}
            </button>
          </div>
        </div>
      </form>
    </div>
  );
};