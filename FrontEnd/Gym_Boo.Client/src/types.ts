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
    id: number;
    name: string;
    role: string;
    initials: string;
}

export interface CancellationResponse {
    enrollmentId: number;
    status: string;
    hasPenalty: boolean;
    amount: number;
}

export interface Reservation {
    enrollmentId: number;
    sessionId: number;
    className: string;
    discipline: string;
    instructorName: string;
    startTime: string;
    endTime: string;
    location: string;
    status: string;
    hasPenalty: boolean;
    penalty: number;
}

export interface ReservationsResponse {
    upcoming: Reservation[];
    past: Reservation[];
}

export type ReviewType = "Class" | "Instructor" | "Facilities";

export interface ReviewRequest {
    enrollmentId: number;
    sessionId: number;
    rating: number;
    comment: string;
}

export interface ReviewContext {
    className: string;
    discipline: string;
    instructorName: string;
    startTime: string;
    endTime: string;
}

export interface SubmittedReview {
    reviewId: number;
    enrollmentId: number;
    reviewType: number;
    rating: number;
    comment: string | null;
    createdAt: string;
}