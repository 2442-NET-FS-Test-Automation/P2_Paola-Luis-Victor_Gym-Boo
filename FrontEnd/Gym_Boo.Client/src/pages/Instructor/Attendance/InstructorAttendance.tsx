import {
  useEffect,
  useMemo,
  useState,
} from "react";

import {
  Search,
} from "lucide-react";

import {
  getInstructorSessions,
  getSessionAttendance,
  toggleAttendance,
  extractAttendanceErrorMessage,
  type AttendanceRecord,
  type InstructorSession,
} from "../../../api/instructor";

import { getStoredUser } from "../../../api/auth";

import {
  formatTime,
  isSameLocalDay,
} from "../../../utils/instructorDates";

import "./InstructorAttendance.css";

type AttendanceFilter =
  | "all"
  | "present"
  | "absent";

const getRecordName = (
  record: AttendanceRecord
): string => {
  if (record.memberName) {
    return record.memberName;
  }

  return `${record.name ?? "Member"} ${
    record.lastName ?? ""
  }`.trim();
};

const isPresent = (
  record: AttendanceRecord
): boolean => {
  return (
    record.isPresent ??
    record.attended ??
    false
  );
};

const getRecordEnrollmentId = (
  record: AttendanceRecord
): number | undefined => {
  return record.enrollmentId ?? record.id;
};

