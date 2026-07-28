import {
  useEffect,
  useMemo,
  useState,
} from "react";

import { useNavigate } from "react-router-dom";
import axios from "axios";

import {
  getInstructor,
  getInstructorSessions,
  type InstructorDetails,
  type InstructorSession,
} from "../../../api/instructor";

import { getStoredUser } from "../../../api/auth";

import {
  addDays,
  formatFullDate,
  formatTime,
  getCountdown,
  getMonday,
  isSameLocalDay,
} from "../../../utils/instructorDates";

import "./InstructorDashboard.css";

const getApiError = (
  error: unknown,
  fallback: string
): string => {
  if (!axios.isAxiosError(error)) {
    return fallback;
  }

  const data = error.response?.data;

  if (typeof data === "string") {
    return data;
  }

  return data?.message ?? fallback;
};

const InstructorDashboard = () => {
  const navigate = useNavigate();
  const authenticatedUser = getStoredUser();

  const [instructor, setInstructor] =
    useState<InstructorDetails | null>(null);

  const [sessions, setSessions] = useState<
    InstructorSession[]
  >([]);

  const [loading, setLoading] = useState(true);

  const [error, setError] = useState<
    string | null
  >(null);

  useEffect(() => {
    if (!authenticatedUser) {
      return;
    }

    const loadDashboard = async () => {
      setLoading(true);
      setError(null);

      try {
        const [
          instructorResult,
          sessionsResult,
        ] = await Promise.all([
          getInstructor(authenticatedUser.id),
          getInstructorSessions(
            authenticatedUser.id
          ),
        ]);

        setInstructor(instructorResult);

        setSessions(
          [...sessionsResult].sort(
            (first, second) =>
              new Date(
                first.startTime
              ).getTime() -
              new Date(
                second.startTime
              ).getTime()
          )
        );
      } catch (error: unknown) {
        setError(
          getApiError(
            error,
            "The instructor dashboard could not be loaded."
          )
        );
      } finally {
        setLoading(false);
      }
    };

    void loadDashboard();
  }, [authenticatedUser?.id]);

  const dashboard = useMemo(() => {
    const now = new Date();

    const weekStart = getMonday(now);
    const nextWeek = addDays(weekStart, 7);

    const todaySessions = sessions.filter(
      (session) =>
        isSameLocalDay(
          new Date(session.startTime),
          now
        )
    );

    const weekSessions = sessions.filter(
      (session) => {
        const start = new Date(
          session.startTime
        );

        return (
          start >= weekStart &&
          start < nextWeek
        );
      }
    );

    const remainingWeekSessions =
      weekSessions.filter(
        (session) =>
          new Date(session.endTime) > now
      );

    const upcomingSessions = sessions.filter(
      (session) =>
        new Date(session.startTime) > now
    );

    const nextSession =
      upcomingSessions[0] ?? null;

    const studentsToday =
      todaySessions.reduce(
        (total, session) =>
          total +
          Math.max(
            0,
            session.totalSpots -
              session.availableSpots
          ),
        0
      );

    return {
      todaySessions,
      weekSessions,
      remainingWeekSessions,
      upcomingSessions:
        upcomingSessions.slice(0, 3),
      nextSession,
      studentsToday,
    };
  }, [sessions]);

  if (loading) {
    return (
      <p className="instructor-page-state">
        Loading dashboard...
      </p>
    );
  }

  return (
    <div className="instructor-dashboard">
      <header className="instructor-dashboard__hero">
        <div>
          <p className="instructor-eyebrow">
            INSTRUCTOR PORTAL
          </p>

          <h1>
            GOOD MORNING,{" "}
            {(
              instructor?.name ??
              authenticatedUser?.name ??
              "INSTRUCTOR"
            ).toUpperCase()}
          </h1>

          <p className="instructor-dashboard__subtitle">
            {formatFullDate(new Date())} · You
            have{" "}
            {dashboard.todaySessions.length}{" "}
            classes today
          </p>
        </div>

        <button
          type="button"
          className="instructor-outline-button"
          onClick={() =>
            navigate("/coach/schedule")
          }
        >
          View Full Schedule →
        </button>
      </header>

      {error && (
        <p className="instructor-page-error">
          {error}
        </p>
      )}

      <section className="instructor-stat-grid">
        <article className="instructor-stat-card instructor-stat-card--lime">
          <div className="instructor-stat-card__dot" />

          <strong>
            {dashboard.studentsToday}
          </strong>

          <span>STUDENTS TODAY</span>

          <p>
            across{" "}
            {dashboard.todaySessions.length}{" "}
            sessions
          </p>
        </article>

        <article className="instructor-stat-card instructor-stat-card--orange">
          <div className="instructor-stat-card__dot" />

          <strong>
            {instructor?.averageRating
              ? `${instructor.averageRating.toFixed(
                  2
                )} ★`
              : "—"}
          </strong>

          <span>AVG RATING</span>

          <p>
            {instructor?.averageRating
              ? "current instructor rating"
              : "rating not available"}
          </p>
        </article>

        <article className="instructor-stat-card instructor-stat-card--blue">
          <div className="instructor-stat-card__dot" />

          <strong>
            {dashboard.weekSessions.length}
          </strong>

          <span>CLASSES THIS WEEK</span>

          <p>
            {
              dashboard
                .remainingWeekSessions.length
            }{" "}
            remaining
          </p>
        </article>

        <article className="instructor-stat-card instructor-stat-card--green">
          <div className="instructor-stat-card__dot" />

          <strong>
            {dashboard.nextSession
              ? getCountdown(
                  dashboard.nextSession.startTime
                )
              : "—"}
          </strong>

          <span>NEXT CLASS IN</span>

          <p>
            {dashboard.nextSession
              ? dashboard.nextSession.className
              : "No upcoming class"}
          </p>
        </article>
      </section>

      <div className="instructor-dashboard__content">
        <section className="instructor-panel instructor-agenda">
          <header className="instructor-panel__header">
            <h2>TODAY&apos;S AGENDA</h2>

            <span>
              {new Intl.DateTimeFormat(
                "en-US",
                {
                  month: "short",
                  day: "numeric",
                  year: "numeric",
                }
              )
                .format(new Date())
                .toUpperCase()}
            </span>
          </header>

          {dashboard.todaySessions.length ===
          0 ? (
            <p className="instructor-panel__empty">
              No sessions scheduled for today.
            </p>
          ) : (
            dashboard.todaySessions.map(
              (session) => {
                const enrolled = Math.max(
                  0,
                  session.totalSpots -
                    session.availableSpots
                );

                const occupancy =
                  session.totalSpots === 0
                    ? 0
                    : Math.round(
                        (enrolled /
                          session.totalSpots) *
                          100
                      );

                return (
                  <article
                    key={session.id}
                    className="agenda-row"
                  >
                    <div className="agenda-row__time">
                      <strong>
                        {formatTime(
                          session.startTime
                        )}
                      </strong>

                      <small>
                        {formatTime(
                          session.endTime
                        )}
                      </small>
                    </div>

                    <div className="agenda-row__content">
                      <div className="agenda-row__top">
                        <h3>
                          {session.className}
                        </h3>

                        <span>
                          {session.location ??
                            session.placeName ??
                            "GymBoo"}
                        </span>
                      </div>

                      <div className="agenda-row__occupancy">
                        <span>
                          {enrolled}/
                          {session.totalSpots}{" "}
                          enrolled
                        </span>

                        <div>
                          <i
                            style={{
                              width: `${Math.min(
                                occupancy,
                                100
                              )}%`,
                            }}
                          />
                        </div>

                        <strong>
                          {occupancy}%
                        </strong>
                      </div>

                      <button
                        type="button"
                        onClick={() =>
                          navigate(
                            `/coach/attendance?sessionId=${session.id}`
                          )
                        }
                      >
                        ✓ Open Attendance Sheet
                      </button>
                    </div>
                  </article>
                );
              }
            )
          )}
        </section>

        <section className="instructor-panel instructor-reviews">
          <header className="instructor-panel__header">
            <h2>RECENT REVIEWS</h2>
          </header>

          <div className="instructor-reviews__placeholder">
            <strong>Reviews unavailable</strong>

            <p>
              The current API does not provide an
              endpoint to retrieve instructor
              reviews.
            </p>
          </div>
        </section>
      </div>

      <section className="instructor-panel instructor-upcoming">
        <header className="instructor-panel__header">
          <h2>UPCOMING CLASSES</h2>
        </header>

        {dashboard.upcomingSessions.length ===
        0 ? (
          <p className="instructor-panel__empty">
            No upcoming sessions.
          </p>
        ) : (
          <div className="instructor-upcoming__grid">
            {dashboard.upcomingSessions.map(
              (session) => (
                <article key={session.id}>
                  <span>
                    {new Intl.DateTimeFormat(
                      "en-US",
                      {
                        weekday: "short",
                        month: "short",
                        day: "numeric",
                      }
                    ).format(
                      new Date(
                        session.startTime
                      )
                    )}
                  </span>

                  <h3>{session.className}</h3>

                  <p>
                    {formatTime(
                      session.startTime
                    )}{" "}
                    ·{" "}
                    {session.location ??
                      session.placeName ??
                      "GymBoo"}
                  </p>
                </article>
              )
            )}
          </div>
        )}
      </section>
    </div>
  );
};

export default InstructorDashboard;