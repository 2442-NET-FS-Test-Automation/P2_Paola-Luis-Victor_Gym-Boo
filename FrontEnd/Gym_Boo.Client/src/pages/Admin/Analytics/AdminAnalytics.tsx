import { useCallback, useEffect, useMemo, useState } from "react";
import axios from "axios";
import {
  CircleDollarSign,
  RefreshCw,
  Star,
  TicketX,
  TrendingUp,
  Users,
} from "lucide-react";

import {
  getBestRatedReport,
  getRevenueReport,
  getSessionReports,
  type BestRatedReport,
  type RevenueReport,
  type SessionReport,
} from "../../../api/admin";

import "./AdminAnalytics.css";

const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    maximumFractionDigits: 2,
  }).format(value);
};

const AdminAnalytics = () => {
  const [sessionReports, setSessionReports] = useState<SessionReport[]>([]);
  const [revenueReport, setRevenueReport] = useState<RevenueReport | null>(
    null
  );
  const [bestRatedReports, setBestRatedReports] = useState<
    BestRatedReport[]
  >([]);

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadAnalytics = useCallback(async (refresh = false) => {
    if (refresh) {
      setRefreshing(true);
    } else {
      setLoading(true);
    }

    setError(null);

    /*
     * Each request is loaded separately so that a 404 or 204 response
     * from one report does not prevent the other reports from loading.
     */
    const [sessionsResult, revenueResult, bestRatedResult] =
      await Promise.allSettled([
        getSessionReports(),
        getRevenueReport(),
        getBestRatedReport(),
      ]);

    let failedRequest = false;

    if (sessionsResult.status === "fulfilled") {
      setSessionReports(sessionsResult.value ?? []);
    } else if (
      axios.isAxiosError(sessionsResult.reason) &&
      sessionsResult.reason.response?.status === 404
    ) {
      setSessionReports([]);
    } else {
      failedRequest = true;
      setSessionReports([]);
    }

    if (revenueResult.status === "fulfilled") {
      setRevenueReport(revenueResult.value);
    } else {
      failedRequest = true;
      setRevenueReport(null);
    }

    if (bestRatedResult.status === "fulfilled") {
      setBestRatedReports(bestRatedResult.value ?? []);
    } else if (
      axios.isAxiosError(bestRatedResult.reason) &&
      bestRatedResult.reason.response?.status === 204
    ) {
      setBestRatedReports([]);
    } else {
      failedRequest = true;
      setBestRatedReports([]);
    }

    if (failedRequest) {
      setError(
        "Some reports could not be loaded. The available data is still displayed."
      );
    }

    setLoading(false);
    setRefreshing(false);
  }, []);

  useEffect(() => {
    void loadAnalytics();
  }, [loadAnalytics]);

  const cancellationRevenue =
    revenueReport?.cancellationRevenue ?? 0;

  const subscriptionRevenue =
    revenueReport?.subscriptionRevenue ?? 0;

  const totalRevenue =
    revenueReport?.totalRevenue ?? 0;

  const totalEnrollments = useMemo(() => {
    return sessionReports.reduce(
      (total, report) => total + report.totalEnrollments,
      0
    );
  }, [sessionReports]);

  const averageEnrollments = useMemo(() => {
    if (sessionReports.length === 0) {
      return 0;
    }

    return Math.round(totalEnrollments / sessionReports.length);
  }, [sessionReports, totalEnrollments]);

  const highestEnrollmentValue = useMemo(() => {
    if (sessionReports.length === 0) {
      return 1;
    }

    return Math.max(
      ...sessionReports.map((report) => report.totalEnrollments),
      1
    );
  }, [sessionReports]);

  const subscriptionPercentage = useMemo(() => {
    if (totalRevenue <= 0) {
      return 0;
    }

    return Math.round((subscriptionRevenue / totalRevenue) * 100);
  }, [subscriptionRevenue, totalRevenue]);

  const cancellationPercentage = useMemo(() => {
    if (totalRevenue <= 0) {
      return 0;
    }

    return Math.round((cancellationRevenue / totalRevenue) * 100);
  }, [cancellationRevenue, totalRevenue]);

  const sortedClasses = useMemo(() => {
    return [...bestRatedReports].sort(
      (first, second) =>
        second.averageRating - first.averageRating
    );
  }, [bestRatedReports]);

  if (loading) {
    return (
      <main className="analytics-page">
        <div className="analytics-loading">
          <div className="analytics-spinner" />
          <p>Loading analytics...</p>
        </div>
      </main>
    );
  }

  return (
    <main className="analytics-page">
      <header className="analytics-header">
        <div>
          <p className="analytics-eyebrow">Admin panel</p>

          <h1>Analytics &amp; Reports</h1>

          <p className="analytics-subtitle">
            Revenue, enrollments and class ratings.
          </p>
        </div>

        <button
          type="button"
          className="analytics-refresh-button"
          disabled={refreshing}
          onClick={() => void loadAnalytics(true)}
        >
          <RefreshCw
            size={17}
            className={refreshing ? "is-spinning" : ""}
          />

          {refreshing ? "Refreshing..." : "Refresh"}
        </button>
      </header>

      {error && (
        <div className="analytics-alert" role="alert">
          <span>{error}</span>

          <button
            type="button"
            onClick={() => void loadAnalytics(true)}
          >
            Try again
          </button>
        </div>
      )}

      <section className="analytics-summary-grid">
        <article className="analytics-stat-card analytics-stat-card--green">
          <div className="analytics-stat-card__top">
            <span className="analytics-stat-card__label">
              Total revenue
            </span>

            <CircleDollarSign size={21} />
          </div>

          <strong>{formatCurrency(totalRevenue)}</strong>

          <p>
            <TrendingUp size={14} />
            Combined platform revenue
          </p>
        </article>

        <article className="analytics-stat-card analytics-stat-card--blue">
          <div className="analytics-stat-card__top">
            <span className="analytics-stat-card__label">
              Subscription revenue
            </span>

            <Users size={21} />
          </div>

          <strong>
            {formatCurrency(subscriptionRevenue)}
          </strong>

          <p>{subscriptionPercentage}% of total revenue</p>
        </article>

        <article className="analytics-stat-card analytics-stat-card--orange">
          <div className="analytics-stat-card__top">
            <span className="analytics-stat-card__label">
              Cancellation revenue
            </span>

            <TicketX size={21} />
          </div>

          <strong>
            {formatCurrency(cancellationRevenue)}
          </strong>

          <p>{cancellationPercentage}% of total revenue</p>
        </article>

        <article className="analytics-stat-card analytics-stat-card--purple">
          <div className="analytics-stat-card__top">
            <span className="analytics-stat-card__label">
              Total enrollments
            </span>

            <Users size={21} />
          </div>

          <strong>
            {totalEnrollments.toLocaleString("en-US")}
          </strong>

          <p>{averageEnrollments} average per discipline</p>
        </article>
      </section>

      <section className="analytics-main-grid">
        <article className="analytics-panel">
          <div className="analytics-panel__header">
            <div>
              <h2>Enrollments by discipline</h2>

              <p>
                Total registrations grouped by discipline
              </p>
            </div>

            <Users size={20} />
          </div>

          {sessionReports.length === 0 ? (
            <div className="analytics-empty">
              <Users size={30} />

              <p>No enrollment reports are available.</p>
            </div>
          ) : (
            <div className="analytics-bars">
              {sessionReports.map((report) => {
                const barPercentage =
                  (report.totalEnrollments /
                    highestEnrollmentValue) *
                  100;

                return (
                  <div
                    className="analytics-bar-row"
                    key={report.disciplineName}
                  >
                    <div className="analytics-bar-row__details">
                      <div>
                        <strong>
                          {report.disciplineName}
                        </strong>

                        <span>Discipline</span>
                      </div>

                      <span>
                        {report.totalEnrollments}
                      </span>
                    </div>

                    <div className="analytics-bar-track">
                      <div
                        className="analytics-bar-fill"
                        style={{
                          width: `${Math.max(
                            barPercentage,
                            3
                          )}%`,
                        }}
                      />
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </article>

        <article className="analytics-panel">
          <div className="analytics-panel__header">
            <div>
              <h2>Revenue breakdown</h2>

              <p>Revenue distribution by source</p>
            </div>

            <CircleDollarSign size={20} />
          </div>

          <div className="analytics-revenue-total">
            <span>Total revenue</span>

            <strong>
              {formatCurrency(totalRevenue)}
            </strong>
          </div>

          <div className="analytics-breakdown-list">
            <div className="analytics-breakdown-item">
              <div className="analytics-breakdown-item__header">
                <div>
                  <span className="analytics-dot analytics-dot--subscription" />
                  <span>Subscriptions</span>
                </div>

                <strong>
                  {subscriptionPercentage}%
                </strong>
              </div>

              <div className="analytics-progress-track">
                <div
                  className="analytics-progress-fill analytics-progress-fill--subscription"
                  style={{
                    width: `${subscriptionPercentage}%`,
                  }}
                />
              </div>

              <small>
                {formatCurrency(subscriptionRevenue)}
              </small>
            </div>

            <div className="analytics-breakdown-item">
              <div className="analytics-breakdown-item__header">
                <div>
                  <span className="analytics-dot analytics-dot--cancellation" />
                  <span>Cancellation fees</span>
                </div>

                <strong>
                  {cancellationPercentage}%
                </strong>
              </div>

              <div className="analytics-progress-track">
                <div
                  className="analytics-progress-fill analytics-progress-fill--cancellation"
                  style={{
                    width: `${cancellationPercentage}%`,
                  }}
                />
              </div>

              <small>
                {formatCurrency(cancellationRevenue)}
              </small>
            </div>
          </div>
        </article>
      </section>

      <section className="analytics-secondary-grid">
        <article className="analytics-panel">
          <div className="analytics-panel__header">
            <div>
              <h2>Best-rated classes</h2>

              <p>Classes ordered by average rating</p>
            </div>

            <Star size={20} />
          </div>

          {sortedClasses.length === 0 ? (
            <div className="analytics-empty">
              <Star size={30} />

              <p>No rating information is available.</p>
            </div>
          ) : (
            <div className="analytics-ranking">
              {sortedClasses.map((classReport, index) => {
                const ratingPercentage = Math.min(
                  Math.max(
                    (classReport.averageRating / 5) * 100,
                    0
                  ),
                  100
                );

                return (
                  <div
                    className="analytics-ranking-item"
                    key={classReport.id}
                  >
                    <span className="analytics-ranking-item__position">
                      {index + 1}
                    </span>

                    <div className="analytics-ranking-item__name">
                      <strong>
                        {classReport.className}
                      </strong>

                      <span className="analytics-instructor-name">
                        {classReport.instructorName}
                      </span>

                      <div className="analytics-rating-stars">
                        <Star
                          size={14}
                          fill="currentColor"
                        />

                        <span>
                          {classReport.averageRating.toFixed(
                            1
                          )}
                        </span>
                      </div>
                    </div>

                    <div className="analytics-rating-bar">
                      <div
                        style={{
                          width: `${ratingPercentage}%`,
                        }}
                      />
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </article>

        <article className="analytics-panel analytics-panel--table">
          <div className="analytics-panel__header">
            <div>
              <h2>Discipline report</h2>

              <p>Detailed enrollment totals</p>
            </div>

            <Users size={20} />
          </div>

          {sessionReports.length === 0 ? (
            <div className="analytics-empty">
              <Users size={30} />

              <p>No enrollment information was found.</p>
            </div>
          ) : (
            <div className="analytics-table-wrapper">
              <table className="analytics-table">
                <thead>
                  <tr>
                    <th>Discipline</th>
                    <th>Total enrollments</th>
                  </tr>
                </thead>

                <tbody>
                  {sessionReports.map((report) => (
                    <tr key={report.disciplineName}>
                      <td>
                        <div className="analytics-class-cell">
                          <span>
                            {report.disciplineName
                              .charAt(0)
                              .toUpperCase()}
                          </span>

                          <strong>
                            {report.disciplineName}
                          </strong>
                        </div>
                      </td>

                      <td>
                        <span className="analytics-registration-badge">
                          {report.totalEnrollments}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </article>
      </section>
    </main>
  );
};

export default AdminAnalytics;
