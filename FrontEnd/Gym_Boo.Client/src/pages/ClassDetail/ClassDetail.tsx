import { useEffect, useMemo, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { Star, ArrowLeft } from "lucide-react";
import { getClasses } from "../../api/sessions";
import { bookClass } from "../../api/reservations";
import type { ApiClassSession } from "../../types";
import { getDisciplineStyle } from "../../utils/disciplineColors";
import {
    formatSessionDate,
    formatSessionTime,
    getDurationMinutes,
} from "../../utils/sessionFormat";
import { useCurrentUser } from "../../components/SideBar/useCurrentUser";
import CapacityBar from "../../components/CapacityBar/CapacityBar";
import InfoStatCard from "../../components/InfoStatCard/InfoStatCard";
import "./ClassDetail.css";

const ClassDetail = () => {
    const { id } = useParams<{ id: string }>();
    const user = useCurrentUser();

    const [session, setSession] = useState<ApiClassSession | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [booking, setBooking] = useState(false);
    const [bookingResult, setBookingResult] = useState< boolean | null>(null);
    const [bookingError, setBookingError] = useState<string | null>(null);

    useEffect(() => {
        let cancelled = false;
        setLoading(true);
        setError(null);

        getClasses()
            .then((data) => {
                if (cancelled) return;
                const found = data.find((s) => s.id === Number(id));
                setSession(found ?? null);
            })
            .catch(() => {
                if (!cancelled) setError("We couldn't load this class");
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });

        return () => {
            cancelled = true;
        };
    }, [id]);

    const disciplineStyle = useMemo(
        () => (session ? getDisciplineStyle(session.discipline) : null),
        [session]
    );

    const handleBook = async () => {
        if (!session) return;
        setBooking(true);
        setBookingError(null);
        setBookingResult(null);

        try {
            await bookClass(session.id, user.id);
            setBookingResult(true);
        } catch (err: any) {
            const message =
                err?.response?.data?.message ?? "We couldn't complete your reservation.";
            setBookingError(message);
        } finally {
            setBooking(false);
        }
    };

    if (loading) return <p className="class-detail__status">Loading…</p>;
    if (error) return <p className="class-detail__status class-detail__status--error">{error}</p>;
    if (!session) return <p className="class-detail__status">Class not found.</p>;

    const isFull = session.availableSpots <= 0;
    const duration = getDurationMinutes(session.startTime, session.endTime);

    return (
        <div className="class-detail">
            <Link to="/member/discover" className="class-detail__back">
                <ArrowLeft size={16} /> Back to Discover
            </Link>

            <div className="class-detail__layout">
                <div className="class-detail__main">
                    <div
                        className="class-detail__hero"
                        style={{
                            background: `linear-gradient(135deg, ${disciplineStyle?.background}, var(--color-bg-elevated))`,
                        }}
                    >
                        <span className="class-detail__rating">
                            <Star size={14} fill="currentColor" /> {session.instructorRating.toFixed(1)}
                        </span>

                        <div className="class-detail__hero-bottom">
                            <span
                                className="class-detail__discipline-badge"
                                style={{
                                    color: disciplineStyle?.color,
                                    backgroundColor: disciplineStyle?.background,
                                }}
                            >
                                {session.discipline}
                            </span>
                            <h1>{session.className}</h1>
                        </div>
                    </div>

                    <div className="class-detail__stats">
                        <InfoStatCard label="DATE" value={formatSessionDate(session.startTime)} />
                        <InfoStatCard label="TIME" value={`${formatSessionTime(session.startTime)} – ${formatSessionTime(session.endTime)}`} />
                        <InfoStatCard label="DURATION" value={`${duration} minutes`} />
                        <InfoStatCard label="LOCATION" value={session.location} />
                    </div>
                </div>

                <aside className="class-detail__sidebar">
                    <h2>Reserve Your Spot</h2>

                    <CapacityBar
                        availableSpots={session.availableSpots}
                        totalSpots={session.totalSpots}
                    />

                    <dl className="class-detail__details">
                        <div>
                            <dt>Class</dt>
                            <dd>{session.className}</dd>
                        </div>
                        <div>
                            <dt>Date</dt>
                            <dd>{formatSessionDate(session.startTime)}</dd>
                        </div>
                        <div>
                            <dt>Time</dt>
                            <dd>
                                {formatSessionTime(session.startTime)} – {formatSessionTime(session.endTime)}
                            </dd>
                        </div>
                    </dl>

                    {bookingResult && (
                        <p className="class-detail__booking-success">
                            Reservation confirmed
                        </p>
                    )}
                    {bookingError && (
                        <p className="class-detail__booking-error">{bookingError}</p>
                    )}

                    <button
                        type="button"
                        className="class-detail__book-btn"
                        disabled={isFull || booking || !!bookingResult}
                        onClick={handleBook}
                    >
                        {booking
                            ? "Booking…"
                            : bookingResult
                                ? "Booked"
                                : isFull
                                    ? "Class Full"
                                    : "Book Spot"}
                    </button>

                    <p className="class-detail__cancellation-note">
                        Free cancellation up to 2 hours before the class
                    </p>
                </aside>
            </div>
        </div>
    );
};

export default ClassDetail;