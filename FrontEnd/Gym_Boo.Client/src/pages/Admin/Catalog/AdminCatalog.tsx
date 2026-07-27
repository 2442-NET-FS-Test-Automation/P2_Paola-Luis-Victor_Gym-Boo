import {
  useEffect,
  useMemo,
  useState,
  type FormEvent,
} from "react";

import axios from "axios";

import {
  Archive,
  Edit3,
  Plus,
  Search,
  Trash2,
} from "lucide-react";

import {
  createDiscipline,
  deleteDiscipline,
  getDisciplines,
  toggleDisciplineStatus,
  updateDiscipline,
  type Discipline,
} from "../../../api/admin";

import Modal from "../../../components/Modal/Modal";
import "./AdminCatalog.css";

type StatusFilter =
  | "all"
  | "active"
  | "inactive";

const getDisciplineStatus = (
  discipline: Discipline
): boolean => {
  return (
    discipline.isActive ??
    discipline.isAvailable ??
    discipline.available ??
    true
  );
};

const getApiError = (
  error: unknown,
  fallback: string
): string => {
  if (!axios.isAxiosError(error)) {
    return fallback;
  }

  const data = error.response?.data;

  if (typeof data === "string") {
    return data;
  }

  return data?.message ?? fallback;
};

const AdminCatalog = () => {
  const [disciplines, setDisciplines] =
    useState<Discipline[]>([]);

  const [search, setSearch] = useState("");

  const [statusFilter, setStatusFilter] =
    useState<StatusFilter>("all");

  const [loading, setLoading] = useState(true);

  const [submitting, setSubmitting] =
    useState(false);

  const [pageError, setPageError] =
    useState<string | null>(null);

  const [modalError, setModalError] =
    useState<string | null>(null);

  const [isCreateOpen, setIsCreateOpen] =
    useState(false);

  const [
    editingDiscipline,
    setEditingDiscipline,
  ] = useState<Discipline | null>(null);

  const [disciplineName, setDisciplineName] =
    useState("");

  const loadDisciplines = async () => {
    setLoading(true);
    setPageError(null);

    try {
      const result = await getDisciplines();
      setDisciplines(result);
    } catch (error: unknown) {
      setPageError(
        getApiError(
          error,
          "The class catalog could not be loaded."
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadDisciplines();
  }, []);

  const statistics = useMemo(() => {
    const active = disciplines.filter(
      getDisciplineStatus
    ).length;

    return {
      total: disciplines.length,
      active,
      inactive: disciplines.length - active,
    };
  }, [disciplines]);

  const filteredDisciplines = useMemo(() => {
    const term = search.trim().toLowerCase();

    return disciplines.filter((discipline) => {
      const isActive =
        getDisciplineStatus(discipline);

      const matchesSearch =
        !term ||
        discipline.name
          .toLowerCase()
          .includes(term);

      const matchesStatus =
        statusFilter === "all" ||
        (statusFilter === "active" &&
          isActive) ||
        (statusFilter === "inactive" &&
          !isActive);

      return matchesSearch && matchesStatus;
    });
  }, [disciplines, search, statusFilter]);

  const openCreateModal = () => {
    setDisciplineName("");
    setModalError(null);
    setIsCreateOpen(true);
  };

  const openEditModal = (
    discipline: Discipline
  ) => {
    setDisciplineName(discipline.name);
    setModalError(null);
    setEditingDiscipline(discipline);
  };

  const closeModals = () => {
    setIsCreateOpen(false);
    setEditingDiscipline(null);
    setDisciplineName("");
    setModalError(null);
  };

  const handleCreate = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    const name = disciplineName.trim();

    if (!name) {
      setModalError(
        "The discipline name is required."
      );
      return;
    }

    setSubmitting(true);
    setModalError(null);

    try {
      await createDiscipline(name);

      closeModals();
      await loadDisciplines();
    } catch (error: unknown) {
      setModalError(
        getApiError(
          error,
          "The discipline could not be created."
        )
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleUpdate = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    if (!editingDiscipline) {
      return;
    }

    const name = disciplineName.trim();

    if (!name) {
      setModalError(
        "The discipline name is required."
      );
      return;
    }

    setSubmitting(true);
    setModalError(null);

    try {
      await updateDiscipline(
        editingDiscipline.id,
        name
      );

      closeModals();
      await loadDisciplines();
    } catch (error: unknown) {
      setModalError(
        getApiError(
          error,
          "The discipline could not be updated."
        )
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleToggle = async (
    discipline: Discipline
  ) => {
    setPageError(null);

    try {
      await toggleDisciplineStatus(
        discipline.id
      );

      await loadDisciplines();
    } catch (error: unknown) {
      setPageError(
        getApiError(
          error,
          "The discipline status could not be changed."
        )
      );
    }
  };

  const handleDelete = async (
    discipline: Discipline
  ) => {
    const confirmed = window.confirm(
      `Delete "${discipline.name}" permanently?`
    );

    if (!confirmed) {
      return;
    }

    setPageError(null);

    try {
      await deleteDiscipline(
        discipline.name
      );

      await loadDisciplines();
    } catch (error: unknown) {
      setPageError(
        getApiError(
          error,
          "The discipline could not be deleted."
        )
      );
    }
  };

  return (
    <div className="catalog-page">
      <header className="catalog-header">
        <div>
          <p className="admin-eyebrow">
            ADMIN PANEL
          </p>

          <h1>CLASS CATALOG</h1>

          <p className="admin-subtitle">
            {statistics.active} active disciplines
            · {statistics.inactive} archived
          </p>
        </div>

        <button
          type="button"
          className="admin-primary-button"
          onClick={openCreateModal}
        >
          <Plus size={19} />
          New discipline
        </button>
      </header>

      <section className="catalog-stats">
        <article className="catalog-stat-card">
          <strong>{statistics.total}</strong>
          <span>Total disciplines</span>
        </article>

        <article className="catalog-stat-card catalog-stat-card--active">
          <strong>{statistics.active}</strong>
          <span>Active disciplines</span>
        </article>

        <article className="catalog-stat-card catalog-stat-card--inactive">
          <strong>{statistics.inactive}</strong>
          <span>Archived disciplines</span>
        </article>
      </section>

      <section className="catalog-toolbar">
        <label className="admin-search">
          <Search size={18} />

          <input
            value={search}
            placeholder="Search disciplines..."
            onChange={(event) =>
              setSearch(event.target.value)
            }
          />
        </label>

        <div className="admin-segmented-control">
          <button
            type="button"
            className={
              statusFilter === "all"
                ? "is-active"
                : ""
            }
            onClick={() =>
              setStatusFilter("all")
            }
          >
            All
          </button>

          <button
            type="button"
            className={
              statusFilter === "active"
                ? "is-active"
                : ""
            }
            onClick={() =>
              setStatusFilter("active")
            }
          >
            Active
          </button>

          <button
            type="button"
            className={
              statusFilter === "inactive"
                ? "is-active"
                : ""
            }
            onClick={() =>
              setStatusFilter("inactive")
            }
          >
            Archived
          </button>
        </div>
      </section>

      {pageError && (
        <p className="admin-error">
          {pageError}
        </p>
      )}

      <section className="admin-data-panel">
        <div className="catalog-table catalog-table--header">
          <span>ID</span>
          <span>Discipline</span>
          <span>Status</span>
          <span>Actions</span>
        </div>

        {loading && (
          <p className="admin-state">
            Loading catalog...
          </p>
        )}

        {!loading &&
          filteredDisciplines.length === 0 && (
            <p className="admin-state">
              No disciplines match the selected
              filters.
            </p>
          )}

        {!loading &&
          filteredDisciplines.map(
            (discipline) => {
              const isActive =
                getDisciplineStatus(
                  discipline
                );

              return (
                <div
                  key={discipline.id}
                  className="catalog-table catalog-table--row"
                >
                  <span className="catalog-id">
                    #{discipline.id}
                  </span>

                  <div className="catalog-name">
                    <span className="catalog-name__icon">
                      {discipline.name
                        .charAt(0)
                        .toUpperCase()}
                    </span>

                    <strong>
                      {discipline.name}
                    </strong>
                  </div>

                  <span
                    className={`status-pill ${
                      isActive
                        ? "status-pill--active"
                        : "status-pill--inactive"
                    }`}
                  >
                    {isActive
                      ? "Active"
                      : "Archived"}
                  </span>

                  <div className="catalog-actions">
                    <button
                      type="button"
                      className="admin-secondary-button"
                      onClick={() =>
                        openEditModal(
                          discipline
                        )
                      }
                    >
                      <Edit3 size={15} />
                      Edit
                    </button>

                    <button
                      type="button"
                      className="admin-secondary-button"
                      onClick={() =>
                        void handleToggle(
                          discipline
                        )
                      }
                    >
                      <Archive size={15} />

                      {isActive
                        ? "Archive"
                        : "Activate"}
                    </button>

                    <button
                      type="button"
                      className="admin-icon-danger"
                      aria-label={`Delete ${discipline.name}`}
                      onClick={() =>
                        void handleDelete(
                          discipline
                        )
                      }
                    >
                      <Trash2 size={17} />
                    </button>
                  </div>
                </div>
              );
            }
          )}
      </section>

      <Modal
        isOpen={isCreateOpen}
        title="Add new discipline"
        onClose={closeModals}
      >
        <form
          className="admin-modal-form"
          onSubmit={handleCreate}
        >
          <label htmlFor="new-discipline">
            Discipline name
          </label>

          <input
            id="new-discipline"
            value={disciplineName}
            placeholder="e.g. Yoga"
            onChange={(event) =>
              setDisciplineName(
                event.target.value
              )
            }
            autoFocus
          />

          {modalError && (
            <p className="admin-modal-error">
              {modalError}
            </p>
          )}

          <div className="admin-modal-actions">
            <button
              type="button"
              className="admin-modal-cancel"
              onClick={closeModals}
            >
              Cancel
            </button>

            <button
              type="submit"
              className="admin-modal-submit"
              disabled={submitting}
            >
              {submitting
                ? "Creating..."
                : "Create"}
            </button>
          </div>
        </form>
      </Modal>

      <Modal
        isOpen={editingDiscipline !== null}
        title="Edit discipline"
        onClose={closeModals}
      >
        <form
          className="admin-modal-form"
          onSubmit={handleUpdate}
        >
          <label htmlFor="edit-discipline">
            Discipline name
          </label>

          <input
            id="edit-discipline"
            value={disciplineName}
            onChange={(event) =>
              setDisciplineName(
                event.target.value
              )
            }
            autoFocus
          />

          {modalError && (
            <p className="admin-modal-error">
              {modalError}
            </p>
          )}

          <div className="admin-modal-actions">
            <button
              type="button"
              className="admin-modal-cancel"
              onClick={closeModals}
            >
              Cancel
            </button>

            <button
              type="submit"
              className="admin-modal-submit"
              disabled={submitting}
            >
              {submitting
                ? "Saving..."
                : "Save changes"}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};

export default AdminCatalog;