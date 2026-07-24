import { api } from "./client";
import type { ApiClassSession, ClassFilters } from "../types";

export const getClasses = async (
    filters?: ClassFilters
): Promise<ApiClassSession[]> => {
    const { data } = await api.get<ApiClassSession[]>("/api/Classes", {
        params: filters,
    });
    return data;
};