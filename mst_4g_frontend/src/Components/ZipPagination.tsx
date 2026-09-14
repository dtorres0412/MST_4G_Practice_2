interface ZipPaginationProps {
  currentPage: number;
  totalPages: number;
  totalRecords: number;
  currentCount: number;
  onPageChange: (page: number) => void;
}

export const ZipPagination = ({
  currentPage,
  totalPages,
  totalRecords,
  currentCount,
  onPageChange,
}: ZipPaginationProps) => {
  const safeTotalPages = Math.max(totalPages, 1);
  const isFirstPage = currentPage <= 1;
  const isLastPage = currentPage >= safeTotalPages || totalRecords === 0;

  return (
    <div className="flex justify-between items-center text-sm text-gray-600">
      <div>
        Showing <span className="font-semibold text-gray-800">{currentCount}</span> of{' '}
        <span className="font-semibold text-gray-800">{totalRecords}</span> records
      </div>

      <div className="flex items-center space-x-2">
        <button
          disabled={isFirstPage}
          onClick={() => onPageChange(currentPage - 1)}
          className="px-3 py-1 border rounded bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
        >
          Previous
        </button>

        <span className="text-xs font-medium px-2">
          Page <span className="font-bold text-gray-800">{currentPage}</span> of {safeTotalPages}
        </span>

        <button
          disabled={isLastPage}
          onClick={() => onPageChange(currentPage + 1)}
          className="px-3 py-1 border rounded bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
        >
          Next
        </button>
      </div>
    </div>
  );
};