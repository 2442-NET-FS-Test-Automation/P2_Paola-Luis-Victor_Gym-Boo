import {
  useEffect,
  useState,
} from "react";

import { useSearchParams } from "react-router-dom";

import {
  getAttendance,
  type AttendanceRecord,
} from "../../../api/instructor";

const InstructorAttendance = () => {
  const [searchParams] = useSearchParams();

  const initialSessionId =
    searchParams.get("sessionId") ?? "";

  const [sessionId, setSessionId] =
    useState(initialSessionId);

  const [records, setRecords] = useState<
    AttendanceRecord[]
  >([]);

  const [loading, setLoading] =
    useState(false);

  const [error, setError] = useState<
    string | null
  >(null);

  const loadAttendance = async () => {
    const numericId = Number(sessionId);

    if (!Number.isInteger(numericId) || numericId <= 0) {
      setError(
        "Enter a valid session ID."
      );

      return;
    }

    setLoading(true);
    setError(null);

    try {
      const data = await getAttendance(
        numericId
      );

      setRecords(data);
    } catch {
      setError(
        "Attendance could not be loaded."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (initialSessionId) {
      void loadAttendance();
    }
  }, []);

  return (
    <div className="admin-page">
      <header className="admin-page__header">
        <p>INSTRUCTOR PORTAL</p>
        <h1>ATTENDANCE SHEET</h1>
        <span>
          Attendance is read-only with the
          current API.
        </span>
      </header>

      <div className="admin-create-form">
        <input
          type="number"
          min="1"
          value={sessionId}
          placeholder="Session ID"
          onChange={(event) =>
            setSessionId(event.target.value)
          }
        />

        <button
          type="button"
          onClick={() =>
            void loadAttendance()
          }
        >
          Load Attendance
        </button>
      </div>

      {error && (
        <p className="admin-page__error">
          {error}
        </p>
      )}

      {loading ? (
        <p>Loading attendance...</p>
      ) : (
        <section className="admin-table">
          <div
            className="admin-table__header"
            style={{
              gridTemplateColumns:
                "80px 1fr 1fr 140px",
            }}
          >
            <span>#</span>
            <span>Member</span>
            <span>Email</span>
            <span>Status</span>
          </div>

          {records.map((record, index) => {
            const present =
              record.isPresent ??
              record.attended ??
              false;

            return (
              <div
                className="admin-table__row"
                key={
                  record.enrollmentId ??
                  record.id ??
                  index
                }
                style={{
                  gridTemplateColumns:
                    "80px 1fr 1fr 140px",
                }}
              >
                <span>{index + 1}</span>

                <strong>
                  {record.memberName ??
                    record.name ??
                    "Member"}
                </strong>

                <span>
                  {record.memberEmail ??
                    record.email ??
                    "No email"}
                </span>

                <span>
                  {present
                    ? "Present"
                    : "Not marked"}
                </span>
              </div>
            );
          })}
        </section>
      )}
    </div>
  );
};

export default InstructorAttendance;