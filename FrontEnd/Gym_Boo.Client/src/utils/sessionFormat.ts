export const formatSessionDate = (iso: string): string =>
    new Date(iso).toLocaleDateString("en-US", {
        weekday: "short",
        month: "short",
        day: "numeric",
    });

export const formatSessionTime = (iso: string): string =>
    new Date(iso).toLocaleTimeString("en-US", {
        hour: "2-digit",
        minute: "2-digit",
    });

export const getDurationMinutes = (start: string, end: string): number =>
    Math.round((new Date(end).getTime() - new Date(start).getTime()) / 60000);

export const getInitials = (name: string): string =>
    name
        .trim()
        .split(/\s+/)
        .slice(0, 2)
        .map((part) => part[0]?.toUpperCase() ?? "")
        .join("");