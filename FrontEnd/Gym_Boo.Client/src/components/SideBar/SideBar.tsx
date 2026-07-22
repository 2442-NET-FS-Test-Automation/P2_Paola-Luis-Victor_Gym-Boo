import { NavLink, useNavigate } from "react-router-dom";
import { LogOut } from "lucide-react";
import { ROLES, useActiveRole } from "./roleConfig";
import { useCurrentUser } from "./useCurrentUser";
import "./Sidebar.css";

const Sidebar = () => {
    const navigate = useNavigate();
    const activeRole = useActiveRole();
    const user = useCurrentUser();

    return (
        <aside className="sidebar">
            <div className="sidebar__logo">
                <span className="sidebar__logo-mark">G</span>
                <span className="sidebar__logo-text">GYMBOO</span>
            </div>

            <div className="sidebar__tabs">
                {ROLES.map((role) => (
                    <button
                        key={role.key}
                        type="button"
                        className={`sidebar__tab ${role.key === activeRole.key ? "is-active" : ""
                            }`}
                        onClick={() => navigate(role.links[0].to)}
                    >
                        {role.tabLabel}
                    </button>
                ))}
            </div>

            <p className="sidebar__section-label">{activeRole.portalLabel}</p>

            <nav className="sidebar__nav">
                {activeRole.links.map((link) => {
                    const Icon = link.icon;
                    return (
                        <NavLink
                            key={link.to}
                            to={link.to}
                            className={({ isActive }) =>
                                `sidebar__nav-link ${isActive ? "is-active" : ""}`
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
                    <span className="sidebar__avatar">{user.initials}</span>
                    <div>
                        <p className="sidebar__user-name">{user.name}</p>
                        <p className="sidebar__user-role">{user.role}</p>
                    </div>
                </div>
                <button
                    type="button"
                    className="sidebar__logout"
                    aria-label="Log out"
                    onClick={() => {
                        // TODO: conectar a logout real cuando exista auth
                    }}
                >
                    <LogOut size={18} />
                </button>
            </div>
        </aside>
    );
};

export default Sidebar;