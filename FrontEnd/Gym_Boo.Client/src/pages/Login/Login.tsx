import {
  useState,
  type FormEvent,
} from "react";
import {
  Navigate,
  useNavigate,
} from "react-router-dom";
import axios from "axios";

import {
  getRoleHome,
  getStoredUser,
  login,
} from "../../api/auth";

import "./Login.css";

function Login() {
  const navigate = useNavigate();
  const storedUser = getStoredUser();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [error, setError] = useState<string | null>(
    null
  );

  const [loading, setLoading] = useState(false);

  if (storedUser) {
    return (
      <Navigate
        to={getRoleHome(storedUser.role)}
        replace
      />
    );
  }

  const handleSubmit = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    setLoading(true);
    setError(null);

    try {
      const result = await login({
        email,
        password,
      });

      navigate(getRoleHome(result.user.role), {
        replace: true,
      });
    } catch (error: unknown) {
      if (axios.isAxiosError(error)) {
        setError(
          error.response?.data?.message ??
            "Unable to sign in."
        );
      } else {
        setError("An unexpected error occurred.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="login-page">
      <section className="login-hero">
        <h1>GYMBOO</h1>

        <h2>
          Train smarter.
          <span> Perform better.</span>
        </h2>
      </section>

      <section className="login-form-section">
        <form
          className="login-form"
          onSubmit={handleSubmit}
        >
          <h2>Sign In</h2>

          <label htmlFor="email">
            Email address
          </label>

          <input
            id="email"
            type="email"
            placeholder="you@example.com"
            value={email}
            onChange={(event) =>
              setEmail(event.target.value)
            }
            required
          />

          <label htmlFor="password">
            Password
          </label>

          <input
            id="password"
            type="password"
            placeholder="Enter your password"
            value={password}
            onChange={(event) =>
              setPassword(event.target.value)
            }
            required
          />

          {error && (
            <p className="login-form__error">
              {error}
            </p>
          )}

          <button
            type="submit"
            disabled={loading}
          >
            {loading
              ? "Signing In..."
              : "Sign In"}
          </button>
        </form>
      </section>
    </main>
  );
}

export default Login;