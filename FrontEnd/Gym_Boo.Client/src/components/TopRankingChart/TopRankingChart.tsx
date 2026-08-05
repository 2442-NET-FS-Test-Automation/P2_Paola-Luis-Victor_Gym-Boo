import {
    ResponsiveContainer,
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
} from "recharts";
import type { RankingItem } from "../../types";
import "./TopRankingChart.css";

interface TopRankingChartProps {
    title: string;
    subtitle: string;
    data: RankingItem[];
    barColor?: string;
    emptyMessage?: string;
}

const TopRankingChart = ({
    title,
    subtitle,
    data,
    barColor = "var(--color-accent)",
    emptyMessage = "No data available yet.",
}: TopRankingChartProps) => {
    const chartHeight = Math.max(data.length * 44, 120);

    return (
        <div className="top-ranking-chart">
            <div className="top-ranking-chart__header">
                <h2>{title}</h2>
                <p>{subtitle}</p>
            </div>

            {data.length === 0 ? (
                <p className="top-ranking-chart__empty">{emptyMessage}</p>
            ) : (
                <ResponsiveContainer width="100%" height={chartHeight}>
                    <BarChart
                        data={data}
                        layout="vertical"
                        margin={{ top: 0, right: 30, left: 0, bottom: 0 }}
                    >
                        <CartesianGrid stroke="var(--color-border-soft)" horizontal={false} />
                        <XAxis type="number" hide />
                        <YAxis
                            type="category"
                            dataKey="label"
                            width={150}
                            stroke="var(--color-text-muted)"
                            tick={{ fontSize: 12 }}
                            axisLine={false}
                            tickLine={false}
                        />
                        <Bar dataKey="value" fill={barColor} radius={[0, 4, 4, 0]} maxBarSize={22}>
                            {/* label del valor al final de cada barra */}
                        </Bar>
                    </BarChart>
                </ResponsiveContainer>
            )}

            {data.length > 0 && (
                <ul className="top-ranking-chart__list">
                    {data.map((item, index) => (
                        <li key={item.id}>
                            <span className="top-ranking-chart__rank">#{index + 1}</span>
                            <div className="top-ranking-chart__labels">
                                <span className="top-ranking-chart__label">{item.label}</span>
                                {item.subLabel && (
                                    <span className="top-ranking-chart__sublabel">{item.subLabel}</span>
                                )}
                            </div>
                            <span className="top-ranking-chart__value">{item.displayValue}</span>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default TopRankingChart;