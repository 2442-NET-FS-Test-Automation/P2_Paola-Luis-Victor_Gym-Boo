import { ArrowUp, ArrowDown } from "lucide-react";
import "./StatCardTrend.css";

interface StatCardTrendProps {
    label: string;
    value: string;
    changePct: number;
    changeLabel: string;
    color?: "accent" | "info" | "warning";
}

const StatCardTrend = ({
    label,
    value,
    changePct,
    changeLabel,
    color = "accent",
}: StatCardTrendProps) => {
    const isPositive = changePct >= 0;

    return (
        <div className="stat-card-trend">
            <p className="stat-card-trend__label">{label}</p>
            <p className={`stat-card-trend__value stat-card-trend__value--${color}`}>
                {value}
            </p>
            <p
                className={`stat-card-trend__change ${isPositive ? "is-positive" : "is-negative"
                    }`}
            >
                {isPositive ? <ArrowUp size={12} /> : <ArrowDown size={12} />}
                {Math.abs(changePct)}% <span>{changeLabel}</span>
            </p>
        </div>
    );
};

export default StatCardTrend;