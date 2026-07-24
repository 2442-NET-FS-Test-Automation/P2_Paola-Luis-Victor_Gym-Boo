import { api } from "./client";
import type { Plan } from "../types";

export const getPlans = async (): Promise<Plan[]> => {
    const { data } = await api.get<Plan[]>("/api/Plans");
    return data;
};