import {
    ResponsiveContainer,
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
} from "recharts";
import type { RevenueTrendPoint } from "../../types";
import "./RevenueTrendChart.css";

interface RevenueTrendChartProps {
    data: RevenueTrendPoint[];
}

const RevenueTrendChart = ({ data }: RevenueTrendChartProps) => {
    return (
        <div className="revenue-trend-chart">
            <div className="revenue-trend-chart__header">
                <div>
                    <h2>Revenue Trend</h2>
                    <p>Monthly revenue · {data[0]?.month} – {data[data.length - 1]?.month} 2026</p>
                </div>
                <div className="revenue-trend-chart__legend">
                    <span className="revenue-trend-chart__legend-item">
                        <span className="revenue-trend-chart__dot revenue-trend-chart__dot--revenue" />
                        Revenue
                    </span>
                    <span className="revenue-trend-chart__legend-item">
                        <span className="revenue-trend-chart__dot revenue-trend-chart__dot--members" />
                        Members
                    </span>
                </div>
            </div>

            <ResponsiveContainer width="100%" height={280}>
                <LineChart data={data} margin={{ top: 10, right: 10, left: -10, bottom: 0 }}>
                    <CartesianGrid stroke="var(--color-border-soft)" vertical={false} />
                    <XAxis
                        dataKey="month"
                        stroke="var(--color-text-subtle)"
                        tick={{ fontSize: 12 }}
                        axisLine={false}
                        tickLine={false}
                    />
                    <YAxis
                        yAxisId="revenue"
                        stroke="var(--color-text-subtle)"
                        tick={{ fontSize: 12 }}
                        tickFormatter={(v) => `$${v / 1000}k`}
                        axisLine={false}
                        tickLine={false}
                    />
                    <YAxis
                        yAxisId="members"
                        orientation="right"
                        stroke="var(--color-text-subtle)"
                        tick={{ fontSize: 12 }}
                        axisLine={false}
                        tickLine={false}
                    />
                    <Tooltip
                        contentStyle={{
                            backgroundColor: "var(--color-bg-elevated)",
                            border: "1px solid var(--color-border)",
                            borderRadius: 8,
                            fontSize: 12,
                        }}
                    />
                    <Line
                        yAxisId="revenue"
                        type="monotone"
                        dataKey="revenue"
                        stroke="var(--color-info)"
                        strokeWidth={2}
                        dot={{ r: 4, fill: "var(--color-info)" }}
                    />
                    <Line
                        yAxisId="members"
                        type="monotone"
                        dataKey="members"
                        stroke="var(--color-accent)"
                        strokeWidth={2}
                        dot={{ r: 4, fill: "var(--color-accent)" }}
                    />
                </LineChart>
            </ResponsiveContainer>
        </div>
    );
};

export default RevenueTrendChart;