const InstructorAttendance = () => {
  const user = getStoredUser();

  const [sessions, setSessions] = useState
    <InstructorSession[]>([]);

  const [selectedSessionId, setSelectedSessionId] =
    useState("");

  const [attendance, setAttendance] = useState
    <AttendanceRecord[]>([]);

  const [search, setSearch] = useState("");
  const [filter, setFilter] =
    useState<AttendanceFilter>("all");

  const [loading, setLoading] = useState(true);

  const [error, setError] = useState<
    string | null
  >(null);


  const [actionError, setActionError] = useState<
    string | null>(null);

  const [pendingIds, setPendingIds] = useState<
    Set<number>>(new Set());

  // Solo sesiones del día de hoy en el selector.
  const todaySessions = useMemo(() => {
    const today = new Date();
    return sessions.filter((session) =>
      isSameLocalDay(session.startTime, today)
    );
  }, [sessions]);

  useEffect(() => {
    if (!user) {
      return;
    }

    const loadSessions = async () => {
      try {
        const result =
          await getInstructorSessions(user.id);

        setSessions(result);

        const today = new Date();
        const todayResult = result.filter(
          (session) =>
            isSameLocalDay(session.startTime, today)
        );

        const query =
          new URLSearchParams(
            window.location.search
          ).get("sessionId");

        const queryMatchesToday =
          query &&
          todayResult.some(
            (session) =>
              session.id.toString() === query
          );

        const initialId =
          (queryMatchesToday ? query : null) ??
          todayResult[0]?.id.toString() ??
          "";

        setSelectedSessionId(initialId);
      } catch {
        setError(
          "The instructor sessions could not be loaded."
        );
      } finally {
        setLoading(false);
      }
    };

    void loadSessions();
  }, [user?.id]);

  useEffect(() => {
    if (!selectedSessionId) {
      setAttendance([]);
      return;
    }

    const loadAttendance = async () => {
      setLoading(true);
      setError(null);
      setActionError(null);

      try {
        setAttendance(
          await getSessionAttendance(
            Number(selectedSessionId)
          )
        );
      } catch {
        setError(
          "No attendance records were found for this session."
        );

        setAttendance([]);
      } finally {
        setLoading(false);
      }
    };

    void loadAttendance();
  }, [selectedSessionId]);

  const selectedSession =
    sessions.find(
      (session) =>
        session.id ===
        Number(selectedSessionId)
    ) ?? null;

  const totals = useMemo(() => {
    const present = attendance.filter(
      isPresent
    ).length;

    return {
      total: attendance.length,
      present,
      absent: attendance.length - present,
    };
  }, [attendance]);

  const filteredAttendance = useMemo(() => {
    const term = search.trim().toLowerCase();

    return attendance.filter((record) => {
      const present = isPresent(record);

      const matchesSearch =
        !term ||
        getRecordName(record)
          .toLowerCase()
          .includes(term) ||
        (
          record.memberEmail ??
          record.email ??
          ""
        )
          .toLowerCase()
          .includes(term);

      const matchesFilter =
        filter === "all" ||
        (filter === "present" && present) ||
        (filter === "absent" && !present);

      return matchesSearch && matchesFilter;
    });
  }, [attendance, search, filter]);

  const percentage =
    totals.total === 0
      ? 0
      : Math.round(
          (totals.present / totals.total) *
            100
        );

  const handleToggleAttendance = async (
    record: AttendanceRecord
  ) => {
    const enrollmentId = getRecordEnrollmentId(record);
    if (enrollmentId === undefined) return;
    if (pendingIds.has(enrollmentId)) return;

    setActionError(null);

    const currentlyPresent = isPresent(record);
    setPendingIds((prev) => new Set(prev).add(enrollmentId));

    try {
      await toggleAttendance({
        enrollmentId: enrollmentId,
        action: currentlyPresent ? "not attended" : "attended",
      });

      if (selectedSessionId) {
        const refreshedAttendance = await getSessionAttendance(
          Number(selectedSessionId)
        );

        setAttendance(refreshedAttendance);
      }
    } catch (err) {
      setActionError(extractAttendanceErrorMessage(err));
    } finally {
      setPendingIds((prev) => {
        const next = new Set(prev);
        next.delete(enrollmentId);
        return next;
      });
    }
  };

  return (
    <div className="attendance-page">
      <header className="attendance-page__hero">
        <div>
          <p className="instructor-eyebrow">
            INSTRUCTOR PORTAL
          </p>

          <h1>ATTENDANCE SHEET</h1>
        </div>
      </header>

      <section className="attendance-session-picker">
        <label htmlFor="attendance-session">
          Session
        </label>

        <select
          id="attendance-session"
          value={selectedSessionId}
          onChange={(event) =>
            setSelectedSessionId(
              event.target.value
            )
          }
        >
          <option value="">
            {todaySessions.length === 0
              ? "No sessions today"
              : "Select a session"}
          </option>

          {todaySessions.map((session) => (
            <option
              key={session.id}
              value={session.id}
            >
              {session.className} —{" "}
              {new Intl.DateTimeFormat(
                "en-US",
                {
                  month: "short",
                  day: "numeric",
                  hour: "numeric",
                  minute: "2-digit",
                }
              ).format(
                new Date(session.startTime)
              )}
            </option>
          ))}
        </select>
      </section>

      {selectedSession && (
        <section className="attendance-summary">
          <div>
            <span>CLASS</span>
            <strong>
              {selectedSession.className}
            </strong>
          </div>

          <div>
            <span>DATE &amp; TIME</span>
            <strong>
              {new Intl.DateTimeFormat(
                "en-US",
                {
                  weekday: "short",
                  month: "short",
                  day: "numeric",
                }
              ).format(
                new Date(
                  selectedSession.startTime
                )
              )}{" "}
              ·{" "}
              {formatTime(
                selectedSession.startTime
              )}
            </strong>
          </div>

          <div>
            <span>STUDIO</span>
            <strong>
              {selectedSession.location ??
                selectedSession.placeName ??
                "GymBoo"}
            </strong>
          </div>

          <div className="attendance-summary__score">
            <span>ATTENDANCE</span>

            <strong>
              {totals.present}
              <small>/{totals.total}</small>
            </strong>

            <div>
              <i
                style={{
                  width: `${percentage}%`,
                }}
              />
            </div>

            <small>
              {percentage}% present
            </small>
          </div>
        </section>
      )}

      <section className="attendance-controls">
        <label className="attendance-search">
          <Search size={18} />

          <input
            value={search}
            placeholder="Search members..."
            onChange={(event) =>
              setSearch(event.target.value)
            }
          />
        </label>

        <div className="attendance-filters">
          <button
            type="button"
            className={
              filter === "all"
                ? "is-active"
                : ""
            }
            onClick={() => setFilter("all")}
          >
            ALL ({totals.total})
          </button>

          <button
            type="button"
            className={
              filter === "present"
                ? "is-active"
                : ""
            }
            onClick={() =>
              setFilter("present")
            }
          >
            PRESENT ({totals.present})
          </button>

          <button
            type="button"
            className={
              filter === "absent"
                ? "is-active"
                : ""
            }
            onClick={() =>
              setFilter("absent")
            }
          >
            ABSENT ({totals.absent})
          </button>
        </div>
      </section>

      {error && (
        <p className="instructor-page-error">
          {error}
        </p>
      )}

      {actionError && (
        <p className="instructor-page-error">
          {actionError}
        </p>
      )}

      <section className="attendance-table">
        <div className="attendance-table__header">
          <span>#</span>
          <span>Member</span>
          <span>Contact</span>
          <span>Booked</span>
          <span>Check-in</span>
        </div>

        {loading && (
          <p className="instructor-page-state">
            Loading attendance...
          </p>
        )}

        {!loading &&
          filteredAttendance.length === 0 && (
            <p className="instructor-page-state">
              No attendance records found.
            </p>
          )}

        {!loading &&
          filteredAttendance.map(
            (record, index) => {
              const name =
                getRecordName(record);

              const initials = name
                .split(" ")
                .filter(Boolean)
                .map((part) => part[0])
                .join("")
                .slice(0, 2)
                .toUpperCase();

              const present =
                isPresent(record);

              const enrollmentId =
                getRecordEnrollmentId(record);

              const isPending =
                enrollmentId !== undefined &&
                pendingIds.has(enrollmentId);

              return (
                <div
                  key={
                    record.enrollmentId ??
                    record.id ??
                    `${name}-${index}`
                  }
                  className="attendance-table__row"
                >
                  <span>{index + 1}</span>

                  <div className="attendance-member">
                    <span>{initials}</span>
                    <strong>{name}</strong>
                  </div>

                  <span>
                    {record.memberEmail ??
                      record.email ??
                      "No email"}
                  </span>

                  <span>
                    {record.bookedAt ??
                    record.createdAt
                      ? new Intl.DateTimeFormat(
                          "en-US",
                          {
                            month: "short",
                            day: "numeric",
                          }
                        ).format(
                          new Date(
                            record.bookedAt ??
                              record.createdAt!
                          )
                        )
                      : "—"}
                  </span>

                  <button
                    type="button"
                    className={`attendance-checkbox ${
                      present ? "is-present" : ""
                    } ${isPending ? "is-loading" : ""}`}
                    disabled={
                      enrollmentId === undefined ||
                      isPending
                    }
                    onClick={() =>
                      handleToggleAttendance(record)
                    }
                    aria-label={
                      present
                        ? "Mark as absent"
                        : "Mark as present"
                    }
                  >
                    {isPending ? "…" : present ? "✓" : ""}
                  </button>
                </div>
              );
            }
          )}
      </section>
    </div>
  );
};

export default InstructorAttendance;