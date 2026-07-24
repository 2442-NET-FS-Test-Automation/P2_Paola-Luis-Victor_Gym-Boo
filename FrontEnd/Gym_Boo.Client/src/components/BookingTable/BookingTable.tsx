import { Link } from "react-router-dom";
import { Star, XCircle } from "lucide-react";
import type { Reservation } from "../../types";
import { getDisciplineStyle } from "../../utils/disciplineColors";
import { formatSessionDate, formatSessionTime } from "../../utils/sessionFormat";
import { isAttended } from "../../utils/statusStyles";
import StatusBadge from "../StatusBadge/StatusBadge";
import "./BookingTable.css";

interface BookingsTableProps {
    reservations: Reservation[];
    variant?: "upcoming" | "past";
    onCancelClick?: (reservation: Reservation) => void;
}

const BookingsTable = ({
    reservations,
    variant,
    onCancelClick,
}: BookingsTableProps) => {
    const showAction = variant === "upcoming" || variant === "past";

    if (reservations.length === 0) {
        return <p className="bookings-table__empty">No classes here yet.</p>;
    }

    return (
        <table className="bookings-table">
            <thead>
                <tr>
                    <th>Class</th>
                    <th>Instructor</th>
                    <th>Date &amp; Time</th>
                    <th>Status</th>
                    {showAction && <th>Action</th>}
                </tr>
            </thead>
            <tbody>
                {reservations.map((r) => {
                    const style = getDisciplineStyle(r.discipline);
                    return (
                        <tr key={r.enrollmentId}>
                            <td>
                                <div className="bookings-table__class">
                                    <span
                                        className="bookings-table__discipline"
                                        style={{ color: style.color, backgroundColor: style.background }}
                                    >
                                        {r.discipline}
                                    </span>
                                    <span className="bookings-table__class-name">{r.className}</span>
                                </div>
                            </td>
                            <td className="bookings-table__muted">{r.instructorName}</td>
                            <td>
                                <div className="bookings-table__datetime">
                                    <span>{formatSessionDate(r.startTime)}</span>
                                    <span className="bookings-table__muted">
                                        {formatSessionTime(r.startTime)}
                                    </span>
                                </div>
                            </td>
                            <td>
                                <StatusBadge status={r.status} />
                            </td>
                            {showAction && (
                                <td>
                                    {variant === "past" && isAttended(r.status) && (
                                        <Link
                                            to={`/member/review/${r.enrollmentId}/${r.sessionId}`}
                                            state={{
                                                className: r.className,
                                                discipline: r.discipline,
                                                instructorName: r.instructorName,
                                                startTime: r.startTime,
                                                endTime: r.endTime,
                                            }}
                                            className="bookings-table__review-btn"
                                        >
                                            Review <Star size={13} fill="currentColor" />
                                        </Link>
                                    )}

                                    {variant === "upcoming" && (
                                        <button
                                            type="button"
                                            className="bookings-table__cancel-btn"
                                            onClick={() => onCancelClick?.(r)}
                                        >
                                            <XCircle size={13} /> Cancel
                                        </button>
                                    )}
                                </td>
                            )}
                        </tr>
                    );
                })}
            </tbody>
        </table>
    );
};

export default BookingsTable;