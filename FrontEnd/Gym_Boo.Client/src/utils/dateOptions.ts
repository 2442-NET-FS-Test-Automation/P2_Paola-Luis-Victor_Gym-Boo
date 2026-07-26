import type { DateOption } from "../types";

export const getUpcomingDateOptions = (days = 7): DateOption[] => {
    const options: DateOption[] = [];
    const today = new Date();

    for (let i = 0; i < days; i++) {
        const d = new Date(today.getFullYear(), today.getMonth(), today.getDate() + i);

        const label =
            i === 0
                ? "Today"
                : i === 1
                    ? "Tomorrow"
                    : d.toLocaleDateString("en-US", { weekday: "long" });

        const value = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(
            d.getDate()
        ).padStart(2, "0")}`;

        options.push({ label, value });
    }

    return options;
};