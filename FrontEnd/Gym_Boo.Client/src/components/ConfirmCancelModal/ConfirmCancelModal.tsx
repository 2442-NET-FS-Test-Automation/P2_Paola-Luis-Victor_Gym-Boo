import { useState } from "react";
import { X, AlertTriangle, CheckCircle2 } from "lucide-react";
import type { Reservation } from "../../types";
import { cancelReservation } from "../../api/reservations";
import { useCurrentUser } from "../SideBar/useCurrentUser";
import "./ConfirmCancelModal.css";

type Step = "confirm" | "loading" | "success" | "error";

interface ConfirmCancelModalProps {
    reservation: Reservation;
    onClose: () => void;
    onCancelled: (enrollmentId: number) => void;
}

const ConfirmCancelModal = ({
    reservation,
    onClose,
    onCancelled,
}: ConfirmCancelModalProps) => {
    const user = useCurrentUser();
    const [step, setStep] = useState<Step>("confirm");
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const handleConfirm = async () => {
        setStep("loading");
        try {
            await cancelReservation(reservation.enrollmentId, user.id);
            setStep("success");
            onCancelled(reservation.enrollmentId);
        } catch (err: any) {
            setErrorMessage(
                err?.response?.data?.message ?? "Could not cancel this reservation."
            );
            setStep("error");
        }
    };

    return (
        <div className="confirm-cancel-modal__overlay" onClick={onClose}>
            <div
                className="confirm-cancel-modal"
                onClick={(e) => e.stopPropagation()}
            >
                <button
                    type="button"
                    className="confirm-cancel-modal__close"
                    onClick={onClose}
                    aria-label="Close"
                >
                    <X size={18} />
                </button>

                {(step === "confirm" || step === "loading") && (
                    <>
                        <div className="confirm-cancel-modal__icon confirm-cancel-modal__icon--warning">
                            <AlertTriangle size={22} />
                        </div>
                        <h2>Cancel Reservation?</h2>
                        <p className="confirm-cancel-modal__class">{reservation.className}</p>

                        {reservation.hasPenalty ? (
                            <p className="confirm-cancel-modal__message confirm-cancel-modal__message--penalty">
                                You're canceling less than 2 hours before the class. A
                                cancellation fee of{" "}
                                <strong>${reservation.penalty.toFixed(2)}</strong> will apply.
                            </p>
                        ) : (
                            <p className="confirm-cancel-modal__message">
                                You're canceling more than 2 hours before the class. No
                                cancellation fee applies.
                            </p>
                        )}

                        <div className="confirm-cancel-modal__actions">
                            <button
                                type="button"
                                className="confirm-cancel-modal__keep"
                                onClick={onClose}
                                disabled={step === "loading"}
                            >
                                Keep Reservation
                            </button>
                            <button
                                type="button"
                                className="confirm-cancel-modal__confirm"
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
                        <div className="confirm-cancel-modal__icon confirm-cancel-modal__icon--success">
                            <CheckCircle2 size={22} />
                        </div>
                        <h2>Reservation Cancelled</h2>
                        <p className="confirm-cancel-modal__message">
                            Your spot in <strong>{reservation.className}</strong> has been
                            cancelled.
                            {reservation.hasPenalty &&
                                ` A fee of $${reservation.penalty.toFixed(2)} was applied.`}
                        </p>
                        <div className="confirm-cancel-modal__actions">
                            <button
                                type="button"
                                className="confirm-cancel-modal__confirm"
                                onClick={onClose}
                            >
                                Close
                            </button>
                        </div>
                    </>
                )}

                {step === "error" && (
                    <>
                        <div className="confirm-cancel-modal__icon confirm-cancel-modal__icon--warning">
                            <AlertTriangle size={22} />
                        </div>
                        <h2>Something Went Wrong</h2>
                        <p className="confirm-cancel-modal__message confirm-cancel-modal__message--penalty">
                            {errorMessage}
                        </p>
                        <div className="confirm-cancel-modal__actions">
                            <button
                                type="button"
                                className="confirm-cancel-modal__keep"
                                onClick={onClose}
                            >
                                Close
                            </button>
                            <button
                                type="button"
                                className="confirm-cancel-modal__confirm"
                                onClick={handleConfirm}
                            >
                                Try Again
                            </button>
                        </div>
                    </>
                )}
            </div>
        </div>
    );
};

export default ConfirmCancelModal;