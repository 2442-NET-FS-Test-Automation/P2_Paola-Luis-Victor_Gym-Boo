import { useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate, useParams, Link } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { submitReview } from "../../api/reviews";
import { getClasses } from "../../api/sessions";
import type { ReviewContext, ReviewType } from "../../types";
import { getDisciplineStyle } from "../../utils/disciplineColors";
import { formatSessionDateTime, getDurationMinutes } from "../../utils/sessionFormat";
import ReviewSection from "../../components/ReviewSection/ReviewSection";
import "./LeaveReview.css";
import { getSubmittedReviews } from "../../api/reviews";
import { reviewTypeFromNumber } from "../../utils/reviewType";

interface SectionState {
    rating: number;
    comment: string;
    submitted: boolean;
    error: string | null;
}

const REVIEW_TYPES: {
    key: ReviewType;
    title: string;
    description: string;
    placeholder: string;
}[] = [
        {
            key: "Class",
            title: "Rate the Class",
            description: "How was the workout structure and intensity?",
            placeholder: "Share your experience with this class in your own words...",
        },
        {
            key: "Instructor",
            title: "Rate the Instructor",
            description: "How was the coaching and energy?",
            placeholder: "Share your experience with the instructor...",
        },
        {
            key: "Facilities",
            title: "Rate the Facilities",
            description: "How was the studio, equipment, and space?",
            placeholder: "Share your experience with the facilities...",
        },
    ];

const emptySection = (): SectionState => ({
    rating: 0,
    comment: "",
    submitted: false,
    error: null,
});

const LeaveReview = () => {
    const { enrollmentId, sessionId } = useParams<{
        enrollmentId: string;
        sessionId: string;
    }>();
    const location = useLocation();
    const navigate = useNavigate();

    const enrollmentIdNum = Number(enrollmentId);
    const sessionIdNum = Number(sessionId);

    const [context, setContext] = useState<ReviewContext | null>(
        (location.state as ReviewContext) ?? null
    );
    const [loadingContext, setLoadingContext] = useState(!context);

    const [sections, setSections] = useState<Record<ReviewType, SectionState>>({
        Class: emptySection(),
        Instructor: emptySection(),
        Facilities: emptySection(),
    });
    const [submitting, setSubmitting] = useState(false);

    // Fallback: si se llega directo por URL sin state, buscamos la sesión en la lista.
    useEffect(() => {
        if (context || !sessionIdNum) return;
        getClasses()
            .then((data) => {
                const found = data.find((s) => s.id === sessionIdNum);
                if (found) {
                    setContext({
                        className: found.className,
                        discipline: found.discipline,
                        instructorName: found.instructorName,
                        startTime: found.startTime,
                        endTime: found.endTime,
                    });
                }
            })
            .finally(() => setLoadingContext(false));
    }, [context, sessionIdNum]);

    const [loadingSubmitted, setLoadingSubmitted] = useState(true);
    useEffect(() => {
        if (!enrollmentIdNum) return;
        let cancelled = false;

        getSubmittedReviews(enrollmentIdNum)
            .then((reviews) => {
                if (cancelled) return;
                setSections((prev) => {
                    const next = { ...prev };
                    reviews.forEach((review) => {
                        const type = reviewTypeFromNumber(review.reviewType);
                        if (!type) return;
                        next[type] = {
                            rating: review.rating,
                            comment: review.comment ?? "",
                            submitted: true,
                            error: null,
                        };
                    });
                    return next;
                });
            })
            .catch((err: any) => {
                // 404 = sin reviews aún, no es un error real para este flujo
                if (err?.response?.status !== 404 && !cancelled) {
                    console.error("No se pudieron cargar las reviews previas", err);
                }
            })
            .finally(() => {
                if (!cancelled) setLoadingSubmitted(false);
            });

        return () => {
            cancelled = true;
        };
    }, [enrollmentIdNum]);

    const disciplineStyle = useMemo(
        () => (context ? getDisciplineStyle(context.discipline) : null),
        [context]
    );

    const updateSection = (type: ReviewType, patch: Partial<SectionState>) => {
        setSections((prev) => ({ ...prev, [type]: { ...prev[type], ...patch } }));
    };

    const pendingTypes = (Object.keys(sections) as ReviewType[]).filter(
        (type) => !sections[type].submitted && sections[type].rating > 0
    );

    const handleSubmit = async () => {
        if (pendingTypes.length === 0) return;
        setSubmitting(true);

        const results = await Promise.allSettled(
            pendingTypes.map((type) =>
                submitReview(type, {
                    enrollmentId: enrollmentIdNum,
                    sessionId: sessionIdNum,
                    rating: sections[type].rating,
                    comment: sections[type].comment,
                }).then(() => type)
            )
        );

        setSections((prev) => {
            const next = { ...prev };
            results.forEach((result, i) => {
                const type = pendingTypes[i];
                if (result.status === "fulfilled") {
                    next[type] = { ...next[type], submitted: true, error: null };
                } else {
                    const message =
                        (result.reason as any)?.response?.data?.message ??
                        "Could not submit this review.";
                    next[type] = { ...next[type], error: message };
                }
            });
            return next;
        });

        setSubmitting(false);
    };

    const allSubmitted = (Object.keys(sections) as ReviewType[]).every(
        (type) => sections[type].submitted
    );

    return (
        <div className="leave-review">
            <Link to="/member/bookings" className="leave-review__back">
                <ArrowLeft size={16} /> Back to My Bookings
            </Link>

            <p className="leave-review__eyebrow">POST-SESSION FEEDBACK</p>
            <h1>Leave a Review</h1>

            {loadingContext && <p className="leave-review__status">Loading session…</p>}

            {!loadingContext && !loadingSubmitted && context && (
                <div className="leave-review__summary">
                    <span
                        className="leave-review__discipline"
                        style={{
                            color: disciplineStyle?.color,
                            backgroundColor: disciplineStyle?.background,
                        }}
                    >
                        {context.discipline}
                    </span>
                    <div className="leave-review__summary-main">
                        <h3>{context.className}</h3>
                        <p>
                            {context.instructorName} · {formatSessionDateTime(context.startTime)}
                        </p>
                    </div>
                    <div className="leave-review__duration">
                        <span>DURATION</span>
                        <strong>
                            {getDurationMinutes(context.startTime, context.endTime)} min
                        </strong>
                    </div>
                </div>
            )}

            <div className="leave-review__sections">
                {REVIEW_TYPES.map(({ key, title, description, placeholder }) => (
                    <ReviewSection
                        key={key}
                        title={title}
                        description={description}
                        placeholder={placeholder}
                        rating={sections[key].rating}
                        onRatingChange={(n) => updateSection(key, { rating: n })}
                        comment={sections[key].comment}
                        onCommentChange={(v) => updateSection(key, { comment: v })}
                        submitted={sections[key].submitted}
                        error={sections[key].error}
                    />
                ))}
            </div>

            <div className="leave-review__actions">
                <button
                    type="button"
                    className="leave-review__skip"
                    onClick={() => navigate("/member/bookings")}
                >
                    Skip for Now
                </button>
                <button
                    type="button"
                    className="leave-review__submit"
                    disabled={allSubmitted || pendingTypes.length === 0 || submitting}
                    onClick={handleSubmit}
                >
                    {submitting
                        ? "Submitting…"
                        : allSubmitted
                            ? "All Reviews Submitted"
                            : "Submit Review"}
                </button>
            </div>
        </div>
    );
};

export default LeaveReview;