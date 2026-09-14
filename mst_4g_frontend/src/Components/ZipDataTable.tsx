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
  onOpenResume: (row: ZipRecord) => void;
}

const formatDate = (dateStr?: string) => {
  if (!dateStr) return '-';
  const parsed = new Date(dateStr);
  return isNaN(parsed.getTime()) ? '-' : parsed.toLocaleDateString();
};

export const ZipDataTable = ({ tableData, loading, onOpenResume }: ZipDataTableProps) => {
  const TOTAL_COLUMNS = 11;

  return (
    <div className="overflow-x-auto border rounded-lg shadow-sm">
      <table className="min-w-full text-left text-sm">
        <thead className="bg-gray-100 text-gray-700 font-bold border-b">
          <tr>
            <th className="p-3 border-r">ZIP No</th>
            <th className="p-3 border-r">ZIP Name</th>
            <th className="p-3 border-r">County No</th>
            <th className="p-3 border-r">County Name</th>
            <th className="p-3 border-r">ZO No</th>
            <th className="p-3 border-r">ZO Name</th>
            <th className="p-3 border-r">DO No</th>
            <th className="p-3 border-r">DO Name</th>
            <th className="p-3 border-r">Eff Date From</th>
            <th className="p-3 border-r">Eff Date To</th>
            <th className="p-3 text-center">Action</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {loading ? (
            <tr>
              <td colSpan={TOTAL_COLUMNS} className="p-6 text-center text-gray-500">
                Loading records...
              </td>
            </tr>
          ) : tableData.length === 0 ? (
            <tr>
              <td colSpan={TOTAL_COLUMNS} className="p-6 text-center text-gray-500">
                No data found.
              </td>
            </tr>
          ) : (
            tableData.map((row) => (
              <tr key={row.zipId} className="hover:bg-gray-50 transition">
                <td className="p-3 border-r font-medium">{row.zipNo}</td>
                <td className="p-3 border-r">{row.zipName}</td>
                <td className="p-3 border-r">{row.countyNo}</td>
                <td className="p-3 border-r">{row.countyName ?? '-'}</td>
                <td className="p-3 border-r">{row.zoNo}</td>
                <td className="p-3 border-r">{row.zoName ?? '-'}</td>
                <td className="p-3 border-r">{row.doNo}</td>
                <td className="p-3 border-r">{row.doName ?? '-'}</td>
                <td className="p-3 border-r">{formatDate(row.effDateFrom)}</td>
                <td className="p-3 border-r">{formatDate(row.effDateTo)}</td>
                <td className="p-3 text-center">
                  <button
                    onClick={() => onOpenResume(row)}
                    className="bg-amber-500 hover:bg-amber-600 text-white px-3 py-1 rounded text-xs font-semibold"
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