import {
  useEffect,
  useState,
  type FormEvent,
} from "react";

import axios from "axios";

import {
  createInstructor,
  getInstructors,
  removeInstructor,
  type AdminInstructor,
} from "../../../api/admin";

import "./AdminInstructors.css";

const AdminInstructors = () => {
  const [instructors, setInstructors] =
    useState<AdminInstructor[]>([]);

  const [firstName, setFirstName] =
    useState("");

  const [lastName, setLastName] =
    useState("");

  const [email, setEmail] = useState("");
  const [password, setPassword] =
    useState("");

  const [loading, setLoading] =
    useState(true);

  const [saving, setSaving] =
    useState(false);

  const [error, setError] = useState<
    string | null
  >(null);

  const loadInstructors = async () => {
    setLoading(true);

    try {
      const data = await getInstructors();
      setInstructors(data);
    } catch {
      setError("Unable to load instructors.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadInstructors();
  }, []);

  const handleSubmit = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    setSaving(true);
    setError(null);

    try {
      await createInstructor({
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        password,
      });

      setFirstName("");
      setLastName("");
      setEmail("");
      setPassword("");

      await loadInstructors();
    } catch (error: unknown) {
      if (axios.isAxiosError(error)) {
        setError(
          error.response?.data?.message ??
            error.response?.data ??
            "Unable to create instructor."
        );
      } else {
        setError("Unable to create instructor.");
      }
    } finally {
      setSaving(false);
    }
  };

  const handleRemove = async (
    instructor: AdminInstructor
  ) => {
    const confirmed = window.confirm(
      `Remove ${instructor.name} ${instructor.lastName}?`
    );

    if (!confirmed) {
      return;
    }

    try {
      await removeInstructor(instructor.id);
      await loadInstructors();
    } catch {
      setError("Unable to remove instructor.");
    }
  };

  return (
    <div className="admin-page">
      <header className="admin-page__header">
        <p>ADMIN PORTAL</p>
        <h1>INSTRUCTORS</h1>
        <span>
          Register and manage GymBoo
          instructors.
        </span>
      </header>

      <form
        className="instructor-form"
        onSubmit={handleSubmit}
      >
        <input
          value={firstName}
          placeholder="First name"
          onChange={(event) =>
            setFirstName(event.target.value)
          }
          required
        />

        <input
          value={lastName}
          placeholder="Last name"
          onChange={(event) =>
            setLastName(event.target.value)
          }
          required
        />

        <input
          type="email"
          value={email}
          placeholder="Email"
          onChange={(event) =>
            setEmail(event.target.value)
          }
          required
        />

        <input
          type="password"
          value={password}
          placeholder="Temporary password"
          onChange={(event) =>
            setPassword(event.target.value)
          }
          required
        />

        <button
          type="submit"
          disabled={saving}
        >
          {saving
            ? "Creating..."
            : "Add Instructor"}
        </button>
      </form>

      {error && (
        <p className="admin-page__error">
          {error}
        </p>
      )}

      {loading ? (
        <p>Loading instructors...</p>
      ) : (
        <section className="instructor-grid">
          {instructors.map((instructor) => {
            const initials =
              `${instructor.name.charAt(0)}${instructor.lastName.charAt(0)}`.toUpperCase();

            return (
              <article
                className="instructor-admin-card"
                key={instructor.id}
              >
                <span className="instructor-admin-card__avatar">
                  {initials}
                </span>

                <div>
                  <h2>
                    {instructor.name}{" "}
                    {instructor.lastName}
                  </h2>

                  <p>{instructor.email}</p>

                  <span>
                    {instructor.isActive
                      ? "Active"
                      : "Inactive"}
                  </span>
                </div>

                <button
                  type="button"
                  onClick={() =>
                    void handleRemove(
                      instructor
                    )
                  }
                >
                  Remove
                </button>
              </article>
            );
          })}
        </section>
      )}
    </div>
  );
};

export default AdminInstructors;