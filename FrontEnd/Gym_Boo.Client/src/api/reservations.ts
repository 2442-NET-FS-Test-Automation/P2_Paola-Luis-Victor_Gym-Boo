import type { ReservationsResponse } from "../types";
import { api } from "./client";


export const bookClass = async (
    classId: number,
    userId: number
): Promise<void> => {
    const { data } = await api.post<void>(
        `/api/Reservations`,
        {
            sessionId : classId,
            memberId : userId
        }
    );
    return data;
};

export const getReservations = async (
    userId: number
): Promise<ReservationsResponse> => {
    const { data } = await api.get<ReservationsResponse>("/api/Reservations", {
        params: { userId },
    });
    return data;
};

export const cancelReservation = async (
    enrollmentId: number,
    userId: number
): Promise<void> => {
    await api.delete(`/api/Reservations/${enrollmentId}`, {
        params: { userId },
    });
};