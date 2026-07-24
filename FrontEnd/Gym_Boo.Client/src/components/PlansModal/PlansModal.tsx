import { useEffect, useState } from "react";
import { X } from "lucide-react";
import { getPlans } from "../../api/plans";
import type { Plan } from "../../types";
import "./PlansModal.css";

interface PlansModalProps {
    onClose: () => void;
    currentPlanId: number | null;
}

type Tier = "plain" | "popular" | "elite";

const getTier = (plans: Plan[], index: number): Tier => {
    if (plans.length !== 3) return "plain";
    if (index === plans.length - 1) return "elite";
    if (index === 1) return "popular";
    return "plain";
};

const PlansModal = ({ onClose, currentPlanId }: PlansModalProps) => {
    const [plans, setPlans] = useState<Plan[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        getPlans()
            .then((data) =>
                setPlans([...data].sort((a, b) => a.price - b.price))
            )
            .catch(() => setError("We couldn't load plans"))
            .finally(() => setLoading(false));
    }, []);

    const handleChoose = (plan: Plan) => {
        // TODO: conectar al endpoint de suscripción cuando exista
        // (elegir plan / upgrade). Por ahora solo UI.
        console.log("TODO: subscribe to plan", plan.id);
    };

    return (
        <div className="plans-modal__overlay" onClick={onClose}>
            <div className="plans-modal" onClick={(e) => e.stopPropagation()}>
                <button
                    type="button"
                    className="plans-modal__close"
                    onClick={onClose}
                    aria-label="Close"
                >
                    <X size={18} />
                </button>

                <p className="plans-modal__eyebrow">MEMBERSHIP CATALOG</p>
                <h1>Choose Your Plan</h1>
                <p className="plans-modal__subtitle">Cancel anytime. No hidden fees.</p>

                {loading && <p className="plans-modal__status">Loading plans…</p>}
                {error && <p className="plans-modal__status plans-modal__status--error">{error}</p>}
                {!loading && !error && plans.length === 0 && (
                    <p className="plans-modal__status">No plans available right now.</p>
                )}

                {!loading && !error && plans.length > 0 && (
                    <div className="plans-modal__grid">
                        {plans.map((plan, index) => {
                            const tier = getTier(plans, index);
                            const isCurrent = plan.id === currentPlanId;
                            return (
                                <div key={plan.id} className={`plan-card plan-card--${tier}`}>
                                    {tier === "popular" && (
                                        <span className="plan-card__badge">Most Popular</span>
                                    )}
                                    <p className="plan-card__name">{plan.name}</p>
                                    <p className="plan-card__price">
                                        ${plan.price}
                                        <span>/{plan.recurrence}</span>
                                    </p>
                                    <button
                                        type="button"
                                        className="plan-card__cta"
                                        disabled={isCurrent}
                                        onClick={() => handleChoose(plan)}
                                    >
                                        {isCurrent ? "Current Plan" : `Choose ${plan.name}`}
                                    </button>
                                </div>
                            );
                        })}
                    </div>
                )}

                <p className="plans-modal__footer">
                    Secure payment powered by Stripe · All plans auto-renew · Cancel
                    anytime from your profile
                </p>
            </div>
        </div>
    );
};

export default PlansModal;