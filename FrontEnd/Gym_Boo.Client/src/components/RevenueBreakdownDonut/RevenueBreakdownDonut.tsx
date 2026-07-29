import { ResponsiveContainer, PieChart, Pie, Cell } from "recharts";
import type { RevenueSummary } from "../../types";
import "./RevenueBreakdownDonut.css";

interface RevenueBreakdownDonutProps {
    summary: RevenueSummary;
}

const RevenueBreakdownDonut = ({ summary }: RevenueBreakdownDonutProps) => {
    const data = [
        {
            name: "Subscriptions",
            value: summary.subscriptionRevenue,
            color: "var(--color-accent)",
        },
        {
            name: "Cancellation Fees",
            value: summary.cancellationRevenue,
            color: "var(--color-info)",
        },
    ];

    const getPct = (value: number) =>
        summary.totalRevenue > 0
            ? Math.round((value / summary.totalRevenue) * 100)
            : 0;

    return (
        <div className="revenue-breakdown">
            <div className="revenue-breakdown__header">
                <h2>Revenue Breakdown</h2>
                <p>${summary.totalRevenue.toLocaleString()} total</p>
            </div>

            <div className="revenue-breakdown__chart">
                <ResponsiveContainer width="100%" height={220}>
                    <PieChart>
                        <Pie
                            data={data}
                            dataKey="value"
                            innerRadius={65}
                            outerRadius={95}
                            paddingAngle={3}
                            stroke="none"
                        >
                            {data.map((entry) => (
                                <Cell key={entry.name} fill={entry.color} />
                            ))}
                        </Pie>
                    </PieChart>
                </ResponsiveContainer>
            </div>

            <ul className="revenue-breakdown__legend">
                {data.map((entry) => (
                    <li key={entry.name}>
                        <span className="revenue-breakdown__legend-left">
                            <span
                                className="revenue-breakdown__dot"
                                style={{ backgroundColor: entry.color }}
                            />
                            {entry.name}
                        </span>
                        <span className="revenue-breakdown__legend-right">
                            <span>${entry.value.toLocaleString()}</span>
                            <strong>{getPct(entry.value)}%</strong>
                        </span>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default RevenueBreakdownDonut;