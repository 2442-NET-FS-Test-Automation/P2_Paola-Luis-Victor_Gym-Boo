export const getMonday = (date: Date): Date => {
  const result = new Date(date);
  const day = result.getDay();
  const difference = day === 0 ? -6 : 1 - day;

  result.setDate(result.getDate() + difference);
  result.setHours(0, 0, 0, 0);

  return result;
};

export const addDays = (
  date: Date,
  days: number
): Date => {
  const result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
};

export const formatFullDate = (
  date: Date
): string => {
  return new Intl.DateTimeFormat("en-US", {
    weekday: "long",
    month: "long",
    day: "numeric",
    year: "numeric",
  }).format(date);
};

export const formatTime = (
  dateValue: string
): string => {
  return new Intl.DateTimeFormat("en-US", {
    hour: "numeric",
    minute: "2-digit",
  }).format(new Date(dateValue));
};

export const formatWeekRange = (
  weekStart: Date
): string => {
  const weekEnd = addDays(weekStart, 6);

  const startText = new Intl.DateTimeFormat(
    "en-US",
    {
      month: "long",
      day: "numeric",
    }
  ).format(weekStart);

  const endText = new Intl.DateTimeFormat(
    "en-US",
    {
      month:
        weekStart.getMonth() ===
        weekEnd.getMonth()
          ? undefined
          : "long",
      day: "numeric",
      year: "numeric",
    }
  ).format(weekEnd);

  return `${startText} – ${endText}`;
};

export const getCountdown = (
  startTime: string
): string => {
  const difference =
    new Date(startTime).getTime() - Date.now();

  if (difference <= 0) {
    return "Now";
  }

  const totalMinutes = Math.floor(
    difference / 60000
  );

  const hours = Math.floor(
    totalMinutes / 60
  );

  const minutes = totalMinutes % 60;

  if (hours === 0) {
    return `${minutes}m`;
  }

  return `${hours}h ${minutes}m`;
};

export const isSameLocalDay = (isoDate: string, reference: Date): boolean => {
  const d = new Date(isoDate);
  return (
    d.getFullYear() === reference.getFullYear() &&
    d.getMonth() === reference.getMonth() &&
    d.getDate() === reference.getDate()
  );
};