describe("Auth flow", () => {
  beforeEach(() => {
    cy.clearLocalStorage();
  });

  it("renders the login page", () => {
    cy.visit("/login");

    cy.contains("GYMBOO").should("be.visible");
    cy.contains("Sign In").should("be.visible");
    cy.get("#email").should("be.visible");
    cy.get("#password").should("be.visible");
  });

  it("shows an error for invalid login", () => {
    cy.intercept("POST", "**/api/auth/login", {
      statusCode: 401,
      body: { message: "Invalid email or password." },
    });

    cy.visit("/login");
    cy.get("#email").type("wrong@test.com");
    cy.get("#password").type("badpassword");
    cy.contains("button", "Sign In").click();

    cy.contains("Invalid email or password.").should("be.visible");
  });

  it("redirects a member after successful login", () => {
    cy.intercept("POST", "**/api/auth/login", {
      statusCode: 200,
      body: {
        token: "fake-member-token",
        user: {
          id: 1,
          name: "Paola",
          lastName: "Test",
          email: "paola@test.com",
          role: "Member",
        },
      },
    });

    cy.intercept("GET", "**/api/classes**", {
      statusCode: 200,
      body: [],
    });

    cy.visit("/login");
    cy.get("#email").type("paola@test.com");
    cy.get("#password").type("Password123");
    cy.contains("button", "Sign In").click();

    cy.location("pathname").should("eq", "/member/discover");
  });

  it("redirects unauthenticated users to login", () => {
    cy.visit("/member/discover");

    cy.location("pathname").should("eq", "/login");
  });

  it("rejects mismatched registration passwords", () => {
    cy.visit("/register");

    cy.get("#name").type("Paola");
    cy.get("#lastName").type("Test");
    cy.get("#register-email").type("paola@test.com");
    cy.get("#register-password").type("Password123");
    cy.get("#confirm-password").type("Password456");
    cy.contains("button", "Create account").click();

    cy.contains("The passwords do not match.").should("be.visible");
  });
});