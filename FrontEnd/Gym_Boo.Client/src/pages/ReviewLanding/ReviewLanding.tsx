import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Star, MessageSquareText } from "lucide-react";
import { getReservations } from "../../api/reservations";
import type { Reservation, ReservationsResponse } from "../../types";
import { useCurrentUser } from "../../components/SideBar/useCurrentUser";
import { getDisciplineStyle } from "../../utils/disciplineColors";
import { formatSessionDateTime } from "../../utils/sessionFormat";
import { isAttended } from "../../utils/statusStyles";
import "./ReviewLanding.css";

const ReviewLanding = () => {
    const user = useCurrentUser();

    const [data, setData] = useState<ReservationsResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        let cancelled = false;
        setLoading(true);
        setError(null);

        getReservations(user.id)
            .then((res) => {
                if (!cancelled) setData(res);
            })
            .catch(() => {
                if (!cancelled) setError("No pudimos cargar tus clases.");
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });

        return () => {
            cancelled = true;
        };
    }, [user.id]);

    const attendedClasses = useMemo<Reservation[]>(() => {
        const past = data?.past ?? [];
        return past.filter((r) => isAttended(r.status));
    }, [data]);

    return (
        <div className="review-landing">
            <header className="review-landing__hero">
                <span className="review-landing__hero-icon">
                    <MessageSquareText size={22} />
                </span>
                <p className="review-landing__eyebrow">POST-SESSION FEEDBACK</p>
                <h1>Leave a Review</h1>
                <p className="review-landing__subtitle">
                    Your feedback helps instructors improve and helps other members
                    choose the right class. Pick a class you attended below and share
                    how it went — the class, the instructor, and the facilities.
                </p>
            </header>

            {loading && <p className="review-landing__status">Loading your classes…</p>}
            {error && (
                <p className="review-landing__status review-landing__status--error">
                    {error}
                </p>
            )}

            {!loading && !error && attendedClasses.length === 0 && (
                <div className="review-landing__empty">
                    <p>You haven't attended any classes yet.</p>
                    <p className="review-landing__empty-hint">
                        Once you complete a class, it'll show up here so you can leave a
                        review.
                    </p>
                </div>
            )}

            {!loading && !error && attendedClasses.length > 0 && (
                <div className="review-landing__list">
                    {attendedClasses.map((r) => {
                        const style = getDisciplineStyle(r.discipline);
                        return (
                            <div key={r.enrollmentId} className="review-landing__item">
                                <span
                                    className="review-landing__discipline"
                                    style={{ color: style.color, backgroundColor: style.background }}
                                >
                                    {r.discipline}
                                </span>

                                <div className="review-landing__item-main">
                                    <h3>{r.className}</h3>
                                    <p>
                                        {r.instructorName} · {formatSessionDateTime(r.startTime)}
                                    </p>
                                </div>

                                <Link
                                    to={`/member/review/${r.enrollmentId}/${r.sessionId}`}
                                    state={{
                                        className: r.className,
                                        discipline: r.discipline,
                                        instructorName: r.instructorName,
                                        startTime: r.startTime,
                                        endTime: r.endTime,
                                    }}
                                    className="review-landing__cta"
                                >
                                    Review <Star size={13} fill="currentColor" />
                                </Link>
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
};

export default ReviewLanding;