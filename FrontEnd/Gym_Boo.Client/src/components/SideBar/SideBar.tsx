import {
  NavLink,
  useNavigate,
} from "react-router-dom";

import { LogOut } from "lucide-react";

import { logout } from "../../api/auth";
import { ROLE_CONFIGS } from "./roleConfig";
import { useCurrentUser } from "./useCurrentUser";
import Logo from "../Logo/Logo";

import "./Sidebar.css";

const Sidebar = () => {
  const navigate = useNavigate();
  const user = useCurrentUser();

  const config = ROLE_CONFIGS[user.role as keyof typeof ROLE_CONFIGS];

  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <aside className="sidebar">
      <div className="sidebar__logo">
        <Logo size={28} />
        <span className="sidebar__logo-text">GYMBOO</span>
      </div>

      <p className="sidebar__section-label">
        {config.portalLabel}
      </p>

      <nav className="sidebar__nav">
        {config.links.map((link) => {
          const Icon = link.icon;

          return (
            <NavLink
              key={link.to}
              to={link.to}
              className={({ isActive }) =>
                `sidebar__nav-link ${
                  isActive ? "is-active" : ""
                }`
              }
            >
              <Icon size={18} />
              <span>{link.label}</span>
            </NavLink>
          );
        })}
      </nav>

      <div className="sidebar__footer">
        <div className="sidebar__user">
          <span className="sidebar__avatar">
            {user.initials}
          </span>

          <div>
            <p className="sidebar__user-name">
              {user.name} {user.lastName}
            </p>

            <p className="sidebar__user-role">
              {user.role.toUpperCase()}
            </p>
          </div>
        </div>

        <button
          type="button"
          className="sidebar__logout"
          aria-label="Log out"
          onClick={handleLogout}
        >
          <LogOut size={18} />
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;