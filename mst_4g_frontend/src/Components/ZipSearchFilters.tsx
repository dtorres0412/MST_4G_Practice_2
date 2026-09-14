import React from 'react';

export interface ZipFiltersState {
  zipNo: string;
  zipName: string;
  countyNo: string;
  countyName: string;
  zoNo: string;
  zoName: string;
  doNo: string;
  doName: string;
}

interface ZipSearchFiltersProps {
  filters: ZipFiltersState;
  onFilterChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  onQuery: () => void;
}

const FILTER_FIELDS: Array<{
  name: keyof ZipFiltersState;
  label: string;
  placeholder?: string;
}> = [
  { name: 'zipNo', label: 'ZIP No', placeholder: 'e.g. 10002' },
  { name: 'zipName', label: 'ZIP Name', placeholder: 'e.g. Manila' },
  { name: 'countyNo', label: 'County No' },
  { name: 'countyName', label: 'County Name' },
  { name: 'zoNo', label: 'ZO No', placeholder: 'e.g. Z101' },
  { name: 'zoName', label: 'ZO Name' },
  { name: 'doNo', label: 'DO No', placeholder: 'e.g. D001' },
  { name: 'doName', label: 'DO Name' },
];

export const ZipSearchFilters = ({
  filters,
  onFilterChange,
  onQuery,
}: ZipSearchFiltersProps) => {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onQuery();
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="bg-gray-50 p-4 border rounded-lg shadow-sm space-y-4"
    >
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {FILTER_FIELDS.map(({ name, label, placeholder }) => (
          <div key={name}>
            <label className="block text-xs font-semibold text-gray-600 mb-1">
              {label}
            </label>
            <input
              name={name}
              value={filters[name]}
              onChange={onFilterChange}
              placeholder={placeholder}
              className="w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 focus:outline-none bg-white transition"
            />
          </div>
        ))}
      </div>

      <div className="flex justify-end pt-2">
        <button
          type="submit"
          className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded shadow font-semibold transition"
        >
          Query
        </button>
      </div>
    </form>
  );
};