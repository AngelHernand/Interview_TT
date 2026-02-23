import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import userService from '../services/userService';
import Spinner from '../components/UI/Spinner';
import Pagination from '../components/UI/Pagination';
import Modal from '../components/UI/Modal';
import toast from 'react-hot-toast';
import { Search, Plus, Eye, Pencil, Lock, Unlock, Trash2, UsersIcon } from 'lucide-react';

export default function Users() {
  const { isAdmin } = useAuth();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [pagination, setPagination] = useState({ page: 1, pageSize: 10, totalCount: 0, totalPages: 0 });
  const [search, setSearch] = useState('');
  const [deleteModal, setDeleteModal] = useState({ open: false, user: null });
  const [actionLoading, setActionLoading] = useState(false);

  const fetchUsers = async (page = 1) => {
    setLoading(true);
    try {
      const result = await userService.getAll(page, pagination.pageSize);
      if (result.success) {
        setUsers(result.data.items);
        setPagination({
          page: result.data.page,
          pageSize: result.data.pageSize,
          totalCount: result.data.totalCount,
          totalPages: result.data.totalPages,
        });
      }
    } catch (error) {
      console.error('Error fetching users:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchUsers(); }, []);

  const handlePageChange = (page) => fetchUsers(page);

  const handleDelete = async () => {
    if (!deleteModal.user) return;
    setActionLoading(true);
    try {
      const result = await userService.delete(deleteModal.user.id);
      if (result.success) {
        toast.success('Usuario eliminado correctamente');
        setDeleteModal({ open: false, user: null });
        fetchUsers(pagination.page);
      }
    } catch (error) {
      console.error('Error deleting user:', error);
    } finally {
      setActionLoading(false);
    }
  };

  const handleToggleBlock = async (user) => {
    try {
      const result = await userService.toggleBlock(user.id);
      if (result.success) {
        toast.success(result.message);
        fetchUsers(pagination.page);
      }
    } catch (error) {
      console.error('Error toggling block:', error);
    }
  };

  const filteredUsers = users.filter(
    (u) =>
      u.nombreCompleto?.toLowerCase().includes(search.toLowerCase()) ||
      u.email?.toLowerCase().includes(search.toLowerCase())
  );

  if (loading && users.length === 0) {
    return (
      <div className="flex justify-center items-center h-64">
        <Spinner size="lg" />
      </div>
    );
  }

  return (
    <div className="space-y-5">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div className="flex items-center gap-3">
          <div className="flex items-center justify-center w-10 h-10 rounded-xl bg-primary-100">
            <UsersIcon size={20} className="text-primary-600" />
          </div>
          <div>
            <h1 className="text-lg font-bold text-neutral-900">Usuarios</h1>
            <p className="text-sm text-neutral-500">
              {pagination.totalCount} registros en total
            </p>
          </div>
        </div>
        <div className="flex items-center gap-3">
          <div className="relative">
            <Search size={15} className="absolute left-3 top-1/2 -translate-y-1/2 text-neutral-400" />
            <input
              type="text"
              placeholder="Buscar..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="input-field pl-9"
              style={{ width: 220 }}
            />
          </div>
          {isAdmin && (
            <Link to="/users/new" className="btn btn-primary">
              <Plus size={15} />
              Nuevo usuario
            </Link>
          )}
        </div>
      </div>

      {/* Table */}
      <div className="card p-0 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-neutral-200">
            <thead>
              <tr>
                <th className="table-header px-4 py-3">Nombre</th>
                <th className="table-header px-4 py-3">Email</th>
                <th className="table-header px-4 py-3">Rol</th>
                <th className="table-header px-4 py-3">Estado</th>
                <th className="table-header px-4 py-3 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-neutral-100">
              {filteredUsers.length === 0 ? (
                <tr>
                  <td colSpan="5" className="px-4 py-10 text-center text-sm text-neutral-400">
                    No se encontraron usuarios
                  </td>
                </tr>
              ) : (
                filteredUsers.map((u) => (
                  <tr key={u.id} className="hover:bg-neutral-50 transition-colors duration-100">
                    <td className="table-cell px-4 font-medium">{u.nombreCompleto}</td>
                    <td className="table-cell px-4 text-neutral-500">{u.email}</td>
                    <td className="table-cell px-4">
                      <span className={`badge ${u.rol === 'Admin' ? 'badge-info' : 'badge-default'}`}>
                        {u.rol}
                      </span>
                    </td>
                    <td className="table-cell px-4">
                      {u.bloqueado ? (
                        <span className="badge badge-error">Bloqueado</span>
                      ) : u.activo ? (
                        <span className="badge badge-success">Activo</span>
                      ) : (
                        <span className="badge badge-warning">Inactivo</span>
                      )}
                    </td>
                    <td className="table-cell px-4">
                      <div className="flex justify-end gap-1">
                        <Link
                          to={`/users/${u.id}`}
                          className="btn btn-ghost btn-sm hover:text-primary-600 hover:bg-primary-50"
                          title="Ver detalle"
                        >
                          <Eye size={14} />
                        </Link>
                        <Link
                          to={`/users/${u.id}/edit`}
                          className="btn btn-ghost btn-sm hover:text-accent-600 hover:bg-accent-50"
                          title="Editar"
                        >
                          <Pencil size={14} />
                        </Link>
                        {isAdmin && (
                          <>
                            <button
                              onClick={() => handleToggleBlock(u)}
                              className="btn btn-ghost btn-sm"
                              title={u.bloqueado ? 'Desbloquear' : 'Bloquear'}
                            >
                              {u.bloqueado ? <Unlock size={14} /> : <Lock size={14} />}
                            </button>
                            <button
                              onClick={() => setDeleteModal({ open: true, user: u })}
                              className="btn btn-ghost btn-sm text-error hover:bg-red-50"
                              title="Eliminar"
                            >
                              <Trash2 size={14} />
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {pagination.totalPages > 1 && (
          <div className="px-4 py-3 border-t border-neutral-200">
            <Pagination
              currentPage={pagination.page}
              totalPages={pagination.totalPages}
              onPageChange={handlePageChange}
            />
          </div>
        )}
      </div>

      {/* Delete modal */}
      <Modal
        isOpen={deleteModal.open}
        onClose={() => setDeleteModal({ open: false, user: null })}
        title="Confirmar eliminacion"
        size="sm"
      >
        <p className="text-sm text-neutral-600 mb-6">
          Esta accion eliminara permanentemente al usuario{' '}
          <span className="font-semibold text-neutral-900">{deleteModal.user?.nombreCompleto}</span>.
          No se puede deshacer.
        </p>
        <div className="flex justify-end gap-3" style={{ borderTop: '1px solid var(--color-neutral-200)', paddingTop: 16 }}>
          <button onClick={() => setDeleteModal({ open: false, user: null })} className="btn btn-secondary">
            Cancelar
          </button>
          <button onClick={handleDelete} disabled={actionLoading} className="btn btn-danger">
            {actionLoading ? <Spinner size="sm" /> : 'Eliminar'}
          </button>
        </div>
      </Modal>
    </div>
  );
}
