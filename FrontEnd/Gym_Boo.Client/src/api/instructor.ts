import { api } from "./client";

export interface AttendanceRecord {
  id?: number;
  enrollmentId?: number;
  memberId?: number;
  memberName?: string;
  name?: string;
  memberEmail?: string;
  email?: string;
  isPresent?: boolean;
  attended?: boolean;
}

export const getAttendance = async (
  sessionId: number
): Promise<AttendanceRecord[]> => {
  const { data } = await api.get<
    AttendanceRecord[]
  >(
    `/api/instructor/sessions/${sessionId}/attendance`
  );

  return data;
};