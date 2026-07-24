import { useEffect, useState } from "react";
import { getMemberReport } from "../../api/member";
import { getPlans } from "../../api/plans";
import type { MemberReport, Plan } from "../../types";
import { useCurrentUser } from "../../components/SideBar/useCurrentUser";
import { getInitials } from "../../utils/sessionFormat";
import { formatMonthYear } from "../../utils/sessionFormat";
import StatCard from "../../components/StatCard/StatCard";
import PlansModal from "../../components/PlansModal/PlansModal";
import "./MyProfile.css";

const MyProfile = () => {
    const user = useCurrentUser();

    const [report, setReport] = useState<MemberReport | null>(null);
    const [plans, setPlans] = useState<Plan[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [showPlansModal, setShowPlansModal] = useState(false);

    useEffect(() => {
        let cancelled = false;
        setLoading(true);
        setError(null);

        Promise.all([getMemberReport(user.id), getPlans()])
            .then(([reportData, plansData]) => {
                if (cancelled) return;
                setReport(reportData);
                setPlans(plansData);
            })
            .catch(() => {
                if (!cancelled) setError("No pudimos cargar tu perfil.");
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });

        return () => {
            cancelled = true;
        };
    }, [user.id]);

    if (loading) return <p className="my-profile__status">Loading profile…</p>;
    if (error || !report)
        return (
            <p className="my-profile__status my-profile__status--error">
                {error ?? "Profile not found."}
            </p>
        );

    const hasSubscription = report.idSubscription !== null && report.planId !== null;
    const currentPlan = plans.find((p) => p.id === report.planId) ?? null;

    return (
        <div className="my-profile">
            <div className="my-profile__header">
                <h1>My Profile</h1>
                <span
                    className={`my-profile__sub-badge ${hasSubscription ? "is-active" : "is-none"
                        }`}
                >
                    <span className="my-profile__sub-dot" />
                    {hasSubscription ? "Active Subscription" : "No Subscription"}
                </span>
            </div>

            <div className="my-profile__grid">
                <div className="my-profile__left">
                    <section className="my-profile__card">
                        <div className="my-profile__identity">
                            <span className="my-profile__avatar">
                                {getInitials(`${report.name} ${report.lastName}`)}
                            </span>
                            <div>
                                <div className="my-profile__name-row">
                                    <h2>
                                        {report.name} {report.lastName}
                                    </h2>
                                    <span className="my-profile__user-type">{report.userType}</span>
                                </div>
                            </div>
                        </div>

                        <div className="my-profile__field">
                            <p className="my-profile__field-label">FULL NAME</p>
                            <p className="my-profile__field-value">
                                {report.name} {report.lastName}
                            </p>
                        </div>
                        <div className="my-profile__field">
                            <p className="my-profile__field-label">EMAIL</p>
                            <p className="my-profile__field-value">{report.email}</p>
                        </div>
                    </section>

                    <section className="my-profile__card">
                        <h3 className="my-profile__section-title">Fitness Profile</h3>
                        <div className="my-profile__stats">
                            <StatCard value={report.classesAttended} label="Classes Attended" />
                            <StatCard
                                value={`${report.avgReviews.toFixed(1)} ★`}
                                label="Avg Review Given"
                                color="warning"
                            />
                        </div>
                    </section>
                </div>

                <div className="my-profile__right">
                    {hasSubscription ? (
                        <section className="my-profile__card my-profile__plan-card">
                            <p className="my-profile__field-label">CURRENT PLAN</p>
                            <h2 className="my-profile__plan-name">
                                {currentPlan?.name ?? `Plan #${report.planId}`}
                            </h2>
                            {currentPlan && (
                                <p className="my-profile__plan-price">
                                    ${currentPlan.price}
                                    <span>/{currentPlan.recurrence}</span>
                                </p>
                            )}

                            <div className="my-profile__plan-details">
                                {report.startTime && (
                                    <div>
                                        <span>Subscribed since</span>
                                        <strong>{formatMonthYear(report.startTime)}</strong>
                                    </div>
                                )}
                                {report.endTime && (
                                    <div>
                                        <span>Next renewal</span>
                                        <strong>{formatMonthYear(report.endTime)}</strong>
                                    </div>
                                )}
                            </div>

                            <div className="my-profile__plan-actions">
                                <button
                                    type="button"
                                    className="my-profile__upgrade-btn"
                                    onClick={() => setShowPlansModal(true)}
                                >
                                    Upgrade Plan
                                </button>
                                <button
                                    type="button"
                                    className="my-profile__cancel-btn"
                                    onClick={() => {
                                        // TODO: conectar al endpoint de cancelación de suscripción cuando exista
                                        console.log("TODO: cancel subscription");
                                    }}
                                >
                                    Cancel Plan
                                </button>
                            </div>
                        </section>
                    ) : (
                        <section className="my-profile__card my-profile__no-sub-card">
                            <h2>No Active Subscription</h2>
                            <p>
                                You don't have an active plan. Subscribe to unlock unlimited
                                classes and exclusive member benefits.
                            </p>
                            <button
                                type="button"
                                className="my-profile__choose-btn"
                                onClick={() => setShowPlansModal(true)}
                            >
                                Choose a Plan →
                            </button>

                            <div className="my-profile__perks">
                                <p className="my-profile__field-label">WHAT MEMBERS GET</p>
                                <ul>
                                    <li>Unlimited classes across all types</li>
                                    <li>Priority booking window</li>
                                    <li>Progress tracking &amp; analytics</li>
                                    <li>Guest passes for friends &amp; family</li>
                                    <li>Exclusive member workshops &amp; events</li>
                                </ul>
                            </div>
                        </section>
                    )}
                </div>
            </div>

            {showPlansModal && (
                <PlansModal
                    onClose={() => setShowPlansModal(false)}
                    currentPlanId={report.planId}
                />
            )}
        </div>
    );
};

export default MyProfile;