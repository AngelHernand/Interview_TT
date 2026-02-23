import { ChevronLeft, ChevronRight } from 'lucide-react';

export default function Pagination({ currentPage, totalPages, onPageChange }) {
  if (totalPages <= 1) return null;

  const pages = [];
  const maxVisible = 5;
  let start = Math.max(1, currentPage - Math.floor(maxVisible / 2));
  let end = Math.min(totalPages, start + maxVisible - 1);

  if (end - start < maxVisible - 1) {
    start = Math.max(1, end - maxVisible + 1);
  }

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  const pageBtn = (active) =>
    `inline-flex items-center justify-center h-8 w-8 text-xs font-medium rounded-md transition-all duration-150 ${
      active
        ? 'bg-primary-500 text-white'
        : 'text-neutral-600 hover:bg-neutral-100'
    }`;

  return (
    <div className="flex items-center justify-center gap-1 mt-6">
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
        className="btn btn-ghost btn-sm gap-1 disabled:opacity-40"
      >
        <ChevronLeft size={14} />
        Anterior
      </button>

      {start > 1 && (
        <>
          <button onClick={() => onPageChange(1)} className={pageBtn(false)}>1</button>
          {start > 2 && <span className="px-1 text-neutral-400 text-xs">...</span>}
        </>
      )}

      {pages.map((page) => (
        <button key={page} onClick={() => onPageChange(page)} className={pageBtn(page === currentPage)}>
          {page}
        </button>
      ))}

      {end < totalPages && (
        <>
          {end < totalPages - 1 && <span className="px-1 text-neutral-400 text-xs">...</span>}
          <button onClick={() => onPageChange(totalPages)} className={pageBtn(false)}>{totalPages}</button>
        </>
      )}

      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPages}
        className="btn btn-ghost btn-sm gap-1 disabled:opacity-40"
      >
        Siguiente
        <ChevronRight size={14} />
      </button>
    </div>
  );
}
