import {
  useEffect,
  useState,
} from "react";

import { getClasses } from "../../../api/sessions";
import type { ApiClassSession } from "../../../types";

const AdminSessions = () => {
  const [sessions, setSessions] = useState<
    ApiClassSession[]
  >([]);

  const [loading, setLoading] =
    useState(true);

  useEffect(() => {
    getClasses({ past: true })
      .then((data) => {
        const sorted = [...data].sort(
          (a, b) =>
            new Date(a.startTime).getTime() -
            new Date(b.startTime).getTime()
        );

        setSessions(sorted);
      })
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="admin-page">
      <header className="admin-page__header">
        <p>ADMIN PORTAL</p>
        <h1>SESSIONS</h1>
        <span>
          Review all scheduled GymBoo
          sessions.
        </span>
      </header>

      {loading ? (
        <p>Loading sessions...</p>
      ) : (
        <section className="admin-table">
          <div
            className="admin-table__header"
            style={{
              gridTemplateColumns:
                "190px 1fr 1fr 1fr 120px",
            }}
          >
            <span>Date</span>
            <span>Class</span>
            <span>Instructor</span>
            <span>Location</span>
            <span>Enrolled</span>
          </div>

          {sessions.map((session) => (
            <div
              className="admin-table__row"
              key={session.id}
              style={{
                gridTemplateColumns:
                  "190px 1fr 1fr 1fr 120px",
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

              <span>
                {session.instructorName}
              </span>

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

export default AdminSessions;