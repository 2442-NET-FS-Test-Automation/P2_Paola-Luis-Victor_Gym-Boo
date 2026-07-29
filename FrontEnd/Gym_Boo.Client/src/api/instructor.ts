import axios from "axios";
import { api } from "./client";

export interface InstructorDetails {
  id: number;
  name: string;
  lastName: string;
  email: string;
  isActive: boolean;
  averageRating?: number;
}

export interface UpcomingSessionDto {
  sessionId: number;
  className: string;
  placeName: string;
  startTime: string;
  endTime: string;
  availableSlots: number;
}

const parseBackendDate = (value: string): string => {
  return /Z$|[+-]\d{2}:\d{2}$/.test(value)
    ? value
    : `${value}Z`;
};

export interface InstructorSession {
  id: number;
  className: string;
  startTime: string;
  endTime: string;
  totalSpots: number;
  availableSpots: number;
  enrolledSpots: number;
  placeName?: string;
  location?: string;
}

export interface SubscriberDto {
  id: number;
  Id?: number;
  email: string;
  Email?: string;
}

export interface SessionAttendanceResponseDto {
  sessionId: number;
  SessionId?: number;
  totalEnrolled: number;
  TotalEnrolled?: number;
  subscribers: SubscriberDto[];
  Subscribers?: SubscriberDto[];
}

export interface CreateSessionRequest {
  startTime: string;
  endTime: string;
  slots: number;
  cancellationFee: number;
  classId: number;
  instructorId: number;
  placeId: number;
}

export interface AttendanceRecord {
  id?: number;
  enrollmentId?: number;
  memberId?: number;
  memberName?: string;
  name?: string;
  lastName?: string;
  memberEmail?: string;
  email?: string;
  isPresent?: boolean;
  attended?: boolean;
  bookedAt?: string;
  createdAt?: string;
}

export interface ClassOption {
  id: number;
  name: string;
}

export interface PlaceOption {
  id: number;
  name: string;
}

export const getInstructor = async (
  instructorId: number
): Promise<InstructorDetails> => {
  const { data } = await api.get<InstructorDetails>(
    `/api/instructor/${instructorId}`
  );

  return data;
};

export const getInstructorSessions = async (
  instructorId: number
): Promise<InstructorSession[]> => {
  try {
    const { data } = await api.get<UpcomingSessionDto[]>(
      "/api/instructor/sessions/list",
      {
        params: {
          insId: instructorId,
        },
      }
    );

    return Promise.all(
      data.map(async (session) => {
        const enrolledSpots =
          await getSessionEnrolledCount(
            session.sessionId
          );

        return {
          id: session.sessionId,
          className: session.className,
          startTime: parseBackendDate(
            session.startTime
          ),
          endTime: parseBackendDate(
            session.endTime
          ),
          availableSpots:
            session.availableSlots,
          totalSpots:
            session.availableSlots +
            enrolledSpots,
          enrolledSpots,
          placeName: session.placeName,
          location: session.placeName,
        };
      })
    );
  } catch (error: unknown) {
    if (
      axios.isAxiosError(error) &&
      error.response?.status === 404
    ) {
      return [];
    }

    throw error;
  }
};

const getSessionEnrolledCount = async (
  sessionId: number
): Promise<number> => {
  try {
    const { data } =
      await api.get<SessionAttendanceResponseDto>(
        `/api/instructor/sessions/${sessionId}/attendance`
      );

    return (
      data.totalEnrolled ??
      data.TotalEnrolled ??
      data.subscribers?.length ??
      data.Subscribers?.length ??
      0
    );
  } catch {
    return 0;
  }
};

export const createInstructorSession = async (
  request: CreateSessionRequest
): Promise<void> => {
  await api.post(
    "/api/instructor/sessions",
    request
  );
};

export const deleteInstructorSession = async (
  sessionId: number
): Promise<void> => {
  await api.delete(
    "/api/instructor/sessions/delete",
    {
      params: {
        id: sessionId,
      },
    }
  );
};

export const getSessionAttendance = async (
  sessionId: number
): Promise<AttendanceRecord[]> => {
  try {
    const { data } =
      await api.get<SessionAttendanceResponseDto>(
        `/api/instructor/sessions/${sessionId}/attendance`
      );

    const subscribers =
      data.subscribers ?? data.Subscribers ?? [];

    return subscribers.map((subscriber) => ({
      id: subscriber.id ?? subscriber.Id,
      enrollmentId:
        subscriber.id ?? subscriber.Id,
      memberId: subscriber.id ?? subscriber.Id,
      memberEmail:
        subscriber.email ?? subscriber.Email,
      email: subscriber.email ?? subscriber.Email,
      memberName:
        subscriber.email ??
        subscriber.Email ??
        "Member",
      isPresent: true,
    }));
  } catch (error: unknown) {
    if (
      axios.isAxiosError(error) &&
      error.response?.status === 404
    ) {
      return [];
    }

    throw error;
  }
};

export const getClassOptions = async (): Promise<
  ClassOption[]
> => {
  const { data } = await api.get<ClassOption[]>(
    "/api/instructor/options/classes"
  );

  return data;
};

export const getPlaceOptions = async (): Promise<
  PlaceOption[]
> => {
  const { data } = await api.get<PlaceOption[]>(
    "/api/instructor/options/places"
  );

  return data;
};
