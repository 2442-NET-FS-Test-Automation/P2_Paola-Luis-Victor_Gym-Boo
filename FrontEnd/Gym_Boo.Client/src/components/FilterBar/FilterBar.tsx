import { LayoutGrid, List } from "lucide-react";
import type { DateOption } from "../../types";
import "./FilterBar.css";

interface FilterBarProps {
    disciplines: string[];
    selectedDiscipline: string;
    onDisciplineChange: (value: string) => void;
    dateOptions: DateOption[];
    selectedDate: string;
    onDateChange: (value: string) => void;
    availableOnly: boolean;
    onAvailableOnlyChange: (value: boolean) => void;
}

const FilterBar = ({
    disciplines,
    selectedDiscipline,
    onDisciplineChange,
    dateOptions,
    selectedDate,
    onDateChange,
    availableOnly,
    onAvailableOnlyChange,
}: FilterBarProps) => {
    return (
        <div className="filter-bar">
            <select
                value={selectedDiscipline}
                onChange={(e) => onDisciplineChange(e.target.value)}
            >
                <option value="">All Types</option>
                {disciplines.map((d) => (
                    <option key={d} value={d}>
                        {d}
                    </option>
                ))}
            </select>

            <select value={selectedDate} onChange={(e) => onDateChange(e.target.value)}>
                <option value="">All Dates</option>
                {dateOptions.map((d) => (
                    <option key={d.value} value={d.value}>
                        {d.label}
                    </option>
                ))}
            </select>

            <label className="filter-bar__toggle">
                <input
                    type="checkbox"
                    checked={availableOnly}
                    onChange={(e) => onAvailableOnlyChange(e.target.checked)}
                />
                <span className="filter-bar__switch" />
                <span>Available only</span>
            </label>

            <div className="filter-bar__view">
                <button type="button" className="is-active" aria-label="Grid view">
                    <LayoutGrid size={16} />
                </button>
                <button type="button" disabled aria-label="List view (coming soon)">
                    <List size={16} />
                </button>
            </div>
        </div>
    );
};

export default FilterBar;