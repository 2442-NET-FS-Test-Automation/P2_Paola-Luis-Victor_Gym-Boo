import {
  Navigate,
  Route,
  Routes,
} from "react-router-dom";

import Login from "./pages/Login/Login";
import Layout from "./components/Layout/Layout";
import ProtectedRoute from "./components/Auth/ProtectedRoute";
import Register from "./pages/Register/Register";

import Discover from "./pages/Discover/Discover";
import ClassDetail from "./pages/ClassDetail/ClassDetail";
import MyBookings from "./pages/MyBookings/MyBookings";
import LeaveReview from "./components/LeaveReview/LeaveReview";
import MyProfile from "./pages/MyProfile/MyProfile";

import InstructorDashboard from "./pages/Instructor/Dashboard/InstructorDashboard";
import InstructorSchedule from "./pages/Instructor/Schedule/InstructorSchedule";
import InstructorAttendance from "./pages/Instructor/Attendance/InstructorAttendance";

import AdminCatalog from "./pages/Admin/Catalog/AdminCatalog";
import AdminSessions from "./pages/Admin/Sessions/AdminSessions";
import AdminInstructors from "./pages/Admin/Instructors/AdminInstructors";
import ReviewLanding from "./pages/ReviewLanding/ReviewLanding";

function App() {
  return (
    <Routes>
      <Route
        path="/login"
        element={<Login />}
      />

      <Route
        path="/register"
        element={<Register />}
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
            path="/member/discover/:id"
            element={<ClassDetail />}
          />

          <Route
            path="/member/bookings"
            element={<MyBookings />}
          />

          <Route
            path="/member/profile"
            element={<MyProfile />}
          />

          <Route
            path="/member/review"
            element={<ReviewLanding />}
          />

          <Route path="/member/discover/:id" element={<ClassDetail />} />
          <Route
            path="/member/review/:enrollmentId/:sessionId"
            element={<LeaveReview />}
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
