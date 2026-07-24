import {
  useEffect,
  useState,
  type FormEvent,
} from "react";
import axios from "axios";
import {
  createDiscipline,
  deleteDiscipline,
  getDisciplines,
  toggleDiscipline,
  updateDiscipline,
  type Discipline,
} from "../../../api/admin";

import "./AdminCatalog.css";

const AdminCatalog = () => {
  const [disciplines, setDisciplines] = useState<
    Discipline[]
  >([]);

  const [newName, setNewName] = useState("");
  const [editingId, setEditingId] = useState<
    number | null
  >(null);

  const [editingName, setEditingName] =
    useState("");

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [error, setError] = useState<
    string | null
  >(null);

  const loadDisciplines = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await getDisciplines();
      setDisciplines(data);
    } catch {
      setError("Unable to load disciplines.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadDisciplines();
  }, []);

  const handleCreate = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    const normalizedName = newName.trim();

    if (!normalizedName) {
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await createDiscipline(normalizedName);
      setNewName("");
      await loadDisciplines();
    } catch (error: unknown) {
      if (axios.isAxiosError(error)) {
        setError(
          error.response?.data?.message ??
            error.response?.data ??
            "Unable to create discipline."
        );
      } else {
        setError("Unable to create discipline.");
      }
    } finally {
      setSaving(false);
    }
  };

  const handleUpdate = async (
    id: number
  ) => {
    const normalizedName = editingName.trim();

    if (!normalizedName) {
      return;
    }

    try {
      await updateDiscipline(id, normalizedName);
      setEditingId(null);
      setEditingName("");
      await loadDisciplines();
    } catch {
      setError("Unable to update discipline.");
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

    try {
      await deleteDiscipline(discipline.name);
      await loadDisciplines();
    } catch {
      setError(
        "Unable to delete the discipline. It may be used by existing classes."
      );
    }
  };

  return (
    <div className="admin-page">
      <header className="admin-page__header">
        <p>ADMIN PORTAL</p>
        <h1>CLASS CATALOG</h1>
        <span>
          Manage the disciplines available at
          GymBoo.
        </span>
      </header>

      <form
        className="admin-create-form"
        onSubmit={handleCreate}
      >
        <input
          type="text"
          value={newName}
          placeholder="New discipline name"
          onChange={(event) =>
            setNewName(event.target.value)
          }
        />

        <button
          type="submit"
          disabled={saving}
        >
          {saving
            ? "Adding..."
            : "Add Discipline"}
        </button>
      </form>

      {error && (
        <p className="admin-page__error">
          {error}
        </p>
      )}

      {loading ? (
        <p>Loading disciplines...</p>
      ) : (
        <section className="admin-table">
          <div className="admin-table__header">
            <span>ID</span>
            <span>Name</span>
            <span>Status</span>
            <span>Actions</span>
          </div>

          {disciplines.map((discipline) => {
            const isActive =
              discipline.isActive ??
              discipline.available ??
              true;

            return (
              <div
                className="admin-table__row"
                key={discipline.id}
              >
                <span>{discipline.id}</span>

                {editingId === discipline.id ? (
                  <input
                    value={editingName}
                    onChange={(event) =>
                      setEditingName(
                        event.target.value
                      )
                    }
                  />
                ) : (
                  <strong>{discipline.name}</strong>
                )}

                <span>
                  {isActive
                    ? "Active"
                    : "Inactive"}
                </span>

                <div className="admin-table__actions">
                  {editingId === discipline.id ? (
                    <>
                      <button
                        type="button"
                        onClick={() =>
                          void handleUpdate(
                            discipline.id
                          )
                        }
                      >
                        Save
                      </button>

                      <button
                        type="button"
                        onClick={() =>
                          setEditingId(null)
                        }
                      >
                        Cancel
                      </button>
                    </>
                  ) : (
                    <button
                      type="button"
                      onClick={() => {
                        setEditingId(
                          discipline.id
                        );

                        setEditingName(
                          discipline.name
                        );
                      }}
                    >
                      Edit
                    </button>
                  )}

                  <button
                    type="button"
                    onClick={async () => {
                      await toggleDiscipline(
                        discipline.id
                      );

                      await loadDisciplines();
                    }}
                  >
                    {isActive
                      ? "Disable"
                      : "Enable"}
                  </button>

                  <button
                    type="button"
                    className="danger-button"
                    onClick={() =>
                      void handleDelete(
                        discipline
                      )
                    }
                  >
                    Delete
                  </button>
                </div>
              </div>
            );
          })}
        </section>
      )}
    </div>
  );
};

export default AdminCatalog;