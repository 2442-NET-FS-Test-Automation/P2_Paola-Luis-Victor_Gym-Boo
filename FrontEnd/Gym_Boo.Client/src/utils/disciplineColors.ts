interface DisciplineStyle {
    color: string;
    background: string;
}

const DISCIPLINE_COLORS: Record<string, DisciplineStyle> = {
    hiit: { color: "var(--color-danger)", background: "var(--color-danger-bg)" },
    yoga: { color: "var(--color-success)", background: "var(--color-success-bg)" },
    crossfit: { color: "var(--color-warning)", background: "var(--color-warning-bg)" },
    cycling: { color: "var(--color-info)", background: "var(--color-info-bg)" },
    pilates: { color: "var(--color-purple)", background: "var(--color-purple-bg)" },
    boxing: { color: "var(--color-danger)", background: "var(--color-danger-bg)" },
};

const DEFAULT_STYLE: DisciplineStyle = {
    color: "var(--color-accent)",
    background: "var(--color-accent-soft)",
};

export const getDisciplineStyle = (discipline: string): DisciplineStyle =>
    DISCIPLINE_COLORS[discipline.toLowerCase()] ?? DEFAULT_STYLE;