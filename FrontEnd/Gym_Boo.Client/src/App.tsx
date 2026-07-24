import {
  Navigate,
  Route,
  Routes,
} from "react-router-dom";

import Login from "./pages/Login/Login";
import Layout from "./components/Layout/Layout";
import ProtectedRoute from "./components/Auth/ProtectedRoute";

import Discover from "./pages/Discover/Discover";

import InstructorDashboard from "./pages/Instructor/Dashboard/InstructorDashboard";
import InstructorSchedule from "./pages/Instructor/Schedule/InstructorSchedule";
import InstructorAttendance from "./pages/Instructor/Attendance/InstructorAttendance";

import AdminCatalog from "./pages/Admin/Catalog/AdminCatalog";
import AdminSessions from "./pages/Admin/Sessions/AdminSessions";
import AdminInstructors from "./pages/Admin/Instructors/AdminInstructors";

function App() {
  return (
    <Routes>
      <Route
        path="/login"
        element={<Login />}
      />

      <Route
        element={
          <ProtectedRoute
            allowedRoles={["Member"]}
          />
        }
      >
        <Route element={<Layout />}>
          <Route
            path="/member/discover"
            element={<Discover />}
          />

          <Route
            path="/member/bookings"
            element={<div>My Bookings</div>}
          />

          <Route
            path="/member/profile"
            element={<div>Profile</div>}
          />

          <Route
            path="/member/review"
            element={<div>Leave a Review</div>}
          />
        </Route>
      </Route>

      <Route
        element={
          <ProtectedRoute
            allowedRoles={["Instructor"]}
          />
        }
      >
        <Route element={<Layout />}>
          <Route
            path="/coach/dashboard"
            element={<InstructorDashboard />}
          />

          <Route
            path="/coach/schedule"
            element={<InstructorSchedule />}
          />

          <Route
            path="/coach/attendance"
            element={<InstructorAttendance />}
          />
        </Route>
      </Route>

      <Route
        element={
          <ProtectedRoute
            allowedRoles={["Admin"]}
          />
        }
      >
        <Route element={<Layout />}>
          <Route
            path="/admin/catalog"
            element={<AdminCatalog />}
          />

          <Route
            path="/admin/sessions"
            element={<AdminSessions />}
          />

          <Route
            path="/admin/instructors"
            element={<AdminInstructors />}
          />
        </Route>
      </Route>

      <Route
        path="/"
        element={
          <Navigate
            to="/login"
            replace
          />
        }
      />

      <Route
        path="*"
        element={
          <Navigate
            to="/login"
            replace
          />
        }
      />
    </Routes>
  );
}

export default App;