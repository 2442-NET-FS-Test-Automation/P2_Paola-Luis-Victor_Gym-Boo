import {
  useEffect,
  useMemo,
  useState,
} from "react";

import {
  CalendarDays,
  ChevronLeft,
  ChevronRight,
  List,
} from "lucide-react";

import { getClasses } from "../../../api/sessions";
import type { ApiClassSession } from "../../../types";

import "./AdminSessions.css";

const DAYS_IN_WEEK = 7;
const START_HOUR = 6;
const END_HOUR = 21;
const HOUR_HEIGHT = 74;

type SessionsView = "week" | "list";

const getMonday = (date: Date): Date => {
  const result = new Date(date);

  const currentDay = result.getDay();

  const difference =
    currentDay === 0
      ? -6
      : 1 - currentDay;

  result.setDate(
    result.getDate() + difference
  );

  result.setHours(0, 0, 0, 0);

  return result;
};

const addDays = (
  date: Date,
  amount: number
): Date => {
  const result = new Date(date);

  result.setDate(
    result.getDate() + amount
  );

  return result;
};

const parseSessionDate = (
  value: string
): Date => {
  const match = value.match(
    /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})(?::(\d{2}))?/
  );

  if (!match) {
    return new Date(value);
  }

  const [
    ,
    year,
    month,
    day,
    hour,
    minute,
    second = "0",
  ] = match;

  return new Date(
    Number(year),
    Number(month) - 1,
    Number(day),
    Number(hour),
    Number(minute),
    Number(second)
  );
};

const isSameDay = (
  first: Date,
  second: Date
): boolean => {
  return (
    first.getFullYear() ===
      second.getFullYear() &&
    first.getMonth() ===
      second.getMonth() &&
    first.getDate() ===
      second.getDate()
  );
};

const getMinutesFromStart = (
  date: Date
): number => {
  return (
    (date.getHours() - START_HOUR) * 60 +
    date.getMinutes()
  );
};

const getSessionColorClass = (
  session: ApiClassSession
): string => {
  const discipline =
    session.discipline.toLowerCase();

  const enrolled =
    session.totalSpots -
    session.availableSpots;

  const occupancy =
    session.totalSpots <= 0
      ? 0
      : enrolled / session.totalSpots;

  if (occupancy >= 0.9) {
    return "calendar-session--red";
  }

  if (discipline.includes("yoga")) {
    return "calendar-session--green";
  }

  if (discipline.includes("mobility")) {
    return "calendar-session--green";
  }

  if (discipline.includes("pilates")) {
    return "calendar-session--purple";
  }

  if (discipline.includes("crossfit")) {
    return "calendar-session--orange";
  }

  if (discipline.includes("boxing")) {
    return "calendar-session--red";
  }

  if (discipline.includes("hiit")) {
    return "calendar-session--red";
  }

  return "calendar-session--blue";
};

const formatWeekRange = (
  monday: Date,
  sunday: Date
): string => {
  const startFormatter =
    new Intl.DateTimeFormat("en-US", {
      month: "long",
      day: "numeric",
    });

  const endFormatter =
    new Intl.DateTimeFormat("en-US", {
      month:
        monday.getMonth() ===
        sunday.getMonth()
          ? undefined
          : "long",
      day: "numeric",
      year: "numeric",
    });

  return `${startFormatter.format(
    monday
  )} – ${endFormatter.format(sunday)}`;
};

