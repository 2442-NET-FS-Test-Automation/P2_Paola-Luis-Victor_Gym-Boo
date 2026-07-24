import { api } from "./client";
import type { ReviewRequest, ReviewType, SubmittedReview } from "../types";

export const submitReview = async (
    reviewType: ReviewType,
    payload: ReviewRequest
): Promise<void> => {
    await api.post(`/api/Reviews/${reviewType}`, payload);
};

export const getSubmittedReviews = async (
    enrollmentId: number
): Promise<SubmittedReview[]> => {
    const { data } = await api.get<SubmittedReview[]>(
        `/api/Reviews/submitted/${enrollmentId}`
    );
    return data;
};