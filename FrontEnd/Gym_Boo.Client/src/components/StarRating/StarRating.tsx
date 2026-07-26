import { useState } from "react";
import { Star } from "lucide-react";
import "./StarRating.css";

interface StarRatingProps {
    value: number;
    onChange?: (value: number) => void;
    readOnly?: boolean;
    size?: number;
}

const StarRating = ({ value, onChange, readOnly, size = 24 }: StarRatingProps) => {
    const [hover, setHover] = useState(0);
    const display = hover || value;

    return (
        <div
            className={`star-rating ${readOnly ? "is-readonly" : ""}`}
            onMouseLeave={() => setHover(0)}
        >
            {[1, 2, 3, 4, 5].map((n) => (
                <button
                    key={n}
                    type="button"
                    disabled={readOnly}
                    className="star-rating__btn"
                    onClick={() => onChange?.(n)}
                    onMouseEnter={() => !readOnly && setHover(n)}
                    aria-label={`${n} star${n > 1 ? "s" : ""}`}
                >
                    <Star
                        size={size}
                        className={n <= display ? "is-filled" : "is-empty"}
                        fill={n <= display ? "currentColor" : "none"}
                    />
                </button>
            ))}
        </div>
    );
};

export default StarRating;