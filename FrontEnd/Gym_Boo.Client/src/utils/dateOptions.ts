import type { DateOption } from "../types";

export const getUpcomingDateOptions = (days = 7): DateOption[] => {
    const options: DateOption[] = [];
    const today = new Date();

    for (let i = 0; i < days; i++) {
        const d = new Date(today);
        d.setDate(today.getDate() + i);

        const label =
            i === 0
                ? "Today"
                : i === 1
                    ? "Tomorrow"
                    : d.toLocaleDateString("en-US", { weekday: "long" });

        options.push({ label, value: d.toISOString().split("T")[0] });
    }

    return options;
};