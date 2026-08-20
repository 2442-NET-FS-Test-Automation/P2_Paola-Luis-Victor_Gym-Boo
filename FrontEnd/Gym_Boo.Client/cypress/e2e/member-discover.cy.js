describe("Member discover", () => {
  beforeEach(() => {
    cy.clearLocalStorage();

    cy.window().then((win) => {
      win.localStorage.setItem("gymboo_token", "fake-token");
      win.localStorage.setItem(
        "gymboo_user",
        JSON.stringify({
          id: 1,
          name: "Paola",
          lastName: "Test",
          email: "paola@test.com",
          role: "Member",
        })
      );
    });
  });

  it("loads available classes", () => {
    cy.intercept("GET", "**/api/classes**", {
      statusCode: 200,
      body: [
        {
          id: 101,
          className: "Yoga Flow",
          discipline: "Yoga",
          instructorName: "Alex Rivera",
          instructorRating: 4.8,
          startTime: "2030-01-01T10:00:00Z",
          endTime: "2030-01-01T11:00:00Z",
          location: "Studio A",
          capacity: 20,
          availableSpots: 8,
          totalSpots: 20,
        },
      ],
    });

    cy.visit("/member/discover");

    cy.contains("Discover Classes").should("be.visible");
    cy.contains("Power Yoga").should("be.visible");
    cy.contains("Emily Davis").should("be.visible");
  });
});
