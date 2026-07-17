# 👻 GymBoo Fullstack Application
GymBoo is a full-stack web application designed to simplify gym management by connecting members, instructors, and administrators. Members can use the platform to create an account, browse scheduled classes using specific filters, book available spots, and leave ratings for instructors. Instructors get their own dashboard to manage class templates, set their own availability, and view a digital list of registered members for roll call. Administrators have full control to manage the gym's sports catalog, register or disable instructors, and view business reports that track metrics like class popularity, peak hours, and total revenue.

## 1. Project Overview

### User Stories
This section describes the functional requirements of the Gym Management Platform using user stories organized by epic.

---

# Epic: Authentication & Security

<details>
<summary><strong>US-01 · Member Registration</strong></summary>

### 👤 User Story
**As a** visitor,  
**I want** to create an account,  
**So that** I can reserve gym classes.

### ✅ Acceptance Criteria

- Registration form requests:
  - Name
  - Email
  - Password
- API creates a **Member** account.
- Password is stored using **Hash + Salt**.
- Duplicate email returns:
  - `400 Bad Request`
  - Friendly error message.
- Public users cannot register as **Administrator** or **Instructor**.

</details>

---

<details>
<summary><strong>US-02 · Login & Logout</strong></summary>

### 👤 User Story
**As a** Member, Instructor, or Administrator,  
**I want** to log in securely,  
**So that** I can access features according to my role.

### ✅ Acceptance Criteria

- Successful login returns a JWT.
- JWT is stored in React Context / LocalStorage.
- Axios automatically includes:

```http
Authorization: Bearer <token>
```

- Invalid credentials return:

```
401 Unauthorized
```

- Logout removes the token and redirects to the public page.

</details>

---

# Epic: Member Area

<details>
<summary><strong>US-03 · Browse Available Classes</strong></summary>

### 👤 User Story
**As a** member,  
**I want** to browse and filter available classes,  
**So that** I can schedule my workouts.

### ✅ Acceptance Criteria

- Calendar or grid view.
- Server-side filtering.

```http
GET /api/classes?discipline=yoga&date=2026-07-15
```

Each class card displays:

- Class name
- Instructor
- Schedule
- Location
- Available spots

Example:

```
12 / 20 spots available
```

</details>

---

<details>
<summary><strong>US-04 · Reserve a Class</strong></summary>

### 👤 User Story
**As a** member,  
**I want** to reserve a class,  
**So that** I can secure my attendance.

### ✅ Acceptance Criteria

- Clicking **Reserve** sends:

```http
POST /api/reservations
```

- Backend validates:
  - Available capacity.
  - No duplicate reservation at the same time.
- Success returns:

```
201 Created
```

- Available capacity decreases by one.

</details>

---

<details>
<summary><strong>US-05 · Cancel Reservation & Late Cancellation Penalty</strong></summary>

### 👤 User Story
**As a** member,  
**I want** to cancel my reservation,  
**So that** another member can take my spot.

### ✅ Acceptance Criteria

- "My Reservations" includes **Cancel Reservation**.
- Backend checks the 2-hour rule.

If more than 2 hours remain:

- Reservation is cancelled.
- Status: Free Cancellation.

If less than 2 hours remain:

- Reservation is cancelled.
- A penalty is recorded.
- UI displays a confirmation warning.

</details>

---

<details>
<summary><strong>US-06 · Reservation History</strong></summary>

### 👤 User Story
**As a** member,  
**I want** to view my reservation history,  
**So that** I can track my attendance.

### ✅ Acceptance Criteria

- "My Reservations" lists:
  - Upcoming reservations
  - Past reservations
- Backend retrieves the user exclusively from the JWT claims.

</details>

---

<details>
<summary><strong>US-07 · Instructor Reviews ⭐ (Team Feature)</strong></summary>

### 👤 User Story
**As a** member,  
**I want** to review instructors after attending their classes,  
**So that** I can share my experience.

### ✅ Acceptance Criteria

- Reviews are only available for attended classes.
- Members submit:
  - Rating (1–5 stars)
  - Short comment

```http
POST /api/instructors/{id}/reviews
```

- API validates attendance before saving.

</details>

---

# Epic: Instructor Area

<details>
<summary><strong>US-08 · Instructor Dashboard</strong></summary>

### 👤 User Story
**As an** instructor,  
**I want** a dashboard with my upcoming classes and ratings,  
**So that** I can organize my schedule.

### ✅ Acceptance Criteria

Dashboard includes:

- Instructor profile
- Photo
- Description
- Average rating
- Upcoming classes

Only users with the **Instructor** role can access it.

</details>

---

<details>
<summary><strong>US-09 · Manage Class Schedule ⭐ (Team Feature)</strong></summary>

### 👤 User Story
**As an** instructor,  
**I want** to create and manage my own class sessions,  
**So that** I can organize my availability.

### ✅ Acceptance Criteria

Instructor can:

- Create a new session

```http
POST /api/instructor/classes
```

- Select:
  - Class template
  - Date
  - Time
  - Capacity
- System prevents overlapping schedules.
- Instructor defines late cancellation fee.

</details>

---

<details>
<summary><strong>US-10 · View Class Attendance</strong></summary>

### 👤 User Story
**As an** instructor,  
**I want** to see the members registered for my classes,  
**So that** I can take attendance.

### ✅ Acceptance Criteria

Attendance list displays:

- Member names
- Member emails

Endpoint:

```http
GET /api/instructor/classes/{id}/attendance
```

Accessible only by:

- Assigned instructor
- Administrator

Unauthorized users receive:

```
403 Forbidden
```

</details>

---

# Epic: Administration

<details>
<summary><strong>US-11 · Manage Class Catalog</strong></summary>

### 👤 User Story
**As an** administrator,  
**I want** to manage the gym class catalog,  
**So that** instructors can schedule classes.

### ✅ Acceptance Criteria

Administrator can:

- Create disciplines
- Edit disciplines
- Delete disciplines

Examples:

- Yoga
- CrossFit
- Spinning

Uses standard REST verbs:

- POST
- PUT
- DELETE

</details>

---

<details>
<summary><strong>US-12 · Manage Instructors</strong></summary>

### 👤 User Story
**As an** administrator,  
**I want** to manage instructors,  
**So that** I can maintain the active staff.

### ✅ Acceptance Criteria

Administrator can:

- Register instructors
- Update instructor information
- Soft delete instructors

Disabled instructors:

- Cannot log in.
- Cannot receive new reservations.

</details>

---

<details>
<summary><strong>US-13 · Occupancy & Revenue Reports</strong></summary>

### 👤 User Story
**As an** administrator,  
**I want** occupancy and revenue reports,  
**So that** I can evaluate gym performance.

### ✅ Acceptance Criteria

Dashboard consumes:

```http
GET /api/reports/occupancy
```

Displays:

- Most popular classes
- Peak reservation hours
- Total class revenue
- Total late cancellation penalties

</details>
