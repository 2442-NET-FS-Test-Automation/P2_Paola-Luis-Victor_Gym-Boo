import { useEffect, useState } from "react";
import { X, Check } from "lucide-react";
import { getPlans, subscribeToPlan, updatePlanSubscription } from "../../api/plans";
import type { Plan } from "../../types";
import "./PlansModal.css";

type Tier = "plain" | "popular" | "elite";
type PlanStatus = "idle" | "loading" | "success" | "error";

interface PlansModalProps {
    mode: "new" | "upgrade";
    memberId: number;
    currentPlanId: number | null;
    onClose: () => void;
    onSuccess: () => void;
}

const getTier = (plans: Plan[], index: number): Tier => {
    if (plans.length !== 3) return "plain";
    if (index === plans.length - 1) return "elite";
    if (index === 1) return "popular";
    return "plain";
};

const PlansModal = ({ mode, memberId, currentPlanId, onClose, onSuccess }: PlansModalProps) => {
    const [plans, setPlans] = useState<Plan[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [planStatus, setPlanStatus] = useState<Record<number, PlanStatus>>({});
    const [planErrorMessage, setPlanErrorMessage] = useState<Record<number, string>>({});
    const [succeededMessage, setSucceededMessage] = useState<string | null>(null);

    useEffect(() => {
        getPlans()
            .then((data) => setPlans([...data].sort((a, b) => a.price - b.price)))
            .catch(() => setError("No pudimos cargar los planes."))
            .finally(() => setLoading(false));
    }, []);

    const anyLoading = Object.values(planStatus).some((s) => s === "loading");
    const anySucceeded = Object.values(planStatus).some((s) => s === "success");

    const handleChoose = async (plan: Plan) => {
        if (anyLoading || anySucceeded) return;
        setPlanStatus((prev) => ({ ...prev, [plan.id]: "loading" }));
        setPlanErrorMessage((prev) => ({ ...prev, [plan.id]: "" }));

        try {
            const response =
                mode === "new"
                    ? await subscribeToPlan(memberId, plan.id)
                    : await updatePlanSubscription(memberId, currentPlanId as number, plan.id);

            if (!response.result) {
                throw new Error(response.resultMessage);
            }

            setPlanStatus((prev) => ({ ...prev, [plan.id]: "success" }));
            setSucceededMessage(response.resultMessage);
            onSuccess();
        } catch (err: any) {
            const message =
                err?.response?.data?.message ?? err?.message ?? "Could not process this plan.";
            setPlanStatus((prev) => ({ ...prev, [plan.id]: "error" }));
            setPlanErrorMessage((prev) => ({ ...prev, [plan.id]: message }));
        }
    };

    return (
        <div className="plans-modal__overlay" onClick={anyLoading ? undefined : onClose}>
            <div className="plans-modal" onClick={(e) => e.stopPropagation()}>
                <button
                    type="button"
                    className="plans-modal__close"
                    onClick={onClose}
                    disabled={anyLoading}
                    aria-label="Close"
                >
                    <X size={18} />
                </button>

                <p className="plans-modal__eyebrow">GYMBOO MEMBERSHIP</p>
                <h1>{mode === "new" ? "Choose Your Plan" : "Upgrade Your Plan"}</h1>
                <p className="plans-modal__subtitle">Cancel anytime. No hidden fees.</p>

                {succeededMessage && (
                    <p className="plans-modal__success-banner">
                        <Check size={16} /> {succeededMessage} You can close this window.
                    </p>
                )}

                {loading && <p className="plans-modal__status">Loading plans…</p>}
                {error && <p className="plans-modal__status plans-modal__status--error">{error}</p>}
                {!loading && !error && plans.length === 0 && (
                    <p className="plans-modal__status">No plans available right now.</p>
                )}

                {!loading && !error && plans.length > 0 && (
                    <div className="plans-modal__grid">
                        {plans.map((plan, index) => {
                            const tier = getTier(plans, index);
                            const isCurrent = mode === "upgrade" && plan.id === currentPlanId;
                            const status = planStatus[plan.id] ?? "idle";
                            const disabled = isCurrent || anyLoading || anySucceeded;

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
                                        disabled={disabled}
                                        onClick={() => handleChoose(plan)}
                                    >
                                        {isCurrent
                                            ? "Current Plan"
                                            : status === "loading"
                                                ? "Processing…"
                                                : status === "success"
                                                    ? "Confirmed ✓"
                                                    : mode === "new"
                                                        ? `Choose ${plan.name}`
                                                        : `Switch to ${plan.name}`}
                                    </button>

                                    {status === "error" && (
                                        <p className="plan-card__error">{planErrorMessage[plan.id]}</p>
                                    )}
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