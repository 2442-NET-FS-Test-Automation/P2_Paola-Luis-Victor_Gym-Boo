export type CapacityLevel = "plenty" | "low" | "full";

export const getCapacityLevel = (
    availableSpots: number,
    totalSpots: number
): CapacityLevel => {
    if (availableSpots <= 0) return "full";
    const percentAvailable = (availableSpots / totalSpots) * 100;
    return percentAvailable <= 25 ? "low" : "plenty";
};