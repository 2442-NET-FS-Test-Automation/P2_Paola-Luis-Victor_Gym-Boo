export type StatusVariant = "info" | "success" | "danger" | "neutral";

const STATUS_MAP: Record<string, StatusVariant> = {
    confirmed: "info",
    attended: "success",
    absent: "danger",
    canceled: "neutral",
    cancelled: "neutral",
};

export const getStatusVariant = (status: string): StatusVariant =>
    STATUS_MAP[status.toLowerCase()] ?? "neutral";

export const isCancelled = (status: string): boolean =>
    status.toLowerCase().startsWith("cancel");

export const isAttended = (status: string): boolean =>
    status.toLowerCase() === "attended";