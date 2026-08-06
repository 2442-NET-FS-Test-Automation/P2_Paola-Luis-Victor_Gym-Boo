import type { AdminOverviewStats, ClassOccupancyRate, RevenueSummary, RevenueTrendPoint, TopRatedSession, TopSessionEnrollment } from "../types";
import { api } from "./client";

export interface Discipline {
  id: number;
  name: string;
  isActive?: boolean;
  isAvailable?: boolean;
  available?: boolean;
}

export const getDisciplines = async (): Promise<Discipline[]> => {
  const { data } = await api.get<Discipline[]>(
    "/api/admin/disciplines/list"
  );

  return data;
};

export const createDiscipline = async (
  name: string
): Promise<void> => {
  await api.post("/api/admin/disciplines/create", {
    name,
  });
};

export const updateDiscipline = async (
  id: number,
  name: string
): Promise<void> => {
  await api.put(`/api/admin/disciplines/${id}`, {
    name,
  });
};

export const toggleDisciplineStatus = async (
  id: number
): Promise<void> => {
  await api.patch(
    `/api/admin/disciplines/${id}/toggle-status`
  );
};

export const deleteDiscipline = async (
  name: string
): Promise<void> => {
  await api.delete("/api/admin/disciplines/delete", {
    params: {
      name,
    },
  });
};

export interface AdminInstructor {
  id: number;
  name: string;
  lastName: string;
  email: string;
  role?: string;
  isActive: boolean;
}

export interface CreateInstructorRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface UpdateInstructorRequest {
  id: number;
  name: string;
  lastName: string;
  email: string;
  role: number;
  isActive: boolean;
}

export const getInstructors = async (): Promise<
  AdminInstructor[]
> => {
  const { data } = await api.get<AdminInstructor[]>(
    "/api/admin/instructors/list"
  );

  return data;
};

export const getInstructor = async (
  id: number
): Promise<AdminInstructor> => {
  const { data } = await api.get<AdminInstructor>(
    `/api/admin/instructors/${id}`
  );

  return data;
};

export const createInstructor = async (
  request: CreateInstructorRequest
): Promise<void> => {
  await api.post(
    "/api/admin/instructors/create",
    request
  );
};

export const updateInstructor = async (
  instructor: UpdateInstructorRequest
): Promise<void> => {
  await api.put(
    `/api/admin/instructors/${instructor.id}`,
    instructor
  );
};

export const deleteInstructor = async (
  id: number
): Promise<void> => {
  await api.delete(
    `/api/admin/instructors/${id}`
  );
};

export const toggleInstructorStatus = async (
  instructor: AdminInstructor
): Promise<void> => {
  await updateInstructor({
    id: instructor.id,
    name: instructor.name,
    lastName: instructor.lastName,
    email: instructor.email,
    role: 2,
    isActive: !instructor.isActive,
  });
};

export const getRevenueSummary = async (): Promise<RevenueSummary> => {
  const { data } = await api.get<RevenueSummary>("/api/admin/reports/revenue");
  return data;
};

export const getTopSessionsByEnrollment = async (): Promise<TopSessionEnrollment[]> => {
  const { data } = await api.get<TopSessionEnrollment[]>("/api/admin/reports/sessions");
  return data;
};

export const getTopRatedSessions = async (): Promise<TopRatedSession[]> => {
  const { data } = await api.get<TopRatedSession[]>("/api/admin/reports/bestrated");
  return data;
};


// TODO: reemplazar por llamadas reales cuando existan los endpoints.
// GET /api/admin/reports/overview
// GET /api/admin/reports/revenue-trend
// GET /api/admin/reports/occupancy

const MOCK_OVERVIEW: AdminOverviewStats = {
  totalMembers: 2847,
  totalMembersChangePct: 7,
  sessionsThisMonth: 378,
  sessionsThisMonthChangePct: 6.2,
  avgOccupancyPct: 82,
  avgOccupancyChangePct: 3.1,
  monthlyRevenue: 41800,
  monthlyRevenueChangePct: 9,
};

const MOCK_REVENUE_TREND: RevenueTrendPoint[] = [
  { month: "Feb", revenue: 44000, members: 240 },
  { month: "Mar", revenue: 46500, members: 260 },
  { month: "Apr", revenue: 45200, members: 250 },
  { month: "May", revenue: 49800, members: 280 },
  { month: "Jun", revenue: 52000, members: 290 },
  { month: "Jul", revenue: 55500, members: 320 },
];

const MOCK_OCCUPANCY: ClassOccupancyRate[] = [
  { discipline: "HIIT", occupancyPct: 78 },
  { discipline: "CrossFit", occupancyPct: 90 },
  { discipline: "Cycling", occupancyPct: 96 },
  { discipline: "Kettlebell", occupancyPct: 88 },
  { discipline: "Boxing", occupancyPct: 74 },
  { discipline: "Yoga", occupancyPct: 68 },
  { discipline: "Pilates", occupancyPct: 55 },
  { discipline: "Mobility", occupancyPct: 50 },
];

export const getAdminOverviewStats = async (): Promise<AdminOverviewStats> =>
  Promise.resolve(MOCK_OVERVIEW);

export const getRevenueTrend = async (): Promise<RevenueTrendPoint[]> =>
  Promise.resolve(MOCK_REVENUE_TREND);

export const getClassOccupancyRates = async (): Promise<ClassOccupancyRate[]> =>
  Promise.resolve(MOCK_OCCUPANCY);
