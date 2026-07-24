import { getCapacityLevel } from "../../utils/capacity";
import "./CapacityBar.css";

interface CapacityBarProps {
    availableSpots: number;
    totalSpots: number;
}

const CapacityBar = ({ availableSpots, totalSpots }: CapacityBarProps) => {
    const enrolled = totalSpots - availableSpots;
    const percentEnrolled = totalSpots > 0 ? (enrolled / totalSpots) * 100 : 0;
    const percentAvailable = Math.round(100 - percentEnrolled);
    const level = getCapacityLevel(availableSpots, totalSpots);

    return (
        <div className="capacity-bar">
            <div className="capacity-bar__header">
                <span>CAPACITY</span>
                <span className="capacity-bar__enrolled">
                    {enrolled}/{totalSpots} enrolled
                </span>
            </div>

            <div className="capacity-bar__track">
                <div
                    className={`capacity-bar__fill capacity-bar__fill--${level}`}
                    style={{ width: `${percentEnrolled}%` }}
                />
            </div>

            <div className="capacity-bar__footer">
                <span>{percentAvailable}% available</span>
                {level !== "plenty" && (
                    <span className={`capacity-bar__urgency capacity-bar__urgency--${level}`}>
                        {level === "full" ? "Class is full" : `Only ${availableSpots} spots left!`}
                    </span>
                )}
            </div>
        </div>
    );
};

export default CapacityBar;