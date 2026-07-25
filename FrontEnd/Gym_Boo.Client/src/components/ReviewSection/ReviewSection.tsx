import { Check } from "lucide-react";
import StarRating from "../StarRating/StarRating";
import "./ReviewSection.css";

interface ReviewSectionProps {
    title: string;
    description: string;
    rating: number;
    onRatingChange: (n: number) => void;
    comment: string;
    onCommentChange: (v: string) => void;
    submitted: boolean;
    error?: string | null;
    placeholder?: string;
}

const MAX_LENGTH = 1000;

const ReviewSection = ({
    title,
    description,
    rating,
    onRatingChange,
    comment,
    onCommentChange,
    submitted,
    error,
    placeholder,
}: ReviewSectionProps) => {
    return (
        <section className="review-section">
            <div className="review-section__header">
                <h2>{title}</h2>
                {submitted && (
                    <span className="review-section__submitted">
                        <Check size={14} /> Submitted
                    </span>
                )}
            </div>
            <p className="review-section__description">{description}</p>

            <StarRating value={rating} onChange={onRatingChange} readOnly={submitted} />
            <p className="review-section__hint">
                {submitted ? "Thanks for your feedback!" : "Click a star to rate"}
            </p>

            {!submitted && (
                <div className="review-section__comment">
                    <textarea
                        value={comment}
                        maxLength={MAX_LENGTH}
                        placeholder={placeholder}
                        onChange={(e) => onCommentChange(e.target.value)}
                        rows={4}
                    />
                    <span className="review-section__counter">
                        {comment.length}/{MAX_LENGTH}
                    </span>
                </div>
            )}

            {error && <p className="review-section__error">{error}</p>}
        </section>
    );
};

export default ReviewSection;