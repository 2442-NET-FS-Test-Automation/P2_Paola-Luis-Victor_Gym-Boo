import { useLocation } from "react-router-dom";
import {
    LayoutGrid,
    Calendar,
    User,
    Star,
    ClipboardCheck,
    BarChart3,
    BookOpen,
    Users,
} from "lucide-react";
import type { ComponentType } from "react";

export type RoleKey = "member" | "coach" | "admin";

export interface NavLinkConfig {
    label: string;
    to: string;
    icon: ComponentType<{ size?: number }>;
}

export interface RoleConfig {
    key: RoleKey;
    tabLabel: string;
    portalLabel: string;
    basePath: string;
    links: NavLinkConfig[];
}

export const ROLES: RoleConfig[] = [
    {
        key: "member",
        tabLabel: "MEMBER",
        portalLabel: "MEMBER PORTAL",
        basePath: "/member",
        links: [
            { label: "Discover", to: "/member/discover", icon: LayoutGrid },
            { label: "My Bookings", to: "/member/bookings", icon: Calendar },
            { label: "Profile", to: "/member/profile", icon: User },
            { label: "Leave a Review", to: "/member/review", icon: Star },
        ],
    },
    {
        key: "coach",
        tabLabel: "COACH",
        portalLabel: "COACH PORTAL",
        basePath: "/coach",
        links: [
            { label: "Dashboard", to: "/coach/dashboard", icon: LayoutGrid },
            { label: "My Schedule", to: "/coach/schedule", icon: Calendar },
            { label: "Attendance", to: "/coach/attendance", icon: ClipboardCheck },
        ],
    },
    {
        key: "admin",
        tabLabel: "ADMIN",
        portalLabel: "ADMIN PORTAL",
        basePath: "/admin",
        links: [
            { label: "Analytics", to: "/admin/analytics", icon: BarChart3 },
            { label: "Class Catalog", to: "/admin/catalog", icon: BookOpen },
            { label: "Sessions", to: "/admin/sessions", icon: Calendar },
            { label: "Instructors", to: "/admin/instructors", icon: Users },
        ],
    },
];

export const getRoleByPath = (pathname: string): RoleConfig =>
    ROLES.find((r) => pathname.startsWith(r.basePath)) ?? ROLES[0];

export const useActiveRole = (): RoleConfig => {
    const { pathname } = useLocation();
    return getRoleByPath(pathname);
};