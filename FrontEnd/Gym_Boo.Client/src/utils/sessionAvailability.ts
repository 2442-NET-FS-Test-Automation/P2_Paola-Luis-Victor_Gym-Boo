import { utcIsoToLocalDate } from "./timeZone";

export const isSessionPast = (startTime: string): boolean =>
    utcIsoToLocalDate(startTime).getTime() < Date.now();

export const isSessionFull = (availableSpots: number): boolean =>
    availableSpots <= 0;

export const isSessionUnavailable = (
    startTime: string,
    availableSpots: number
): boolean => isSessionPast(startTime) || isSessionFull(availableSpots);