import {
  useState,
  type FormEvent,
} from "react";

import {
  Link,
  useNavigate,
} from "react-router-dom";

import axios from "axios";
import { ArrowLeft, ArrowRight } from "lucide-react";

import { register } from "../../api/auth";

import "./Register.css";

const getRegisterError = (
  error: unknown
): string => {
  if (!axios.isAxiosError(error)) {
    return "An unexpected error occurred.";
  }

  const data = error.response?.data;

  if (typeof data === "string") {
    return data;
  }

  if (data?.errors) {
    return Object.values(data.errors)
      .flat()
      .join(" ");
  }

  return (
    data?.message ??
    "The account could not be created."
  );
};

function Register() {
  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [lastName, setLastName] =
    useState("");

  const [email, setEmail] = useState("");
  const [password, setPassword] =
    useState("");

  const [
    confirmPassword,
    setConfirmPassword,
  ] = useState("");

  const [error, setError] = useState<
    string | null
  >(null);

  const [loading, setLoading] =
    useState(false);

  const [registrationSuccessful, setRegistrationSuccessful] =
    useState(false);

  const handleSubmit = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();
    setError(null);

    if (password.length < 8) {
      setError(
        "Password must contain at least 8 characters."
      );
      return;
    }

    if (password !== confirmPassword) {
      setError(
        "The passwords do not match."
      );
      return;
    }

    setLoading(true);

    try {
      await register({
        name: name.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        password,
        });

        setRegistrationSuccessful(true);
      
    } catch (error: unknown) {
      setError(getRegisterError(error));
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="register-page">
      <div className="register-container">
        <header className="register-topbar">
          <div className="register-logo">
            <span>G</span>
            <strong>GYMBOO</strong>
          </div>

          <Link
            to="/login"
            className="register-back"
          >
            <ArrowLeft size={16} />
            Back to Sign In
          </Link>
        </header>

        <section className="register-card">
          <header className="register-card__header">
            <h1>CREATE ACCOUNT</h1>

            <p>
              Your journey starts here. Fill in
              your credentials.
            </p>
          </header>

          <form
            className="register-form"
            onSubmit={handleSubmit}
          >
            <div className="register-name-grid">
              <div>
                <label htmlFor="name">
                  First name
                </label>

                <input
                  id="name"
                  value={name}
                  placeholder="First name"
                  onChange={(event) =>
                    setName(
                      event.target.value
                    )
                  }
                  required
                />
              </div>

              <div>
                <label htmlFor="lastName">
                  Last name
                </label>

                <input
                  id="lastName"
                  value={lastName}
                  placeholder="Last name"
                  onChange={(event) =>
                    setLastName(
                      event.target.value
                    )
                  }
                  required
                />
              </div>
            </div>

            <label htmlFor="register-email">
              Email address
            </label>

            <input
              id="register-email"
              type="email"
              value={email}
              placeholder="you@example.com"
              onChange={(event) =>
                setEmail(event.target.value)
              }
              required
            />

            <label htmlFor="register-password">
              Password
            </label>

            <input
              id="register-password"
              type="password"
              value={password}
              placeholder="Min. 8 characters"
              minLength={8}
              onChange={(event) =>
                setPassword(
                  event.target.value
                )
              }
              required
            />

            <label htmlFor="confirm-password">
              Confirm password
            </label>

            <input
              id="confirm-password"
              type="password"
              value={confirmPassword}
              placeholder="Re-enter your password"
              minLength={8}
              onChange={(event) =>
                setConfirmPassword(
                  event.target.value
                )
              }
              required
            />

            {error && (
              <p className="register-error">
                {error}
              </p>
            )}

            <div className="register-actions">
              <button
                type="submit"
                disabled={loading}
              >
                {loading
                  ? "Creating..."
                  : "Create account"}

                {!loading && (
                  <ArrowRight size={19} />
                )}
              </button>
            </div>
          </form>
        </section>

        {registrationSuccessful && (
            <div className="register-success-backdrop">
                <section
                className="register-success-modal"
                role="dialog"
                aria-modal="true"
                aria-labelledby="register-success-title"
                >
                <div className="register-success-icon">
                    ✓
                </div>

                <h2 id="register-success-title">
                    Registration successful!
                </h2>

                <p>
                    Your GymBoo account has been created.
                    You can now sign in using your email and password.
                </p>

                <button
                    type="button"
                    onClick={() =>
                    navigate("/login", { replace: true })
                    }
                >
                    Go to sign in
                </button>
                </section>
            </div>
            )}
      </div>
    </main>
  );
}

export default Register;