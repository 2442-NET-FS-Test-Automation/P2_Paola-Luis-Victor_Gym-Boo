import { api } from "./client";
import type {
  AuthUser,
  LoginRequest,
  LoginResponse,
  UserRole,
} from "../types";

const TOKEN_KEY = "gymboo_token";
const USER_KEY = "gymboo_user";

export const login = async (
  credentials: LoginRequest
): Promise<LoginResponse> => {
  const { data } = await api.post<LoginResponse>(
    "/api/auth/login",
    credentials
  );

  localStorage.setItem(TOKEN_KEY, data.token);
  localStorage.setItem(USER_KEY, JSON.stringify(data.user));

  return data;
};

export const logout = (): void => {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
};

export const getStoredToken = (): string | null => {
  return localStorage.getItem(TOKEN_KEY);
};

export const getStoredUser = (): AuthUser | null => {
  const storedUser = localStorage.getItem(USER_KEY);

  if (!storedUser) {
    return null;
  }

  try {
    return JSON.parse(storedUser) as AuthUser;
  } catch {
    logout();
    return null;
  }
};

export const isAuthenticated = (): boolean => {
  return Boolean(getStoredToken() && getStoredUser());
};

export const getRoleHome = (role: UserRole): string => {
  switch (role) {
    case "Admin":
      return "/admin/catalog";

    case "Instructor":
      return "/coach/dashboard";

    case "Member":
    default:
      return "/member/discover";
  }
};