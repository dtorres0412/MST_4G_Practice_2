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
  onReset?: () => void;
}

interface FilterField {
  name: keyof ZipFiltersState;
  label: string;
  placeholder?: string;
}

const FILTER_FIELDS: FilterField[] = [
  { name: 'zipNo', label: 'ZIP No', placeholder: 'e.g. 10001' },
  { name: 'zipName', label: 'ZIP Name', placeholder: 'e.g. Zip1' },
  { name: 'countyNo', label: 'County No', placeholder: 'e.g. 48919245' },
  { name: 'countyName', label: 'County Name', placeholder: 'e.g. CountyA'},
  { name: 'zoNo', label: 'ZO No', placeholder: 'e.g. Z101' },
  { name: 'zoName', label: 'ZO Name', placeholder: 'e.g. Zone1'},
  { name: 'doNo', label: 'DO No', placeholder: 'e.g. D001' },
  { name: 'doName', label: 'DO Name', placeholder: 'e.g. District1' },
];

export const ZipSearchFilters = ({
  filters,
  onFilterChange,
  onQuery,
  onReset,
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
              className="w-full border p-2 rounded text-sm focus:ring-1 focus:ring-blue-500 outline-none bg-white transition"
            />
          </div>
        ))}
      </div>

      <div className="flex justify-end space-x-2 pt-2">
        {onReset && (
          <button
            type="button"
            onClick={onReset}
            className="px-4 py-2 border rounded text-sm text-gray-600 bg-white hover:bg-gray-100 font-medium transition"
          >
            Reset
          </button>
        )}
        <button
          type="submit"
          className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded text-sm font-semibold shadow transition"
        >
          Query
        </button>
      </div>
    </form>
  );
};