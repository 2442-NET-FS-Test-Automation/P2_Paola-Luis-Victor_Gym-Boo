import {
    ResponsiveContainer,
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Cell,
} from "recharts";
import type { ClassOccupancyRate } from "../../types";
import "./OccupancyBarChart.css";

interface OccupancyBarChartProps {
    data: ClassOccupancyRate[];
}

const getBarColor = (pct: number): string => {
    if (pct >= 98) return "var(--color-danger)";
    if (pct >= 70) return "var(--color-warning)";
    return "var(--color-accent)";
};

const OccupancyBarChart = ({ data }: OccupancyBarChartProps) => {
    return (
        <div className="occupancy-chart">
            <div className="occupancy-chart__header">
                <div>
                    <h2>Class Occupancy Rates</h2>
                    <p>Average occupancy per class type · last 30 days</p>
                </div>
                <div className="occupancy-chart__legend">
                    <span>
                        <span className="occupancy-chart__dot" style={{ backgroundColor: "var(--color-danger)" }} />
                        Full (≥98%)
                    </span>
                    <span>
                        <span className="occupancy-chart__dot" style={{ backgroundColor: "var(--color-warning)" }} />
                        High (≥70%)
                    </span>
                    <span>
                        <span className="occupancy-chart__dot" style={{ backgroundColor: "var(--color-accent)" }} />
                        Normal
                    </span>
                </div>
            </div>

            <ResponsiveContainer width="100%" height={280}>
                <BarChart data={data} margin={{ top: 10, right: 10, left: -10, bottom: 0 }}>
                    <CartesianGrid stroke="var(--color-border-soft)" vertical={false} />
                    <XAxis
                        dataKey="discipline"
                        stroke="var(--color-text-subtle)"
                        tick={{ fontSize: 12 }}
                        axisLine={false}
                        tickLine={false}
                    />
                    <YAxis
                        stroke="var(--color-text-subtle)"
                        tick={{ fontSize: 12 }}
                        tickFormatter={(v) => `${v}%`}
                        axisLine={false}
                        tickLine={false}
                    />
                    <Bar dataKey="occupancyPct" radius={[4, 4, 0, 0]} maxBarSize={40}>
                        {data.map((entry) => (
                            <Cell key={entry.discipline} fill={getBarColor(entry.occupancyPct)} />
                        ))}
                    </Bar>
                </BarChart>
            </ResponsiveContainer>
        </div>
    );
};

export default OccupancyBarChart;