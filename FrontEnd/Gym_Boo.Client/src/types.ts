export interface ApiClassSession {
    id: number;
    className: string;
    discipline: string;
    instructorName: string;
    instructorRating: number;
    startTime: string; // ISO
    endTime: string; // ISO
    location: string;
    availableSpots: number;
    totalSpots: number;
}

export interface ClassFilters {
    discipline?: string;
    date?: string; // ISO date-time
    past?: boolean;
}

export interface DateOption {
    label: string;
    value: string; // "YYYY-MM-DD"
}

export interface CurrentUser {
    name: string;
    role: string;
    initials: string;
}