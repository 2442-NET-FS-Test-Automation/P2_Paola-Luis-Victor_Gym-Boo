import { api } from "./client";

export interface Discipline {
  id: number;
  name: string;
  isActive?: boolean;
  available?: boolean;
}

export interface AdminInstructor {
  id: number;
  name: string;
  firstName?: string;
  lastName: string;
  email: string;
  isActive: boolean;
}

export interface CreateInstructorRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export const getDisciplines = async (): Promise<
  Discipline[]
> => {
  const { data } = await api.get<Discipline[]>(
    "/api/admin/disciplines/list"
  );

  return data;
};

export const createDiscipline = async (
  name: string
): Promise<void> => {
  await api.post(
    "/api/admin/disciplines/create",
    { name }
  );
};

export const updateDiscipline = async (
  id: number,
  name: string
): Promise<void> => {
  await api.put(
    `/api/admin/disciplines/${id}`,
    { name }
  );
};

export const toggleDiscipline = async (
  id: number
): Promise<void> => {
  await api.patch(
    `/api/admin/disciplines/${id}/toggle-status`
  );
};

export const deleteDiscipline = async (
  name: string
): Promise<void> => {
  await api.delete(
    "/api/admin/disciplines/delete",
    {
      params: { name },
    }
  );
};

export const getInstructors = async (): Promise<
  AdminInstructor[]
> => {
  const { data } = await api.get<AdminInstructor[]>(
    "/api/admin/instructors/list"
  );

  return data;
};

export const createInstructor = async (
  instructor: CreateInstructorRequest
): Promise<AdminInstructor> => {
  const { data } = await api.post<AdminInstructor>(
    "/api/admin/instructors/create",
    instructor
  );

  return data;
};

export const removeInstructor = async (
  id: number
): Promise<void> => {
  await api.delete(
    `/api/admin/instructors/${id}`
  );
};