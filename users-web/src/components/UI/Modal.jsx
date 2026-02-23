import { X } from 'lucide-react';

export default function Modal({ isOpen, onClose, title, children, size = 'md' }) {
  if (!isOpen) return null;

  const sizeClasses = {
    sm: 'max-w-sm',
    md: 'max-w-[500px]',
    lg: 'max-w-2xl',
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      {/* Overlay */}
      <div
        className="fixed inset-0 transition-opacity"
        style={{ backgroundColor: 'rgba(31, 41, 51, 0.4)' }}
        onClick={onClose}
      />

      {/* Modal */}
      <div className="flex min-h-full items-center justify-center p-4">
        <div
          className={`relative bg-white ${sizeClasses[size]} w-full animate-fade-in`}
          style={{
            borderRadius: '12px',
            border: '1px solid var(--color-neutral-200)',
            boxShadow: 'var(--shadow-3)',
          }}
        >
          {/* Header */}
          <div
            className="flex items-center justify-between px-6 py-4"
            style={{ borderBottom: '1px solid var(--color-neutral-200)' }}
          >
            <h3 className="text-base font-semibold text-neutral-900">{title}</h3>
            <button
              onClick={onClose}
              className="btn-ghost p-1 rounded-md"
              style={{ height: 'auto', width: 'auto', minWidth: 'auto' }}
            >
              <X size={18} className="text-neutral-500" />
            </button>
          </div>

          {/* Content */}
          <div className="px-6 py-5">{children}</div>
        </div>
      </div>
    </div>
  );
}