const AdminSessions = () => {
  const [allSessions, setAllSessions] =
    useState<ApiClassSession[]>([]);

  const [weekStart, setWeekStart] =
    useState<Date>(
      getMonday(new Date())
    );

  const [view, setView] =
    useState<SessionsView>("week");

  const [loading, setLoading] =
    useState(true);

  const [error, setError] = useState<
    string | null
  >(null);

  useEffect(() => {
    let cancelled = false;

    setLoading(true);
    setError(null);

    getClasses({
      past: true,
    })
      .then((result) => {
        if (!cancelled) {
          setAllSessions(result);
        }
      })
      .catch(() => {
        if (!cancelled) {
          setError(
            "The scheduled sessions could not be loaded."
          );
        }
      })
      .finally(() => {
        if (!cancelled) {
          setLoading(false);
        }
      });

    return () => {
      cancelled = true;
    };
  }, []);

  const weekDays = useMemo(() => {
    return Array.from(
      {
        length: DAYS_IN_WEEK,
      },
      (_, index) =>
        addDays(weekStart, index)
    );
  }, [weekStart]);

  const weekEnd = weekDays[6];

  const weekSessions = useMemo(() => {
    const nextWeek = addDays(
      weekStart,
      DAYS_IN_WEEK
    );

    return allSessions
      .filter((session) => {
        const sessionStart = parseSessionDate(
          session.startTime
        );

        return (
          sessionStart >= weekStart &&
          sessionStart < nextWeek
        );
      })
      .sort((first, second) => {
        return (
          parseSessionDate(
            first.startTime
          ).getTime() -
          parseSessionDate(
            second.startTime
          ).getTime()
        );
      });
  }, [allSessions, weekStart]);

  const statistics = useMemo(() => {
    const totalCapacity =
      weekSessions.reduce(
        (total, session) =>
          total + session.totalSpots,
        0
      );

    const totalEnrolled =
      weekSessions.reduce(
        (total, session) => {
          return (
            total +
            (session.totalSpots -
              session.availableSpots)
          );
        },
        0
      );

    const occupancy =
      totalCapacity === 0
        ? 0
        : Math.round(
            (totalEnrolled /
              totalCapacity) *
              100
          );

    const fullSessions =
      weekSessions.filter(
        (session) =>
          session.availableSpots <= 0
      ).length;

    return {
      totalSessions:
        weekSessions.length,
      totalEnrolled,
      occupancy,
      fullSessions,
    };
  }, [weekSessions]);

  const hours = useMemo(() => {
    return Array.from(
      {
        length:
          END_HOUR - START_HOUR + 1,
      },
      (_, index) =>
        START_HOUR + index
    );
  }, []);

  const goToPreviousWeek = () => {
    setWeekStart((current) =>
      addDays(current, -7)
    );
  };

  const goToCurrentWeek = () => {
    setWeekStart(
      getMonday(new Date())
    );
  };

  const goToNextWeek = () => {
    setWeekStart((current) =>
      addDays(current, 7)
    );
  };

  return (
    <div className="sessions-page">
      <header className="sessions-header">
        <div>
          <p className="admin-eyebrow">
            ADMIN PANEL
          </p>

          <h1>SESSION SCHEDULER</h1>

          <p className="admin-subtitle">
            Week of{" "}
            {formatWeekRange(
              weekStart,
              weekEnd
            )}{" "}
            · {weekSessions.length} sessions
            scheduled
          </p>
        </div>

        <div className="sessions-header__actions">
          <div className="sessions-view-toggle">
            <button
              type="button"
              className={
                view === "week"
                  ? "is-active"
                  : ""
              }
              onClick={() =>
                setView("week")
              }
            >
              <CalendarDays size={15} />
              Week
            </button>

            <button
              type="button"
              className={
                view === "list"
                  ? "is-active"
                  : ""
              }
              onClick={() =>
                setView("list")
              }
            >
              <List size={15} />
              List
            </button>
          </div>
        </div>
      </header>

      <section className="sessions-stats">
        <article className="sessions-stat">
          <strong className="sessions-stat--lime">
            {statistics.totalSessions}
          </strong>

          <span>Sessions this week</span>
        </article>

        <article className="sessions-stat">
          <strong className="sessions-stat--blue">
            {statistics.totalEnrolled}
          </strong>

          <span>Total enrolled</span>
        </article>

        <article className="sessions-stat">
          <strong className="sessions-stat--orange">
            {statistics.occupancy}%
          </strong>

          <span>Overall occupancy</span>
        </article>

        <article className="sessions-stat">
          <strong className="sessions-stat--red">
            {statistics.fullSessions}
          </strong>

          <span>Full sessions</span>
        </article>
      </section>

      <section className="week-navigation">
        <button
          type="button"
          onClick={goToPreviousWeek}
        >
          <ChevronLeft size={17} />
          Previous
        </button>

        <button
          type="button"
          onClick={goToCurrentWeek}
        >
          This week
        </button>

        <button
          type="button"
          onClick={goToNextWeek}
        >
          Next
          <ChevronRight size={17} />
        </button>
      </section>

      {error && (
        <p className="admin-error">
          {error}
        </p>
      )}

      {loading && (
        <p className="admin-state">
          Loading sessions...
        </p>
      )}

      {!loading &&
        !error &&
        view === "week" && (
          <section className="week-calendar">
            <div className="week-calendar__header">
              <div className="week-calendar__corner" />

              {weekDays.map((day) => {
                const daySessions =
                  weekSessions.filter(
                    (session) =>
                      isSameDay(
                        parseSessionDate(
                          session.startTime
                        ),
                        day
                      )
                  );

                const isToday = isSameDay(
                  day,
                  new Date()
                );

                return (
                  <div
                    key={day.toISOString()}
                    className={`week-day-header ${
                      isToday
                        ? "is-today"
                        : ""
                    }`}
                  >
                    <span>
                      {new Intl.DateTimeFormat(
                        "en-US",
                        {
                          weekday: "short",
                        }
                      )
                        .format(day)
                        .toUpperCase()}
                    </span>

                    <strong>
                      {day.getDate()}
                    </strong>

                    <small>
                      {daySessions.length}{" "}
                      {daySessions.length === 1
                        ? "session"
                        : "sessions"}
                    </small>
                  </div>
                );
              })}
            </div>

            <div className="week-calendar__body">
              <div className="calendar-time-column">
                {hours.map((hour) => (
                  <div
                    key={hour}
                    className="calendar-time"
                    style={{
                      height: HOUR_HEIGHT,
                    }}
                  >
                    {new Intl.DateTimeFormat(
                      "en-US",
                      {
                        hour: "numeric",
                      }
                    ).format(
                      new Date(
                        2026,
                        0,
                        1,
                        hour
                      )
                    )}
                  </div>
                ))}
              </div>

              {weekDays.map((day) => {
                const daySessions =
                  weekSessions.filter(
                    (session) =>
                      isSameDay(
                        parseSessionDate(
                          session.startTime
                        ),
                        day
                      )
                  );

                return (
                  <div
                    key={day.toISOString()}
                    className="calendar-day-column"
                    style={{
                      height:
                        hours.length *
                        HOUR_HEIGHT,
                    }}
                  >
                    {hours.map((hour) => (
                      <div
                        key={hour}
                        className="calendar-hour-line"
                        style={{
                          top:
                            (hour -
                              START_HOUR) *
                            HOUR_HEIGHT,
                        }}
                      />
                    ))}

                    {daySessions.map(
                      (session) => {
                        const start =
                          parseSessionDate(
                            session.startTime
                          );

                        const end =
                          parseSessionDate(
                            session.endTime
                          );

                        const startMinutes =
                          getMinutesFromStart(
                            start
                          );

                        const durationMinutes =
                          Math.max(
                            45,
                            (end.getTime() -
                              start.getTime()) /
                              60000
                          );

                        const top =
                          (startMinutes / 60) *
                          HOUR_HEIGHT;

                        const height =
                          Math.max(
                            58,
                            (durationMinutes /
                              60) *
                              HOUR_HEIGHT -
                              6
                          );

                        const enrolled =
                          session.totalSpots -
                          session.availableSpots;

                        const occupancy =
                          session.totalSpots ===
                          0
                            ? 0
                            : Math.round(
                                (enrolled /
                                  session.totalSpots) *
                                  100
                              );

                        const initials =
                          session.instructorName
                            .split(" ")
                            .filter(Boolean)
                            .map(
                              (part) =>
                                part[0]
                            )
                            .join("")
                            .slice(0, 2)
                            .toUpperCase();

                        const firstName =
                          session.instructorName
                            .split(" ")
                            .filter(Boolean)[0] ??
                          session.instructorName;

                        return (
                          <article
                            key={session.id}
                            className={`calendar-session ${getSessionColorClass(
                              session
                            )}`}
                            style={{
                              top,
                              height,
                            }}
                            title={`${session.className} — ${session.instructorName}`}
                          >
                            <strong>
                              {
                                session.className
                              }
                            </strong>

                            <span className="calendar-session__instructor">
                              <small>
                                {initials}
                              </small>

                              {firstName}
                            </span>

                            <div className="calendar-session__progress">
                              <span
                                style={{
                                  width: `${Math.min(
                                    occupancy,
                                    100
                                  )}%`,
                                }}
                              />
                            </div>

                            <span className="calendar-session__capacity">
                              {enrolled}/
                              {
                                session.totalSpots
                              }
                            </span>
                          </article>
                        );
                      }
                    )}
                  </div>
                );
              })}
            </div>
          </section>
        )}

      {!loading &&
        !error &&
        view === "list" && (
          <section className="sessions-list">
            {weekSessions.length === 0 && (
              <p className="admin-state">
                There are no sessions
                scheduled for this week.
              </p>
            )}

            {weekSessions.map(
              (session) => {
                const enrolled =
                  session.totalSpots -
                  session.availableSpots;

                return (
                  <article
                    key={session.id}
                    className="sessions-list__item"
                  >
                    <time>
                      {new Intl.DateTimeFormat(
                        "en-US",
                        {
                          weekday: "short",
                          month: "short",
                          day: "numeric",
                          hour: "numeric",
                          minute: "2-digit",
                        }
                      ).format(
                        parseSessionDate(
                          session.startTime
                        )
                      )}
                    </time>

                    <div>
                      <strong>
                        {session.className}
                      </strong>

                      <span>
                        {session.discipline}
                      </span>
                    </div>

                    <span>
                      {session.instructorName}
                    </span>

                    <span>
                      {session.location}
                    </span>

                    <span>
                      {enrolled}/
                      {session.totalSpots}
                    </span>
                  </article>
                );
              }
            )}
          </section>
        )}
    </div>
  );
};

export default AdminSessions;
