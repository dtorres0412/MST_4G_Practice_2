import React from 'react';

export interface ZipRecord {
  zipId: number;
  zipNo: string;
  zipName: string;
  countyNo: string;
  countyName?: string;
  zoNo: string;
  zoName?: string;
  doNo: string;
  doName?: string;
  effDateFrom: string;
  effDateTo: string;
}

interface ZipDataTableProps {
  tableData: ZipRecord[];
  loading: boolean;
  onEdit: (row: ZipRecord) => void;
}

const formatDate = (dateStr?: string) => {
  if (!dateStr) return '-';
  const parsed = new Date(dateStr);
  return isNaN(parsed.getTime()) ? '-' : parsed.toLocaleDateString();
};

const COLUMNS: Array<{
  key: string;
  label: string;
  render?: (row: ZipRecord) => React.ReactNode;
}> = [
  { key: 'zipNo', label: 'Zip No', render: (row) => <span className="font-medium">{row.zipNo}</span> },
  { key: 'zipName', label: 'Zip Name', render: (row) => row.zipName },
  { key: 'countyNo', label: 'County No', render: (row) => row.countyNo },
  { key: 'countyName', label: 'County Name', render: (row) => row.countyName ?? '-' },
  { key: 'zoNo', label: 'ZO No', render: (row) => row.zoNo },
  { key: 'zoName', label: 'ZO Name', render: (row) => row.zoName ?? '-' },
  { key: 'doNo', label: 'DO No', render: (row) => row.doNo },
  { key: 'doName', label: 'DO Name', render: (row) => row.doName ?? '-' },
  { key: 'effDateFrom', label: 'Eff Date From', render: (row) => formatDate(row.effDateFrom) },
  { key: 'effDateTo', label: 'Eff Date To', render: (row) => formatDate(row.effDateTo) },
];

export const ZipDataTable = ({ tableData, loading, onEdit }: ZipDataTableProps) => {
  const totalColumns = COLUMNS.length + 1; // Dynamic column count (+1 para sa Action column)

  return (
    <div className="overflow-x-auto border rounded-lg shadow-sm bg-white">
      <table id="table" className="min-w-full text-left text-sm border-collapse border border-gray-300">
        <thead className="bg-gray-100 text-gray-700 font-bold border-bold">
          <tr>
            {COLUMNS.map((col) => (
              <th key={col.key} className="p-3 border-r whitespace-nowrap">
                {col.label}
              </th>
            ))}
            <th className="p-3 text-center whitespace-nowrap">Action</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {loading ? (
            <tr>
              <td colSpan={totalColumns} className="p-6 text-center text-gray-500">
                Loading records...
              </td>
            </tr>
          ) : tableData.length === 0 ? (
            <tr>
              <td colSpan={totalColumns} className="p-6 text-center text-gray-500">
                No data found.
              </td>
            </tr>
          ) : (
            tableData.map((row) => (
              <tr key={row.zipId} className="hover:bg-gray-50 transition">
                {COLUMNS.map((col) => (
                  <td key={col.key} className="p-3 border-r">
                    {col.render ? col.render(row) : (row as any)[col.key] ?? '-'}
                  </td>
                ))}
                <td className="p-3 text-center">
                  <button
                    onClick={() => onEdit(row)}
                    className="bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded text-xs font-semibold transition"
                  >
                    Edit
                  </button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
};