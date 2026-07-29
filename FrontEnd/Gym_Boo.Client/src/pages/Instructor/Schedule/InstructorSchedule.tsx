import {
  useEffect,
  useMemo,
  useState,
} from "react";

import { Plus } from "lucide-react";
import AddSessionModal from "./AddSession";

import {
  ChevronLeft,
  ChevronRight,
} from "lucide-react";

import {
  deleteInstructorSession,
  getInstructorSessions,
  type InstructorSession,
} from "../../../api/instructor";

import { getStoredUser } from "../../../api/auth";

import {
  addDays,
  formatWeekRange,
  getMonday,
  isSameLocalDay,
} from "../../../utils/instructorDates";

import "./InstructorSchedule.css";
import Modal from "../../../components/Modal/Modal";

const START_HOUR = 6;
const END_HOUR = 21;
const HOUR_HEIGHT = 72;

const getMinutesFromStart = (
  date: Date
): number => {
  return (
    (date.getHours() - START_HOUR) * 60 +
    date.getMinutes()
  );
};

const InstructorSchedule = () => {
  const user = getStoredUser();

  const [sessions, setSessions] = useState<
    InstructorSession[]
  >([]);

  const [weekStart, setWeekStart] = useState(
    getMonday(new Date())
  );

  const [loading, setLoading] = useState(true);

  const [error, setError] = useState<
    string | null
  >(null);

  const [success, setSuccess] = useState<
    string | null
  >(null);

  const [deletingSession, setDeletingSession] =
    useState(false);

  const [
    selectedSession,
    setSelectedSession,
  ] = useState<InstructorSession | null>(null);

  const loadSessions = async () => {
    if (!user) {
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const result =
        await getInstructorSessions(user.id);

      setSessions(result);
    } catch {
      setError(
        "The instructor schedule could not be loaded."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadSessions();
  }, [user?.id]);

  const weekDays = useMemo(
    () =>
      Array.from(
        { length: 7 },
        (_, index) =>
          addDays(weekStart, index)
      ),
    [weekStart]
  );

  const weekSessions = useMemo(() => {
    const nextWeek = addDays(weekStart, 7);

    return sessions.filter((session) => {
      const start = new Date(
        session.startTime
      );

      return (
        start >= weekStart &&
        start < nextWeek
      );
    });
  }, [sessions, weekStart]);

  const statistics = useMemo(() => {
    const students = weekSessions.reduce(
      (total, session) =>
        total +
        session.enrolledSpots,
      0
    );

    return {
      classes: weekSessions.length,
      students,
    };
  }, [weekSessions]);

  const hours = Array.from(
    {
      length:
        END_HOUR - START_HOUR + 1,
    },
    (_, index) =>
      START_HOUR + index
  );

  const [isAddSessionOpen, setIsAddSessionOpen] =
  useState(false);

  const handleSessionCreated = async () => {
    await loadSessions();
    setSuccess("Session created successfully.");
  };

  const handleDeleteSession = async () => {
    if (!selectedSession) {
      return;
    }

    setDeletingSession(true);
    setError(null);
    setSuccess(null);

    try {
      await deleteInstructorSession(
        selectedSession.id
      );

      setSelectedSession(null);
      await loadSessions();
      setSuccess("Session deleted successfully.");
    } catch {
      setError(
        "The session could not be deleted."
      );
    } finally {
      setDeletingSession(false);
    }
  };

  return (
    <div className="instructor-schedule">
      <header className="instructor-schedule__hero">

        <div className="instructor-schedule__actions">
          <div className="instructor-week-buttons">
            <button
              type="button"
              onClick={() =>
                setWeekStart((current) =>
                  addDays(current, -7)
                )
              }
            >
              <ChevronLeft size={17} />
              Prev Week
            </button>

            <button
              type="button"
              className="is-active"
              onClick={() =>
                setWeekStart(
                  getMonday(new Date())
                )
              }
            >
              This Week
            </button>

            <button
              type="button"
              onClick={() =>
                setWeekStart((current) =>
                  addDays(current, 7)
                )
              }
            >
              Next Week
              <ChevronRight size={17} />
            </button>
          </div>

          <button
            type="button"
            className="instructor-add-session-button"
            onClick={() =>
              setIsAddSessionOpen(true)
            }
          >
            <Plus size={18} />
            Add session
          </button>
        </div>
        <div>
          <p className="instructor-eyebrow">
            INSTRUCTOR PORTAL
          </p>

          <h1>SCHEDULE</h1>

          <p>
            Week of{" "}
            {formatWeekRange(weekStart)}
          </p>
        </div>

        <div className="instructor-week-buttons">
          <button
            type="button"
            onClick={() =>
              setWeekStart((current) =>
                addDays(current, -7)
              )
            }
          >
            <ChevronLeft size={17} />
            Prev Week
          </button>

          <button
            type="button"
            className="is-active"
            onClick={() =>
              setWeekStart(
                getMonday(new Date())
              )
            }
          >
            This Week
          </button>

          <button
            type="button"
            onClick={() =>
              setWeekStart((current) =>
                addDays(current, 7)
              )
            }
          >
            Next Week
            <ChevronRight size={17} />
          </button>
        </div>
      </header>

      <section className="instructor-schedule__stats">
        <article>
          <strong>
            {statistics.classes}
          </strong>
          <span>Classes this week</span>
        </article>

        <article>
          <strong>
            {statistics.students}
          </strong>
          <span>Total students</span>
        </article>
      </section>

      {error && (
        <p className="instructor-page-error">
          {error}
        </p>
      )}

      {success && (
        <p className="instructor-page-success">
          {success}
        </p>
      )}

      {loading ? (
        <p className="instructor-page-state">
          Loading schedule...
        </p>
      ) : (
        <section className="instructor-week-calendar">
          <div className="instructor-week-calendar__header">
            <div />

            {weekDays.map((day) => (
              <div
                key={day.toISOString()}
                className={
                  isSameLocalDay(
                    day,
                    new Date()
                  )
                    ? "is-today"
                    : ""
                }
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

                <strong>{day.getDate()}</strong>
              </div>
            ))}
          </div>

          <div className="instructor-week-calendar__body">
            <div className="instructor-time-column">
              {hours.map((hour) => (
                <div
                  key={hour}
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
                    isSameLocalDay(
                      new Date(
                        session.startTime
                      ),
                      day
                    )
                );

              return (
                <div
                  key={day.toISOString()}
                  className="instructor-day-column"
                  style={{
                    height:
                      hours.length *
                      HOUR_HEIGHT,
                  }}
                >
                  {hours.map((hour) => (
                    <i
                      key={hour}
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
                        new Date(
                          session.startTime
                        );

                      const end =
                        new Date(
                          session.endTime
                        );

                      const minutes =
                        getMinutesFromStart(
                          start
                        );

                      const duration =
                        Math.max(
                          45,
                          (end.getTime() -
                            start.getTime()) /
                            60000
                        );

                      const enrolled =
                        session.enrolledSpots;

                      return (
                        <article
                          key={session.id}
                          role="button"
                          tabIndex={0}
                          className="instructor-calendar-session"
                          onClick={() =>
                            setSelectedSession(
                              session
                            )
                          }
                          onKeyDown={(event) => {
                            if (
                              event.key ===
                                "Enter" ||
                              event.key === " "
                            ) {
                              setSelectedSession(
                                session
                              );
                            }
                          }}
                          style={{
                            top:
                              (minutes / 60) *
                              HOUR_HEIGHT,
                            height: Math.max(
                              54,
                              (duration / 60) *
                                HOUR_HEIGHT -
                                5
                            ),
                          }}
                        >
                          <strong>
                            {
                              session.className
                            }
                          </strong>

                          <span>
                            {enrolled}/
                            {
                              session.totalSpots
                            }{" "}
                            enrolled
                          </span>
                        </article>
                      );
                    }
                  )}
                </div>
              );
            })}
          </div>
          {user && (
            <AddSessionModal
              isOpen={isAddSessionOpen}
              instructorId={user.id}
              onClose={() =>
                setIsAddSessionOpen(false)
              }
              onCreated={handleSessionCreated}
            />
          )}
        </section>
      )}

      <Modal
        isOpen={selectedSession !== null}
        title="Session details"
        onClose={() =>
          setSelectedSession(null)
        }
      >
        {selectedSession && (
          <div className="session-detail-modal">
            <h3>{selectedSession.className}</h3>

            <p>
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
                new Date(
                  selectedSession.startTime
                )
              )}
            </p>

            <p>
              Room:{" "}
              {selectedSession.location ??
                selectedSession.placeName ??
                "GymBoo"}
            </p>

            <div className="admin-modal-actions">
              <button
                type="button"
                className="admin-modal-delete"
                disabled={deletingSession}
                onClick={handleDeleteSession}
              >
                {deletingSession
                  ? "Deleting..."
                  : "Delete session"}
              </button>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default InstructorSchedule;
