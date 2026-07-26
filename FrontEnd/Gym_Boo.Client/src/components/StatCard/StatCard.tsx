import "./StatCard.css";

interface StatCardProps {
    value: string | number;
    label: string;
    color?: "accent" | "warning" | "info";
}

const StatCard = ({ value, label, color = "accent" }: StatCardProps) => (
    <div className="stat-card">
        <p className={`stat-card__value stat-card__value--${color}`}>{value}</p>
        <p className="stat-card__label">{label}</p>
    </div>
);

export default StatCard;