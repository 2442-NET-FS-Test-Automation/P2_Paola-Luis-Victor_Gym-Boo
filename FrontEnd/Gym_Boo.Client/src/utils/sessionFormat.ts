import { utcIsoToLocalDate } from "./timeZone";

export const formatSessionDate = (iso: string): string =>
    utcIsoToLocalDate(iso).toLocaleDateString("en-US", {
        weekday: "short",
        month: "short",
        day: "numeric",
    });

export const formatSessionTime = (iso: string): string =>
    utcIsoToLocalDate(iso).toLocaleTimeString("en-US", {
        hour: "2-digit",
        minute: "2-digit",
    });

export const formatSessionDateTime = (iso: string): string => {
    const date = utcIsoToLocalDate(iso);
    const datePart = date.toLocaleDateString("en-US", {
        weekday: "short",
        month: "short",
        day: "numeric",
        year: "numeric",
    });
    const timePart = date.toLocaleTimeString("en-US", {
        hour: "numeric",
        minute: "2-digit",
    });
    return `${datePart} · ${timePart}`;
};

export const getDurationMinutes = (start: string, end: string): number =>
    Math.round(
        (utcIsoToLocalDate(end).getTime() - utcIsoToLocalDate(start).getTime()) / 60000
    );

export const getInitials = (name: string): string =>
    name
        .trim()
        .split(/\s+/)
        .slice(0, 2)
        .map((part) => part[0]?.toUpperCase() ?? "")
        .join("");