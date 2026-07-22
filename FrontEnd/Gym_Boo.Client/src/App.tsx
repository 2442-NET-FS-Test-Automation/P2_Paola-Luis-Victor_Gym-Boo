import { Navigate, Route, Routes } from "react-router-dom";
import Login from "./pages/Login/Login";
import Layout from "./components/Layout/Layout";
import Discover from "./pages/Discover/Discover";

function App() {
  return (
    <main>
      <Routes>
        <Route path="/login" element={<Login />} />

        <Route element={<Layout />}>
          <Route path="/member/discover" element={<Discover />} />
          <Route path="/member/bookings" element={<div>My Bookings — coming soon</div>} />
          <Route path="/member/profile" element={<div>Profile — coming soon</div>} />
          <Route path="/member/review" element={<div>Leave a Review — coming soon</div>} />

          <Route path="/coach/dashboard" element={<div>Coach Dashboard — coming soon</div>} />
          <Route path="/coach/schedule" element={<div>My Schedule — coming soon</div>} />
          <Route path="/coach/attendance" element={<div>Attendance — coming soon</div>} />

          <Route path="/admin/analytics" element={<div>Analytics — coming soon</div>} />
          <Route path="/admin/catalog" element={<div>Class Catalog — coming soon</div>} />
          <Route path="/admin/sessions" element={<div>Sessions — coming soon</div>} />
          <Route path="/admin/instructors" element={<div>Instructors — coming soon</div>} />
        </Route>

        <Route path="/" element={<Navigate to="/member/discover" replace />} />
      </Routes>
    </main>
  );
}

export default App;