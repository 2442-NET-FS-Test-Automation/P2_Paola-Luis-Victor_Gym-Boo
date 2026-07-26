import {
  useEffect,
  useMemo,
  useState,
} from "react";

import { useNavigate } from "react-router-dom";

import { getClasses } from "../../../api/sessions";
import { useCurrentUser } from "../../../components/SideBar/useCurrentUser";

import type { ApiClassSession } from "../../../types";

import "./InstructorDashboard.css";

const isSameLocalDay = (
  first: Date,
  second: Date
): boolean => {
  return (
    first.getFullYear() === second.getFullYear() &&
    first.getMonth() === second.getMonth() &&
    first.getDate() === second.getDate()
  );
};

const startOfWeek = (date: Date): Date => {
  const result = new Date(date);
  const day = result.getDay();
  const difference = day === 0 ? -6 : 1 - day;

  result.setDate(result.getDate() + difference);
  result.setHours(0, 0, 0, 0);

  return result;
};

const endOfWeek = (date: Date): Date => {
  const result = startOfWeek(date);
  result.setDate(result.getDate() + 7);
  return result;
};

const formatCountdown = (
  targetDate: Date
): string => {
  const milliseconds =
    targetDate.getTime() - Date.now();

  if (milliseconds <= 0) {
    return "Now";
  }

  const totalMinutes = Math.floor(
    milliseconds / 60000
  );

  const hours = Math.floor(
    totalMinutes / 60
  );

  const minutes = totalMinutes % 60;

  return `${hours}h ${minutes}m`;
};

const InstructorDashboard = () => {
  const user = useCurrentUser();
  const navigate = useNavigate();

  const [sessions, setSessions] = useState<
    ApiClassSession[]
  >([]);

  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getClasses({ past: true })
      .then((data) => {
        const instructorSessions = data.filter(
          (session) =>
            session.instructorName
              .toLowerCase()
              .includes(
                user.name.toLowerCase()
              )
        );

        setSessions(instructorSessions);
      })
      .finally(() => setLoading(false));
  }, [user.name]);

  const dashboard = useMemo(() => {
    const now = new Date();
    const weekStart = startOfWeek(now);
    const weekEnd = endOfWeek(now);

    const sorted = [...sessions].sort(
      (a, b) =>
        new Date(a.startTime).getTime() -
        new Date(b.startTime).getTime()
    );

    const todaySessions = sorted.filter(
      (session) =>
        isSameLocalDay(
          new Date(session.startTime),
          now
        )
    );

    const weekSessions = sorted.filter(
      (session) => {
        const start = new Date(
          session.startTime
        );

        return (
          start >= weekStart &&
          start < weekEnd
        );
      }
    );

    const remainingSessions =
      weekSessions.filter(
        (session) =>
          new Date(session.endTime) > now
      );

    const upcomingSessions = sorted.filter(
      (session) =>
        new Date(session.startTime) > now
    );

    const nextSession =
      upcomingSessions[0] ?? null;

    const totalStudentsToday =
      todaySessions.reduce(
        (total, session) =>
          total +
          (session.totalSpots -
            session.availableSpots),
        0
      );

    const ratings = sessions
      .map((session) =>
        Number(session.instructorRating)
      )
      .filter((rating) => rating > 0);

    const averageRating =
      ratings.length > 0
        ? ratings.reduce(
            (total, rating) =>
              total + rating,
            0
          ) / ratings.length
        : 0;

    return {
      todaySessions,
      weekSessions,
      remainingSessions,
      upcomingSessions:
        upcomingSessions.slice(0, 3),
      nextSession,
      totalStudentsToday,
      averageRating,
    };
  }, [sessions]);

  const todayText =
    new Intl.DateTimeFormat("en-US", {
      weekday: "long",
      month: "long",
      day: "numeric",
      year: "numeric",
    }).format(new Date());

  if (loading) {
    return <p>Loading dashboard...</p>;
  }

  return (
    <div className="instructor-dashboard">
      <header className="instructor-dashboard__header">
        <div>
          <p className="instructor-dashboard__eyebrow">
            INSTRUCTOR PORTAL
          </p>

          <h1>
            GOOD MORNING,{" "}
            {user.name.toUpperCase()}
          </h1>

          <p>
            {todayText} · You have{" "}
            {dashboard.todaySessions.length}{" "}
            classes today
          </p>
        </div>

        <button
          type="button"
          onClick={() =>
            navigate("/coach/schedule")
          }
        >
          View Full Schedule →
        </button>
      </header>

      <section className="dashboard-stat-grid">
        <article className="dashboard-stat-card">
          <strong>
            {dashboard.totalStudentsToday}
          </strong>

          <span>STUDENTS TODAY</span>

          <p>
            across{" "}
            {dashboard.todaySessions.length}{" "}
            sessions
          </p>
        </article>

        <article className="dashboard-stat-card">
          <strong>
            {dashboard.averageRating.toFixed(2)} ★
          </strong>

          <span>AVG RATING</span>
          <p>based on available sessions</p>
        </article>

        <article className="dashboard-stat-card">
          <strong>
            {dashboard.weekSessions.length}
          </strong>

          <span>CLASSES THIS WEEK</span>

          <p>
            {
              dashboard.remainingSessions
                .length
            }{" "}
            remaining
          </p>
        </article>

        <article className="dashboard-stat-card">
          <strong>
            {dashboard.nextSession
              ? formatCountdown(
                  new Date(
                    dashboard.nextSession
                      .startTime
                  )
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

      <section className="dashboard-panel">
        <header>
          <h2>TODAY&apos;S AGENDA</h2>
          <span>{todayText}</span>
        </header>

        {dashboard.todaySessions.length ===
        0 ? (
          <p className="dashboard-empty">
            No sessions scheduled today.
          </p>
        ) : (
          dashboard.todaySessions.map(
            (session) => {
              const enrolled =
                session.totalSpots -
                session.availableSpots;

              return (
                <article
                  key={session.id}
                  className="agenda-session"
                >
                  <time>
                    {new Intl.DateTimeFormat(
                      "en-US",
                      {
                        hour: "numeric",
                        minute: "2-digit",
                      }
                    ).format(
                      new Date(
                        session.startTime
                      )
                    )}
                  </time>

                  <div>
                    <h3>
                      {session.className}
                    </h3>

                    <p>
                      {enrolled}/
                      {session.totalSpots}{" "}
                      enrolled ·{" "}
                      {session.location}
                    </p>
                  </div>

                  <button
                    type="button"
                    onClick={() =>
                      navigate(
                        `/coach/attendance?sessionId=${session.id}`
                      )
                    }
                  >
                    Open Attendance
                  </button>
                </article>
              );
            }
          )
        )}
      </section>

      <section className="dashboard-panel">
        <header>
          <h2>UPCOMING CLASSES</h2>
        </header>

        {dashboard.upcomingSessions.map(
          (session) => (
            <article
              key={session.id}
              className="agenda-session"
            >
              <time>
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
              </time>

              <div>
                <h3>{session.className}</h3>
                <p>{session.location}</p>
              </div>
            </article>
          )
        )}
      </section>
    </div>
  );
};

export default InstructorDashboard;