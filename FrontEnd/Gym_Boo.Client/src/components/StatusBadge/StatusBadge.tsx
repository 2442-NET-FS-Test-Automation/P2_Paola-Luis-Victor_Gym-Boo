import { getStatusVariant } from "../../utils/statusStyles";
import "./StatusBadge.css";

interface StatusBadgeProps {
    status: string;
}

const StatusBadge = ({ status }: StatusBadgeProps) => {
    const variant = getStatusVariant(status);
    return <span className={`status-badge status-badge--${variant}`}>{status}</span>;
};

export default StatusBadge;