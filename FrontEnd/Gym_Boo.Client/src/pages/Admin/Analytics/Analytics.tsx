import { useEffect, useState } from "react";
import {
    getAdminOverviewStats,
    getRevenueTrend,
    getClassOccupancyRates,
    getRevenueSummary,
    getTopSessionsByEnrollment, getTopRatedSessions
} from "../../../api/admin";
import type {
    AdminOverviewStats,
    ClassOccupancyRate,
    RevenueSummary,
    RevenueTrendPoint,
    RankingItem,
    TopRatedSession,
    TopSessionEnrollment
} from "../../../types";
import StatCardTrend from "../../../components/StatCardTrend/StatCardTrend";
import RevenueTrendChart from "../../../components/RevenueTrendChart/RevenueTrendChart";
import RevenueBreakdownDonut from "../../../components/RevenueBreakdownDonut/RevenueBreakdownDonut";
import OccupancyBarChart from "../../../components/OccupancyBarChart/OccupancyBarChart";
import TopRankingChart from "../../../components/TopRankingChart/TopRankingChart";
import "./Analytics.css";

const AdminAnalytics = () => {
    const [overview, setOverview] = useState<AdminOverviewStats | null>(null);
    const [trend, setTrend] = useState<RevenueTrendPoint[]>([]);
    const [occupancy, setOccupancy] = useState<ClassOccupancyRate[]>([]);
    const [revenueSummary, setRevenueSummary] = useState<RevenueSummary | null>(null);
    const [topEnrollments, setTopEnrollments] = useState<TopSessionEnrollment[]>([]);
    const [topRated, setTopRated] = useState<TopRatedSession[]>([]);

    const enrollmentRankingData: RankingItem[] = [...topEnrollments]
        .sort((a, b) => b.totalEnrollments - a.totalEnrollments)
        .slice(0, 5)
        .map((item) => ({
            id: item.disciplineName,
            label: item.disciplineName,
            value: item.totalEnrollments,
            displayValue: `${item.totalEnrollments} enrollments`,
        }));

    const ratedRankingData: RankingItem[] = [...topRated]
        .filter((item) => item.averageRating > 0)
        .sort((a, b) => b.averageRating - a.averageRating)
        .slice(0, 5)
        .map((item) => ({
            id: item.id,
            label: item.className,
            subLabel: item.instructorName,
            value: item.averageRating,
            displayValue: `${item.averageRating.toFixed(1)} ★`,
        }));

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        let cancelled = false;
        setLoading(true);
        setError(null);

        Promise.all([
            getAdminOverviewStats(),
            getRevenueTrend(),
            getClassOccupancyRates(),
            getRevenueSummary(),
            getTopSessionsByEnrollment(),
            getTopRatedSessions(),
        ])
            .then(([overviewData, trendData, occupancyData, revenueData, enrollmentsData, ratedData]) => {
                if (cancelled) return;
                setOverview(overviewData);
                setTrend(trendData);
                setOccupancy(occupancyData);
                setRevenueSummary(revenueData);
                setTopEnrollments(enrollmentsData);
                setTopRated(ratedData);
            })
            .catch(() => {
                if (!cancelled) setError("We couldn't load the reports.");
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });

        return () => {
            cancelled = true;
        };
    }, []);

    if (loading) return <p className="admin-analytics__status">Loading reports…</p>;
    if (error || !overview || !revenueSummary)
        return (
            <p className="admin-analytics__status admin-analytics__status--error">
                {error ?? "No data available."}
            </p>
        );

    return (
        <div className="admin-analytics">
            <header className="admin-analytics__header">
                <p className="admin-analytics__eyebrow">ADMIN PANEL</p>
                <h1>Analytics &amp; Reports</h1>
                <p className="admin-analytics__subtitle">Data through today</p>
            </header>

            <div className="admin-analytics__stats">
                <StatCardTrend
                    label="TOTAL MEMBERS"
                    value={overview.totalMembers.toLocaleString()}
                    changePct={overview.totalMembersChangePct}
                    changeLabel="vs last month"
                    color="accent"
                />
                <StatCardTrend
                    label="SESSIONS THIS MONTH"
                    value={overview.sessionsThisMonth.toLocaleString()}
                    changePct={overview.sessionsThisMonthChangePct}
                    changeLabel="vs last month"
                    color="info"
                />
                <StatCardTrend
                    label="AVG OCCUPANCY"
                    value={`${overview.avgOccupancyPct}%`}
                    changePct={overview.avgOccupancyChangePct}
                    changeLabel="across all classes"
                    color="warning"
                />
                <StatCardTrend
                    label="MONTHLY REVENUE"
                    value={`$${overview.monthlyRevenue.toLocaleString()}`}
                    changePct={overview.monthlyRevenueChangePct}
                    changeLabel="vs last month"
                    color="accent"
                />
            </div>

            <div className="admin-analytics__mid">
                <RevenueTrendChart data={trend} />
                <RevenueBreakdownDonut summary={revenueSummary} />
            </div>

            <OccupancyBarChart data={occupancy} />

            <div className="admin-analytics__rankings">
                <TopRankingChart
                    title="Top Classes by Enrollment"
                    subtitle="All-time top disciplines by total sign-ups"
                    data={enrollmentRankingData}
                    barColor="var(--color-info)"
                />
                <TopRankingChart
                    title="Top Rated Sessions"
                    subtitle="All-time highest rated sessions"
                    data={ratedRankingData}
                    barColor="var(--color-warning)"
                />
            </div>
        </div>
    );
};

export default AdminAnalytics;