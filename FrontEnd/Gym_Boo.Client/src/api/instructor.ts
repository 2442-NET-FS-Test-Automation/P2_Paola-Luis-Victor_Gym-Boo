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
  placeName?: string;
  location?: string;
}

export interface SubscriberDto {
  id: number;
  email: string;
}

export interface SessionAttendanceResponseDto {
  sessionId: number;
  totalEnrolled: number;
  subscribers: SubscriberDto[];
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

    return data.map((session) => ({
      id: session.sessionId,
      className: session.className,
      startTime: parseBackendDate(session.startTime),
      endTime: parseBackendDate(session.endTime),
      availableSpots: session.availableSlots,
      totalSpots: session.availableSlots,
      placeName: session.placeName,
      location: session.placeName,
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

export const createInstructorSession = async (
  request: CreateSessionRequest
): Promise<void> => {
  await api.post(
    "/api/instructor/sessions",
    request
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

    return data.subscribers.map((subscriber) => ({
      id: subscriber.id,
      enrollmentId: subscriber.id,
      memberId: subscriber.id,
      memberEmail: subscriber.email,
      email: subscriber.email,
      memberName: subscriber.email,
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
