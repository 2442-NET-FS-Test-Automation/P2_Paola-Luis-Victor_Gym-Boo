export interface ApiClassSession {
    id: number;
    className: string;
    discipline: string;
    instructorName: string;
    instructorRating: number;
    startTime: string; // ISO
    endTime: string; // ISO
    location: string;
    availableSpots: number;
    totalSpots: number;
}

export interface ClassFilters {
    discipline?: string;
    date?: string; // ISO date-time
    past?: boolean;
}

export interface DateOption {
    label: string;
    value: string; // "YYYY-MM-DD"
}

export type UserRole = "Member" | "Instructor" | "Admin";

export interface AuthUser {
  id: number;
  name: string;
  lastName: string;
  email: string;
  role: UserRole;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  message: string;
  token: string;
  user: AuthUser;
}

export interface CurrentUser {
  id: number;
  name: string;
  lastName: string;
  email: string;
  role: UserRole;
  initials: string;
}