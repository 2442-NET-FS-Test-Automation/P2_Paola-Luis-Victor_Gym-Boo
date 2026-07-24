import { useState } from "react";
import { X, AlertTriangle, CheckCircle2 } from "lucide-react";
import { cancelSubscription } from "../../api/plans";
import "./ConfirmCancelSubscriptionModal.css";

type Step = "confirm" | "loading" | "success" | "error";

interface ConfirmCancelSubscriptionModalProps {
    memberId: number;
    planName: string;
    onClose: () => void;
    onCancelled: () => void;
}

const ConfirmCancelSubscriptionModal = ({
    memberId,
    planName,
    onClose,
    onCancelled,
}: ConfirmCancelSubscriptionModalProps) => {
    const [step, setStep] = useState<Step>("confirm");
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const handleConfirm = async () => {
        setStep("loading");
        try {
            await cancelSubscription(memberId);
            setStep("success");
            onCancelled();
        } catch (err: any) {
            setErrorMessage(
                err?.response?.data?.message ?? "Could not cancel your subscription."
            );
            setStep("error");
        }
    };

    return (
        <div className="confirm-cancel-sub__overlay" onClick={step === "loading" ? undefined : onClose}>
            <div className="confirm-cancel-sub" onClick={(e) => e.stopPropagation()}>
                <button
                    type="button"
                    className="confirm-cancel-sub__close"
                    onClick={onClose}
                    disabled={step === "loading"}
                    aria-label="Close"
                >
                    <X size={18} />
                </button>

                {(step === "confirm" || step === "loading") && (
                    <>
                        <div className="confirm-cancel-sub__icon confirm-cancel-sub__icon--warning">
                            <AlertTriangle size={22} />
                        </div>
                        <h2>Cancel Subscription?</h2>
                        <p className="confirm-cancel-sub__message">
                            You're about to cancel your <strong>{planName}</strong>{" "}
                            membership. You'll lose access to unlimited classes and member
                            benefits at the end of your current billing period.
                        </p>
                        <div className="confirm-cancel-sub__actions">
                            <button
                                type="button"
                                className="confirm-cancel-sub__keep"
                                onClick={onClose}
                                disabled={step === "loading"}
                            >
                                Keep Subscription
                            </button>
                            <button
                                type="button"
                                className="confirm-cancel-sub__confirm"
                                onClick={handleConfirm}
                                disabled={step === "loading"}
                            >
                                {step === "loading" ? "Cancelling…" : "Confirm Cancellation"}
                            </button>
                        </div>
                    </>
                )}

                {step === "success" && (
                    <>
                        <div className="confirm-cancel-sub__icon confirm-cancel-sub__icon--success">
                            <CheckCircle2 size={22} />
                        </div>
                        <h2>Subscription Cancelled</h2>
                        <p className="confirm-cancel-sub__message">
                            Your <strong>{planName}</strong> membership has been cancelled.
                        </p>
                        <div className="confirm-cancel-sub__actions">
                            <button type="button" className="confirm-cancel-sub__confirm" onClick={onClose}>
                                Close
                            </button>
                        </div>
                    </>
                )}

                {step === "error" && (
                    <>
                        <div className="confirm-cancel-sub__icon confirm-cancel-sub__icon--warning">
                            <AlertTriangle size={22} />
                        </div>
                        <h2>Something Went Wrong</h2>
                        <p className="confirm-cancel-sub__message confirm-cancel-sub__message--error">
                            {errorMessage}
                        </p>
                        <div className="confirm-cancel-sub__actions">
                            <button type="button" className="confirm-cancel-sub__keep" onClick={onClose}>
                                Close
                            </button>
                            <button type="button" className="confirm-cancel-sub__confirm" onClick={handleConfirm}>
                                Try Again
                            </button>
                        </div>
                    </>
                )}
            </div>
        </div>
    );
};

export default ConfirmCancelSubscriptionModal;