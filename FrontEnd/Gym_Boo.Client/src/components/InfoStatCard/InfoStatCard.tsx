import "./InfoStatCard.css";

interface InfoStatCardProps {
    label: string;
    value: string;
}

const InfoStatCard = ({ label, value }: InfoStatCardProps) => (
    <div className="info-stat-card">
        <p className="info-stat-card__label">{label}</p>
        <p className="info-stat-card__value">{value}</p>
    </div>
);

export default InfoStatCard;