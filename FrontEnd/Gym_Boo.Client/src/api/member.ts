import { api } from "./client";
import type { MemberReport } from "../types";

export const getMemberReport = async (memberId: number): Promise<MemberReport> => {
    const { data } = await api.get<MemberReport>(`/api/Member/${memberId}/report`);
    return data;
};