import {
  useEffect,
  useMemo,
  useState,
  type FormEvent,
} from "react";

import axios from "axios";

import {
  Plus,
  Search,
} from "lucide-react";

import {
    getInstructors,
    createInstructor,
    updateInstructor,
    toggleInstructorStatus,
    deleteInstructor,
    type AdminInstructor,
} from "../../../api/admin";

import Modal from "../../../components/Modal/Modal";

import "./AdminInstructors.css";

type InstructorFilter =
  | "all"
  | "active"
  | "inactive";

interface InstructorForm {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

const emptyForm: InstructorForm = {
  firstName: "",
  lastName: "",
  email: "",
  password: "",
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

const getInitials = (
  instructor: AdminInstructor
): string => {
  return `${instructor.name.charAt(
    0
  )}${instructor.lastName.charAt(0)}`.toUpperCase();
};

const AdminInstructors = () => {
  const [instructors, setInstructors] =
    useState<AdminInstructor[]>([]);

  const [search, setSearch] = useState("");

  const [filter, setFilter] =
    useState<InstructorFilter>("all");

  const [loading, setLoading] =
    useState(true);

  const [submitting, setSubmitting] =
    useState(false);

  const [pageError, setPageError] =
    useState<string | null>(null);

  const [modalError, setModalError] =
    useState<string | null>(null);

  const [isCreateOpen, setIsCreateOpen] =
    useState(false);

  const [
    editingInstructor,
    setEditingInstructor,
  ] = useState<AdminInstructor | null>(null);

  const [form, setForm] =
    useState<InstructorForm>(emptyForm);

const [
    instructorToDelete,
    setInstructorToDelete,
] = useState<AdminInstructor | null>(null);

const [deleting, setDeleting] =
    useState(false);

const [deleteError, setDeleteError] =
    useState<string | null>(null);

  const loadInstructors = async () => {
    setLoading(true);
    setPageError(null);

    try {
      const result = await getInstructors();
      setInstructors(result);
    } catch (error: unknown) {
      setPageError(
        getApiError(
          error,
          "The instructors could not be loaded."
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadInstructors();
  }, []);

  const statistics = useMemo(() => {
    const active = instructors.filter(
      (instructor) => instructor.isActive
    ).length;

    return {
      total: instructors.length,
      active,
      inactive: instructors.length - active,
    };
  }, [instructors]);

  const filteredInstructors = useMemo(() => {
    const term = search.trim().toLowerCase();

    return instructors.filter((instructor) => {
      const fullName =
        `${instructor.name} ${instructor.lastName}`.toLowerCase();

      const matchesSearch =
        !term ||
        fullName.includes(term) ||
        instructor.email
          .toLowerCase()
          .includes(term);

      const matchesFilter =
        filter === "all" ||
        (filter === "active" &&
          instructor.isActive) ||
        (filter === "inactive" &&
          !instructor.isActive);

      return matchesSearch && matchesFilter;
    });
  }, [instructors, search, filter]);

  const updateForm = (
    field: keyof InstructorForm,
    value: string
  ) => {
    setForm((current) => ({
      ...current,
      [field]: value,
    }));
  };

  const openCreateModal = () => {
    setForm(emptyForm);
    setModalError(null);
    setIsCreateOpen(true);
  };

  const openEditModal = (
    instructor: AdminInstructor
  ) => {
    setForm({
      firstName: instructor.name,
      lastName: instructor.lastName,
      email: instructor.email,
      password: "",
    });

    setModalError(null);
    setEditingInstructor(instructor);
  };

  const closeModals = () => {
    setIsCreateOpen(false);
    setEditingInstructor(null);
    setForm(emptyForm);
    setModalError(null);
  };

  const handleCreate = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    if (
      !form.firstName.trim() ||
      !form.lastName.trim() ||
      !form.email.trim() ||
      !form.password
    ) {
      setModalError(
        "All fields are required."
      );
      return;
    }

    setSubmitting(true);
    setModalError(null);

    try {
      await createInstructor({
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        email: form.email.trim(),
        password: form.password,
      });

      closeModals();
      await loadInstructors();
    } catch (error: unknown) {
      setModalError(
        getApiError(
          error,
          "The instructor could not be created."
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

    if (!editingInstructor) {
      return;
    }

    if (
      !form.firstName.trim() ||
      !form.lastName.trim() ||
      !form.email.trim()
    ) {
      setModalError(
        "First name, last name and email are required."
      );
      return;
    }

    setSubmitting(true);
    setModalError(null);

    try {
      await updateInstructor({
        id: editingInstructor.id,
        name: form.firstName.trim(),
        lastName: form.lastName.trim(),
        email: form.email.trim(),
        role: 2,
        isActive:
          editingInstructor.isActive,
      });

      closeModals();
      await loadInstructors();
    } catch (error: unknown) {
      setModalError(
        getApiError(
          error,
          "The instructor could not be updated."
        )
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleToggleStatus = async (
        instructor: AdminInstructor
    ) => {
        try {
            await toggleInstructorStatus(
                instructor
            );

            await loadInstructors();
        } catch (error) {
            setPageError(
                "Unable to change instructor status."
            );
        }
    };

    const openDeleteModal = (
        instructor: AdminInstructor
    ) => {
        setDeleteError(null);

        setInstructorToDelete(
            instructor
        );
    };

    const closeDeleteModal = () => {
        setInstructorToDelete(null);

        setDeleteError(null);
    };

    const handleDelete = async () => {
        if (!instructorToDelete) return;

        try {
            setDeleting(true);

            await deleteInstructor(
                instructorToDelete.id
            );

            closeDeleteModal();

            await loadInstructors();
        } catch {
            setDeleteError(
                "This instructor has scheduled classes and cannot be deleted."
            );
        } finally {
            setDeleting(false);
        }
    };

  return (
    <div className="instructors-page">
      <header className="instructors-header">
        <div>
          <p className="admin-eyebrow">
            ADMIN PANEL
          </p>

          <h1>INSTRUCTOR MANAGEMENT</h1>

          <p className="admin-subtitle">
            {statistics.active} active instructors
            · {statistics.inactive} inactive
          </p>
        </div>

        <button
          type="button"
          className="admin-primary-button"
          onClick={openCreateModal}
        >
          <Plus size={19} />
          Add instructor
        </button>
      </header>

      <section className="instructor-stats">
        <article className="instructor-stat-card">
          <strong>
            {statistics.total}
          </strong>

          <span>Total instructors</span>
        </article>

        <article className="instructor-stat-card instructor-stat-card--active">
          <strong>
            {statistics.active}
          </strong>

          <span>Active instructors</span>
        </article>

        <article className="instructor-stat-card instructor-stat-card--inactive">
          <strong>
            {statistics.inactive}
          </strong>

          <span>Inactive instructors</span>
        </article>
      </section>

      <section className="instructor-toolbar">
        <label className="admin-search">
          <Search size={18} />

          <input
            value={search}
            placeholder="Search by name or email..."
            onChange={(event) =>
              setSearch(event.target.value)
            }
          />
        </label>

        <div className="admin-segmented-control">
          <button
            type="button"
            className={
              filter === "all"
                ? "is-active"
                : ""
            }
            onClick={() =>
              setFilter("all")
            }
          >
            All
          </button>

          <button
            type="button"
            className={
              filter === "active"
                ? "is-active"
                : ""
            }
            onClick={() =>
              setFilter("active")
            }
          >
            Active
          </button>

          <button
            type="button"
            className={
              filter === "inactive"
                ? "is-active"
                : ""
            }
            onClick={() =>
              setFilter("inactive")
            }
          >
            Inactive
          </button>
        </div>
      </section>

      {pageError && (
        <p className="admin-error">
          {pageError}
        </p>
      )}

      <section className="admin-data-panel">
        <div className="instructors-table instructors-table--header">
          <span>Instructor</span>
          <span>Email</span>
          <span>Role</span>
          <span>Status</span>
          <span>Actions</span>
        </div>

        {loading && (
          <p className="admin-state">
            Loading instructors...
          </p>
        )}

        {!loading &&
          filteredInstructors.length === 0 && (
            <p className="admin-state">
              No instructors match the selected
              filters.
            </p>
          )}

        {!loading &&
          filteredInstructors.map(
            (instructor) => (
              <div
                key={instructor.id}
                className={`instructors-table instructors-table--row ${
                  !instructor.isActive
                    ? "is-disabled"
                    : ""
                }`}
              >
                <div className="instructor-identity">
                  <span className="instructor-avatar">
                    {getInitials(
                      instructor
                    )}
                  </span>

                  <div>
                    <strong>
                      {instructor.name}{" "}
                      {instructor.lastName}
                    </strong>

                    <small>
                      ID #{instructor.id}
                    </small>
                  </div>
                </div>

                <span className="instructor-email">
                  {instructor.email}
                </span>

                <span className="instructor-role">
                  Instructor
                </span>

                <span
                  className={`status-pill ${
                    instructor.isActive
                      ? "status-pill--active"
                      : "status-pill--inactive"
                  }`}
                >
                  {instructor.isActive
                    ? "Active"
                    : "Inactive"}
                </span>

                <div className="instructor-actions">

                    <button
                        className="admin-secondary-button"
                        onClick={() =>
                            openEditModal(instructor)
                        }
                    >
                        Edit
                    </button>

                    <button
                        className="admin-status-button"
                        onClick={() =>
                            handleToggleStatus(
                                instructor
                            )
                        }
                    >
                        {instructor.isActive
                            ? "Disable"
                            : "Enable"}
                    </button>

                    <button
                        className="admin-delete-button"
                        onClick={() =>
                            openDeleteModal(
                                instructor
                            )
                        }
                    >
                        Delete
                    </button>

                </div>
              </div>
            )
          )}
      </section>

      <Modal
        isOpen={isCreateOpen}
        title="Add instructor"
        onClose={closeModals}
      >
        <form
          className="admin-modal-form"
          onSubmit={handleCreate}
        >
          <InstructorFields
            form={form}
            showPassword
            onChange={updateForm}
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
        isOpen={editingInstructor !== null}
        title="Edit instructor"
        onClose={closeModals}
      >
        <form
          className="admin-modal-form"
          onSubmit={handleUpdate}
        >
          <InstructorFields
            form={form}
            showPassword={false}
            onChange={updateForm}
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

      <Modal
            isOpen={
                instructorToDelete != null
            }
            title="Delete Instructor"
            onClose={closeDeleteModal}
        >

            <p>
                Are you sure you want to
                permanently delete
                <strong>
                    {" "}
                    {instructorToDelete?.name}
                </strong>
                ?
            </p>

            {deleteError && (
                <p className="admin-modal-error">
                    {deleteError}
                </p>
            )}

            <div className="admin-modal-actions">

                <button
                    className="admin-modal-cancel"
                    onClick={closeDeleteModal}
                >
                    Cancel
                </button>

                <button
                    className="admin-modal-delete"
                    onClick={handleDelete}
                >
                    {deleting
                        ? "Deleting..."
                        : "Delete"}
                </button>

            </div>

        </Modal>
    </div>
  );
};

interface InstructorFieldsProps {
  form: InstructorForm;
  showPassword: boolean;
  onChange: (
    field: keyof InstructorForm,
    value: string
  ) => void;
}

const InstructorFields = ({
  form,
  showPassword,
  onChange,
}: InstructorFieldsProps) => {
  return (
    <>
      <label htmlFor="instructor-first-name">
        First name
      </label>

      <input
        id="instructor-first-name"
        value={form.firstName}
        onChange={(event) =>
          onChange(
            "firstName",
            event.target.value
          )
        }
        required
      />

      <label htmlFor="instructor-last-name">
        Last name
      </label>

      <input
        id="instructor-last-name"
        value={form.lastName}
        onChange={(event) =>
          onChange(
            "lastName",
            event.target.value
          )
        }
        required
      />

      <label htmlFor="instructor-email">
        Email
      </label>

      <input
        id="instructor-email"
        type="email"
        value={form.email}
        onChange={(event) =>
          onChange(
            "email",
            event.target.value
          )
        }
        required
      />

      {showPassword && (
        <>
          <label htmlFor="instructor-password">
            Password
          </label>

          <input
            id="instructor-password"
            type="password"
            value={form.password}
            minLength={8}
            onChange={(event) =>
              onChange(
                "password",
                event.target.value
              )
            }
            required
          />
        </>
      )}
    </>
  );
};

export default AdminInstructors;
