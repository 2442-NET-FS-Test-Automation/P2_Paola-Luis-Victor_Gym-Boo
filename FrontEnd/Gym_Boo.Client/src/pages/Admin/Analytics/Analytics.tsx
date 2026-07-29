import { useEffect, useState } from "react";
import {
    getAdminOverviewStats,
    getRevenueTrend,
    getClassOccupancyRates,
    getRevenueSummary
} from "../../../api/admin";
import type {
    AdminOverviewStats,
    ClassOccupancyRate,
    RevenueSummary,
    RevenueTrendPoint,
} from "../../../types";
import StatCardTrend from "../../../components/StatCardTrend/StatCardTrend";
import RevenueTrendChart from "../../../components/RevenueTrendChart/RevenueTrendChart";
import RevenueBreakdownDonut from "../../../components/RevenueBreakdownDonut/RevenueBreakdownDonut";
import OccupancyBarChart from "../../../components/OccupancyBarChart/OccupancyBarChart";
import "./Analytics.css";

const AdminAnalytics = () => {
    const [overview, setOverview] = useState<AdminOverviewStats | null>(null);
    const [trend, setTrend] = useState<RevenueTrendPoint[]>([]);
    const [occupancy, setOccupancy] = useState<ClassOccupancyRate[]>([]);
    const [revenueSummary, setRevenueSummary] = useState<RevenueSummary | null>(null);

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
        ])
            .then(([overviewData, trendData, occupancyData, revenueData]) => {
                if (cancelled) return;
                setOverview(overviewData);
                setTrend(trendData);
                setOccupancy(occupancyData);
                setRevenueSummary(revenueData);
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
        </div>
    );
};

export default AdminAnalytics;