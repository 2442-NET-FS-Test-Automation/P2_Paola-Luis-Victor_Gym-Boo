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

// ==================== REPORTS ====================

export interface SessionReport {
  disciplineName: string;
  totalEnrollments: number;
}

export interface RevenueReport {
  cancellationRevenue: number;
  subscriptionRevenue: number;
  totalRevenue: number;
}

export interface BestRatedReport {
  id: number;
  className: string;
  instructorName: string;
  averageRating: number;
}

interface BackendSessionReport {
  DisciplineName?: string;
  disciplineName?: string;
  TotalEnrollments?: number;
  totalEnrollments?: number;
}

interface BackendBestRatedReport {
  Id?: number;
  id?: number;
  ClassName?: string;
  className?: string;
  InstructorName?: string;
  instructorName?: string;
  AverageRating?: number;
  averageRating?: number;
}

// Gets the session registration report.
export const getSessionReports = async (): Promise<
  SessionReport[]
> => {
  const response = await api.get<BackendSessionReport[]>(
    "/api/admin/reports/sessions"
  );

  return response.data.map((report) => ({
    disciplineName:
      report.disciplineName ??
      report.DisciplineName ??
      "Unknown",
    totalEnrollments:
      report.totalEnrollments ??
      report.TotalEnrollments ??
      0,
  }));
};

// Gets cancellation, subscription, and total revenue.
export const getRevenueReport =
  async (): Promise<RevenueReport> => {
    const response = await api.get<RevenueReport>(
      "/api/admin/reports/revenue"
    );

    return response.data;
  };

// Gets the best-rated or most popular classes.
export const getBestRatedReport = async (): Promise<
  BestRatedReport[]
> => {
  const response = await api.get<BackendBestRatedReport[]>(
    "/api/admin/reports/bestrated"
  );

  return response.data.map((report) => ({
    id: report.id ?? report.Id ?? 0,
    className:
      report.className ??
      report.ClassName ??
      "Unknown class",
    instructorName:
      report.instructorName ??
      report.InstructorName ??
      "Unknown instructor",
    averageRating:
      report.averageRating ??
      report.AverageRating ??
      0,
  }));
};
