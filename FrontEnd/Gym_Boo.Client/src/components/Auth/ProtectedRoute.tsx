import {
  Navigate,
  Outlet,
  useLocation,
} from "react-router-dom";

import {
  getRoleHome,
  getStoredUser,
  isAuthenticated,
} from "../../api/auth";

import type { UserRole } from "../../types";

interface ProtectedRouteProps {
  allowedRoles: UserRole[];
}

const ProtectedRoute = ({
  allowedRoles,
}: ProtectedRouteProps) => {
  const location = useLocation();
  const user = getStoredUser();

  if (!isAuthenticated() || !user) {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: location.pathname }}
      />
    );
  }

  if (!allowedRoles.includes(user.role)) {
    return (
      <Navigate
        to={getRoleHome(user.role)}
        replace
      />
    );
  }

  return <Outlet />;
};

export default ProtectedRoute;