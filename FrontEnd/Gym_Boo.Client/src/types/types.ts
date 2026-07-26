
export interface Discipline {
    id: number;
    name: string;
    isActive: boolean;
}

export interface Instructor {
    id: number;
    name: string;
    lastName: string;
    email: string;
    isActive: boolean;
    role: number; // Based on your Role enum
}

export interface CreateInstructorDto {
    firstName: string;
    lastName: string;
    email: string;
    password?: string; // Optional on the frontend after submission
}

export interface ApiResponse {
    message: string;
}