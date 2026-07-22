import { Star, Clock, Users } from "lucide-react";
import type { ApiClassSession } from "../../types";
import { getDisciplineStyle } from "../../utils/disciplineColors";
import {
    formatSessionDate,
    formatSessionTime,
    getDurationMinutes,
    getInitials,
} from "../../utils/sessionFormat";
import "./SessionCard.css";

interface SessionCardProps {
    session: ApiClassSession;
}

const SessionCard = ({ session }: SessionCardProps) => {
    const disciplineStyle = getDisciplineStyle(session.discipline);
    const isFull = session.availableSpots <= 0;
    const duration = getDurationMinutes(session.startTime, session.endTime);

    return (
        <article className="session-card">
            <div className="session-card__top">
                <span
                    className="session-card__badge"
                    style={{
                        color: disciplineStyle.color,
                        backgroundColor: disciplineStyle.background,
                    }}
                >
                    {session.discipline}
                </span>
                <span
                    className={`session-card__availability ${isFull ? "is-full" : ""
                        }`}
                >
                    {isFull ? "FULL" : `${session.availableSpots} left`}
                </span>
            </div>

            <h3 className="session-card__title">{session.className}</h3>

            <div className="session-card__rating">
                <Star size={14} fill="currentColor" />
                <span>—</span>
            </div>

            <div className="session-card__instructor">
                <span className="session-card__avatar">
                    {getInitials(session.instructorName)}
                </span>
                <span>{session.instructorName}</span>
            </div>

            <div className="session-card__meta">
                <span>
                    {formatSessionDate(session.startTime)} ·{" "}
                    {formatSessionTime(session.startTime)} · {duration}min
                </span>
                <span className="session-card__spots">
                    <Users size={13} /> {session.totalSpots} spots
                </span>
            </div>
        </article>
    );
};

export default SessionCard;