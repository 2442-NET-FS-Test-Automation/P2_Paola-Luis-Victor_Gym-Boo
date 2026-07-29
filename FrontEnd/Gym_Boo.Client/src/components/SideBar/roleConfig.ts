import {
  LayoutGrid,
  Calendar,
  User,
  Star,
  ClipboardCheck,
  BookOpen,
  Users,
  ChartPie
} from "lucide-react";

import type { ComponentType } from "react";
import type { UserRole } from "../../types";

export interface NavLinkConfig {
  label: string;
  to: string;
  icon: ComponentType<{ size?: number }>;
}

export interface RoleConfig {
  portalLabel: string;
  links: NavLinkConfig[];
}

export const ROLE_CONFIGS: Record<
  UserRole,
  RoleConfig
> = {
  Member: {
    portalLabel: "MEMBER PORTAL",
    links: [
      {
        label: "Discover",
        to: "/member/discover",
        icon: LayoutGrid,
      },
      {
        label: "My Bookings",
        to: "/member/bookings",
        icon: Calendar,
      },
      {
        label: "Profile",
        to: "/member/profile",
        icon: User,
      },
      {
        label: "Leave a Review",
        to: "/member/review",
        icon: Star,
      },
    ],
  },

  Instructor: {
    portalLabel: "INSTRUCTOR PORTAL",
    links: [
      {
        label: "Dashboard",
        to: "/coach/dashboard",
        icon: LayoutGrid,
      },
      {
        label: "My Schedule",
        to: "/coach/schedule",
        icon: Calendar,
      },
      {
        label: "Attendance",
        to: "/coach/attendance",
        icon: ClipboardCheck,
      },
    ],
  },

  Admin: {
    portalLabel: "ADMIN PORTAL",
    links: [
      {
        label: "Analytics",
        to: "admin/analytics",
        icon: ChartPie
      },
      {
        label: "Class Catalog",
        to: "/admin/catalog",
        icon: BookOpen,
      },
      {
        label: "Sessions",
        to: "/admin/sessions",
        icon: Calendar,
      },
      {
        label: "Instructors",
        to: "/admin/instructors",
        icon: Users,
      },
    ],
  },
};