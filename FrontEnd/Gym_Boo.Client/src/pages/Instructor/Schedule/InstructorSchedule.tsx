import {
  useEffect,
  useState,
} from "react";

import { getClasses } from "../../../api/sessions";
import { useCurrentUser } from "../../../components/SideBar/useCurrentUser";

import type { ApiClassSession } from "../../../types";

const InstructorSchedule = () => {
  const user = useCurrentUser();

  const [sessions, setSessions] = useState<
    ApiClassSession[]
  >([]);

  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getClasses({ past: true })
      .then((data) => {
        const ownSessions = data
          .filter((session) =>
            session.instructorName
              .toLowerCase()
              .includes(
                user.name.toLowerCase()
              )
          )
          .sort(
            (a, b) =>
              new Date(
                a.startTime
              ).getTime() -
              new Date(
                b.startTime
              ).getTime()
          );

        setSessions(ownSessions);
      })
      .finally(() => setLoading(false));
  }, [user.name]);

  return (
    <div className="admin-page">
      <header className="admin-page__header">
        <p>INSTRUCTOR PORTAL</p>
        <h1>SCHEDULE &amp; AVAILABILITY</h1>
        <span>
          Your assigned GymBoo sessions.
        </span>
      </header>

      {loading ? (
        <p>Loading schedule...</p>
      ) : (
        <section className="admin-table">
          <div
            className="admin-table__header"
            style={{
              gridTemplateColumns:
                "180px 1fr 1fr 140px",
            }}
          >
            <span>Date</span>
            <span>Class</span>
            <span>Location</span>
            <span>Enrolled</span>
          </div>

          {sessions.map((session) => (
            <div
              className="admin-table__row"
              key={session.id}
              style={{
                gridTemplateColumns:
                  "180px 1fr 1fr 140px",
              }}
            >
              <span>
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
              </span>

              <strong>
                {session.className}
              </strong>

              <span>{session.location}</span>

              <span>
                {session.totalSpots -
                  session.availableSpots}
                /{session.totalSpots}
              </span>
            </div>
          ))}
        </section>
      )}
    </div>
  );
};

export default InstructorSchedule;