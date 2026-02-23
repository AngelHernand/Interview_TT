import { Loader2 } from 'lucide-react';

export default function Spinner({ size = 'md', className = '' }) {
  const sizeMap = { sm: 16, md: 24, lg: 32 };

  return (
    <div className={`flex justify-center items-center ${className}`}>
      <Loader2
        size={sizeMap[size]}
        className="animate-spin text-primary-500"
      />
    </div>
  );
}
