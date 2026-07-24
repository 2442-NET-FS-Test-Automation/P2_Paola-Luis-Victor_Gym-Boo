/**
* All dates received from the backend are in UTC (ISO with the Z suffix).
* All dates sent to the backend must be in UTC.
* These utilities are the only input/output point for this conversion,
* so time zone logic is not repeated in each component.
*/

// UTC (backend ISO) -> Date in browser's local time.
// new Date() already performs this conversion when parsing an ISO with 'Z' or offset;
// this function simply makes it explicit as the single "reading" point.
export const utcIsoToLocalDate = (utcIso: string): Date => new Date(utcIso);

// Date local -> ISO in UTC, ready to send to the backend.
export const localDateToUtcIso = (date: Date): string => date.toISOString();

// Takes a calendar date "YYYY-MM-DD" chosen by the user in their local time zone
// (e.g., from a day selector) and converts it to local midnight of that day,
// expressed in UTC, to send it as a query param to the backend.
export const localDateStringToUtcIso = (dateStr: string): string => {
    const [year, month, day] = dateStr.split("-").map(Number);
    const localMidnight = new Date(year, month - 1, day, 0, 0, 0, 0);
    return localDateToUtcIso(localMidnight);
};