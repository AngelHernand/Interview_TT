import { Outlet } from 'react-router-dom';
import Sidebar from './Navbar';

export default function MainLayout() {
  return (
    <div className="min-h-screen bg-neutral-50">
      <Sidebar />
      <main
        className="min-h-screen"
        style={{ marginLeft: 240, padding: '28px 32px' }}
      >
        <div className="max-w-[1100px] mx-auto animate-fade-in">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
