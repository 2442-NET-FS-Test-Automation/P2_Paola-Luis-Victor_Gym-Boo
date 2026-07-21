import "./Login.css";

function Login() {
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
        <form className="login-form">
          <h2>Sign In</h2>

          <label htmlFor="email">Email address</label>
          <input
            id="email"
            type="email"
            placeholder="you@example.com"
          />

          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            placeholder="Enter your password"
          />

          <button type="submit">Sign In</button>
        </form>
      </section>
    </main>
  );
}

export default Login;