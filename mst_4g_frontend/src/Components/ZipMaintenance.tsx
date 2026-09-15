import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import { ZipSearchFilters, ZipFiltersState } from './ZipSearchFilters';
import { ZipDataTable, ZipRecord } from './ZipDataTable';
import { ZipPagination } from './ZipPagination';
import { ZipEditModule } from './ZipEditModule';
import { ZipCreateModule } from './ZipCreateModule';
import { API_BASE_URL } from '../config/api';

interface SearchApiResponse {
  items: ZipRecord[];
  totalCount: number;
}

const INITIAL_FILTERS: ZipFiltersState = {
  zipNo: '',
  zipName: '',
  countyNo: '',
  countyName: '',
  zoNo: '',
  zoName: '',
  doNo: '',
  doName: '',
};

const PAGE_SIZE = 10;

const downloadBlob = (data: BlobPart, filename: string) => {
  const url = URL.createObjectURL(new Blob([data]));
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  link.click();
  URL.revokeObjectURL(url);
};

export const ZipMaintenance = () => {
  const [filters, setFilters] = useState<ZipFiltersState>(INITIAL_FILTERS);
  const [tableData, setTableData] = useState<ZipRecord[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [currentPage, setCurrentPage] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);
  
  const [editingRecord, setEditingRecord] = useState<ZipRecord | null>(null);
  const [isCreating, setIsCreating] = useState(false);

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFilters((prev) => ({ ...prev, [name]: value }));
  };

  const fetchZipData = useCallback(async (page = 1) => {
    setLoading(true);
    setError(null);
    try {
      const { data } = await axios.get<SearchApiResponse>(`${API_BASE_URL}/Search`, {
        params: { ...filters, pageNumber: page, pageSize: PAGE_SIZE },
      });
      setTableData(data.items ?? []);
      setTotalRecords(data.totalCount ?? 0);
      setCurrentPage(page);
    } catch (err) {
      setError('Failed to load ZIP territory records.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  }, [filters]);

  const handleExport = async () => {
    try {
      const { data } = await axios.get(`${API_BASE_URL}/Export`, {
        params: filters,
        responseType: 'blob',
      });
      const filename = `Zip_Territory_Data_${new Date().toISOString().slice(0, 10)}.xlsx`;
      downloadBlob(data, filename);
    } catch (err) {
      alert('Export failed. Please try again.');
    }
  };

  useEffect(() => {
    fetchZipData(1);
  }, []);

  const totalPages = Math.ceil(totalRecords / PAGE_SIZE);

  if (editingRecord) {
    return (
      <div className="p-6 bg-white min-h-screen">
        <ZipEditModule
          record={editingRecord}
          onBack={() => setEditingRecord(null)}
          onSaveSuccess={() => {
            setEditingRecord(null);
            fetchZipData(currentPage);
          }}
        />
      </div>
    );
  }

  if (isCreating) {
    return (
      <div className="p-6 bg-white min-h-screen">
        <ZipCreateModule
          onBack={() => setIsCreating(false)}
          onSuccess={() => {
            setIsCreating(false);
            fetchZipData(1);
          }}
        />
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6 bg-white min-h-screen">
      <div className="flex justify-between items-center border-b pb-4">
        <h1 className="text-2xl font-bold text-gray-800">Zip Code Territory Management</h1>
        
        <div className="flex space-x-2">
          <button
            onClick={() => setIsCreating(true)}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded shadow text-sm font-semibold transition"
          >
            + Add Zip Record
          </button>
          <button
            onClick={handleExport}
            className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded shadow text-sm font-semibold transition"
          >
            Export Data Table
          </button>
        </div>
      </div>

      {error && (
        <div className="p-3 bg-red-100 border border-red-300 text-red-700 rounded text-sm">
          {error}
        </div>
      )}

      <ZipSearchFilters
        filters={filters}
        onFilterChange={handleFilterChange}
        onQuery={() => fetchZipData(1)}
      />

      <ZipDataTable
        tableData={tableData}
        loading={loading}
        onEdit={(row) => setEditingRecord(row)}
      />

      <ZipPagination
        currentPage={currentPage}
        totalPages={totalPages}
        totalRecords={totalRecords}
        currentCount={tableData.length}
        onPageChange={fetchZipData}
      />
    </div>
  );
};