import { useEffect, useMemo, useState } from "react";
import { getClasses } from "../../api/sessions";
import type { ApiClassSession } from "../../types";
import { useActiveRole } from "../../components/SideBar/roleConfig";
import SearchBar from "../../components/SearchBar/SearchBar";
import FilterBar from "../../components/FilterBar/FilterBar";
import SessionCard from "../../components/SessionCard/SessionCard";
import { getUpcomingDateOptions } from "../../utils/dateOptions";
import { localDateStringToUtcIso } from "../../utils/timeZone";
import "./Discover.css";

const Discover = () => {
    const activeRole = useActiveRole();
    const dateOptions = useMemo(() => getUpcomingDateOptions(), []);

    const [allSessions, setAllSessions] = useState<ApiClassSession[]>([]);
    const [disciplineOptions, setDisciplineOptions] = useState<string[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [discipline, setDiscipline] = useState("");
    const [date, setDate] = useState("");
    const [search, setSearch] = useState("");
    const [availableOnly, setAvailableOnly] = useState(false);

    useEffect(() => {
        let cancelled = false;
        setLoading(true);
        setError(null);

        getClasses({
            discipline: discipline || undefined,
            date: date ? localDateStringToUtcIso(date) : undefined,
            past: !availableOnly, // checked -> past=false (only upcoming); unchecked -> past=true (includes expired)
        })
            .then((data) => {
                if (cancelled) return;
                setAllSessions(data);
                setDisciplineOptions((prev) =>
                    prev.length ? prev : Array.from(new Set(data.map((s) => s.discipline)))
                );
            })
            .catch(() => {
                if (!cancelled) setError("We were unable to load the classes. Please try again.");
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });

        return () => {
            cancelled = true;
        };
    }, [discipline, date, availableOnly]);

    const filteredSessions = useMemo(() => {
        return allSessions.filter((s) => {
            const term = search.trim().toLowerCase();
            const matchesSearch =
                !term ||
                s.className.toLowerCase().includes(term) ||
                s.instructorName.toLowerCase().includes(term);

            const matchesAvailability = !availableOnly || s.availableSpots > 0;

            return matchesSearch && matchesAvailability;
        });
    }, [allSessions, search, availableOnly]);

    return (
        <div className="discover-page">
            <header className="discover-page__header">
                <p className="discover-page__eyebrow">{activeRole.portalLabel}</p>
                <h1>Discover Classes</h1>
                <p className="discover-page__subtitle">
                    {filteredSessions.length} sessions found
                </p>
            </header>

            <div className="discover-page__controls">
                <SearchBar
                    value={search}
                    onChange={setSearch}
                    placeholder="Search classes or instructors..."
                />
                <FilterBar
                    disciplines={disciplineOptions}
                    selectedDiscipline={discipline}
                    onDisciplineChange={setDiscipline}
                    dateOptions={dateOptions}
                    selectedDate={date}
                    onDateChange={setDate}
                    availableOnly={availableOnly}
                    onAvailableOnlyChange={setAvailableOnly}
                />
            </div>

            {loading && <p className="discover-page__status">Loading classes…</p>}
            {error && (
                <p className="discover-page__status discover-page__status--error">
                    {error}
                </p>
            )}
            {!loading && !error && filteredSessions.length === 0 && (
                <p className="discover-page__status">No classes match your filters.</p>
            )}

            {!loading && !error && filteredSessions.length > 0 && (
                <div className="discover-page__grid">
                    {filteredSessions.map((session) => (
                        <SessionCard key={session.id} session={session} />
                    ))}
                </div>
            )}
        </div>
    );
};

export default Discover;