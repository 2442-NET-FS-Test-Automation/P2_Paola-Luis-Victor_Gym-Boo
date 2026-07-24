import { api } from "./client";
import type { Plan, SubscriptionActionResponse } from "../types";

export const getPlans = async (): Promise<Plan[]> => {
    const { data } = await api.get<Plan[]>("/api/Plans");
    return data;
};

export const subscribeToPlan = async (
    memberId: number,
    planId: number
): Promise<SubscriptionActionResponse> => {
    const { data } = await api.post<SubscriptionActionResponse>(
        "/api/Plans/subscription/new",
        { memberId, planId }
    );
    return data;
};

export const updatePlanSubscription = async (
    memberId: number,
    currentPlanId: number,
    newPlanId: number
): Promise<SubscriptionActionResponse> => {
    const { data } = await api.put<SubscriptionActionResponse>(
        "/api/Plans/subscription/update",
        { memberId, currentPlanId, newPlanId }
    );
    return data;
};

export const cancelSubscription = async (memberId: number): Promise<void> => {
    await api.delete(`/api/Plans/subscription/cancel/${memberId}`);
};