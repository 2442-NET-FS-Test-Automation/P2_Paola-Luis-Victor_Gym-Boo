import { useEffect, useMemo, useState } from "react";
import { getReservations } from "../../api/reservations";
import type { Reservation, ReservationsResponse } from "../../types";
import { useCurrentUser } from "../../components/SideBar/useCurrentUser";
import { isCancelled } from "../../utils/statusStyles";
import Tabs, { type TabItem } from "../../components/Tabs/Tabs";
import BookingsTable from "../../components/BookingTable/BookingTable";
import ConfirmCancelModal from "../../components/ConfirmCancelModal/ConfirmCancelModal";
import "./MyBookings.css";

type TabKey = "upcoming" | "past" | "cancelled";

const MyBookings = () => {
    const user = useCurrentUser();

    const [data, setData] = useState<ReservationsResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [activeTab, setActiveTab] = useState<TabKey>("upcoming");
    const [reservationToCancel, setReservationToCancel] = useState<Reservation | null>(
        null
    );

    useEffect(() => {
        let cancelled = false;
        setLoading(true);
        setError(null);

        getReservations(user.id)
            .then((res) => {
                if (!cancelled) setData(res);
            })
            .catch(() => {
                if (!cancelled) setError("No pudimos cargar tus reservas.");
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });

        return () => {
            cancelled = true;
        };
    }, [user.id]);

    const { upcoming, past, cancelledList } = useMemo(() => {
        const upcomingRaw = data?.upcoming ?? [];
        const pastRaw = data?.past ?? [];

        const upcoming = upcomingRaw.filter((r) => !isCancelled(r.status));
        const past = pastRaw.filter((r) => !isCancelled(r.status));
        const cancelledList: Reservation[] = [...upcomingRaw, ...pastRaw].filter((r) =>
            isCancelled(r.status)
        );

        return { upcoming, past, cancelledList };
    }, [data]);

    const tabs: TabItem[] = [
        { key: "upcoming", label: "Upcoming", count: upcoming.length },
        { key: "past", label: "Past Classes", count: past.length },
        { key: "cancelled", label: "Cancelled", count: cancelledList.length },
    ];

    const handleCancelled = (enrollmentId: number) => {
        setData((prev) => {
            if (!prev) return prev;
            const moveToCancelled = (list: Reservation[]) =>
                list.map((r) =>
                    r.enrollmentId === enrollmentId ? { ...r, status: "Canceled" } : r
                );
            return {
                upcoming: moveToCancelled(prev.upcoming),
                past: moveToCancelled(prev.past),
            };
        });
    };

    return (
        <div className="my-bookings">
            <header className="my-bookings__header">
                <h1>My Bookings</h1>
            </header>

            <div className="my-bookings__panel">
                <Tabs
                    tabs={tabs}
                    activeKey={activeTab}
                    onChange={(key) => setActiveTab(key as TabKey)}
                />

                {loading && <p className="my-bookings__status">Loading…</p>}
                {error && <p className="my-bookings__status my-bookings__status--error">{error}</p>}

                {!loading && !error && (
                    <div className="my-bookings__table-wrapper">
                        {activeTab === "upcoming" && (
                            <BookingsTable
                                reservations={upcoming}
                                variant="upcoming"
                                onCancelClick={setReservationToCancel}
                            />
                        )}
                        {activeTab === "past" && (
                            <BookingsTable reservations={past} variant="past" />
                        )}
                        {activeTab === "cancelled" && <BookingsTable reservations={cancelledList} />}
                    </div>
                )}
            </div>

            {reservationToCancel && (
                <ConfirmCancelModal
                    reservation={reservationToCancel}
                    onClose={() => setReservationToCancel(null)}
                    onCancelled={handleCancelled}
                />
            )}
        </div>
    );
};

export default MyBookings;