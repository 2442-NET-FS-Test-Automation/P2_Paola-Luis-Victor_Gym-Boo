import type { ReviewType } from "../types";

const REVIEW_TYPE_MAP: Record<number, ReviewType> = {
    1: "Facilities",
    2: "Class",
    3: "Instructor",
};

export const reviewTypeFromNumber = (n: number): ReviewType | undefined =>
    REVIEW_TYPE_MAP[n];