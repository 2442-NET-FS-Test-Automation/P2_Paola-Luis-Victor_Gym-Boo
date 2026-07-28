import {
  useEffect,
  useState,
  type FormEvent,
} from "react";

import axios from "axios";

import {
  createInstructorSession,
  getClassOptions,
  getPlaceOptions,
  type ClassOption,
  type CreateSessionRequest,
  type PlaceOption,
} from "../../../api/instructor";

import Modal from "../../../components/Modal/Modal";

interface AddSessionModalProps {
  isOpen: boolean;
  instructorId: number;
  onClose: () => void;
  onCreated: () => Promise<void> | void;
}

const getSessionError = (
  error: unknown
): string => {
  if (!axios.isAxiosError(error)) {
    return "The session could not be created.";
  }

  const data = error.response?.data;

  if (typeof data === "string") {
    return data;
  }

  return (
    data?.message ??
    "The session could not be created."
  );
};

const AddSessionModal = ({
  isOpen,
  instructorId,
  onClose,
  onCreated,
}: AddSessionModalProps) => {
  const [classId, setClassId] = useState("");
  const [placeId, setPlaceId] = useState("");
  const [classOptions, setClassOptions] =
    useState<ClassOption[]>([]);
  const [placeOptions, setPlaceOptions] =
    useState<PlaceOption[]>([]);

  const [startTime, setStartTime] =
    useState("");

  const [endTime, setEndTime] =
    useState("");

  const [slots, setSlots] = useState("20");

  const [
    cancellationFee,
    setCancellationFee,
  ] = useState("0");

  const [submitting, setSubmitting] =
    useState(false);

  const [error, setError] = useState<
    string | null
  >(null);

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    let cancelled = false;

    const loadOptions = async () => {
      setError(null);

      try {
        const [classes, places] =
          await Promise.all([
            getClassOptions(),
            getPlaceOptions(),
          ]);

        if (!cancelled) {
          setClassOptions(classes);
          setPlaceOptions(places);
        }
      } catch {
        if (!cancelled) {
          setError(
            "Class and room options could not be loaded."
          );
        }
      }
    };

    void loadOptions();

    return () => {
      cancelled = true;
    };
  }, [isOpen]);

  const clearForm = () => {
    setClassId("");
    setPlaceId("");
    setStartTime("");
    setEndTime("");
    setSlots("20");
    setCancellationFee("0");
    setError(null);
  };

  const handleClose = () => {
    if (submitting) {
      return;
    }

    clearForm();
    onClose();
  };

  const handleSubmit = async (
    event: FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();
    setError(null);

    const numericClassId = Number(classId);
    const numericPlaceId = Number(placeId);
    const numericSlots = Number(slots);
    const numericFee = Number(cancellationFee);

    if (
      numericClassId <= 0 ||
      numericPlaceId <= 0
    ) {
      setError(
        "Class ID and place ID must be valid."
      );

      return;
    }

    if (numericSlots <= 0) {
      setError(
        "The number of slots must be greater than zero."
      );

      return;
    }

    if (!startTime || !endTime) {
      setError(
        "Start and end time are required."
      );

      return;
    }

    if (
      new Date(endTime) <=
      new Date(startTime)
    ) {
      setError(
        "Session end time must be after the start time."
      );

      return;
    }

    const request: CreateSessionRequest = {
      startTime: new Date(
        startTime
      ).toISOString(),
      endTime: new Date(
        endTime
      ).toISOString(),

      slots: numericSlots,
      cancellationFee: numericFee,
      classId: numericClassId,
      instructorId,
      placeId: numericPlaceId,
    };

    setSubmitting(true);

    try {
      await createInstructorSession(request);

      clearForm();
      onClose();
      await onCreated();
    } catch (error: unknown) {
      setError(getSessionError(error));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      title="Add new session"
      onClose={handleClose}
    >
      <form
        className="admin-modal-form"
        onSubmit={handleSubmit}
      >
        <label htmlFor="session-class-id">
          Class
        </label>

        <select
          id="session-class-id"
          value={classId}
          onChange={(event) =>
            setClassId(event.target.value)
          }
          required
        >
          <option value="">
            Select a class
          </option>

          {classOptions.map((gymClass) => (
            <option
              key={gymClass.id}
              value={gymClass.id}
            >
              {gymClass.name}
            </option>
          ))}
        </select>

        <label htmlFor="session-place-id">
          Room
        </label>

        <select
          id="session-place-id"
          value={placeId}
          onChange={(event) =>
            setPlaceId(event.target.value)
          }
          required
        >
          <option value="">
            Select a room
          </option>

          {placeOptions.map((place) => (
            <option
              key={place.id}
              value={place.id}
            >
              {place.name}
            </option>
          ))}
        </select>

        <label htmlFor="session-start">
          Start time
        </label>

        <input
          id="session-start"
          type="datetime-local"
          value={startTime}
          onChange={(event) =>
            setStartTime(event.target.value)
          }
          required
        />

        <label htmlFor="session-end">
          End time
        </label>

        <input
          id="session-end"
          type="datetime-local"
          value={endTime}
          onChange={(event) =>
            setEndTime(event.target.value)
          }
          required
        />

        <label htmlFor="session-slots">
          Available slots
        </label>

        <input
          id="session-slots"
          type="number"
          min="1"
          value={slots}
          onChange={(event) =>
            setSlots(event.target.value)
          }
          required
        />

        <label htmlFor="session-fee">
          Cancellation fee
        </label>

        <input
          id="session-fee"
          type="number"
          min="0"
          step="0.01"
          value={cancellationFee}
          onChange={(event) =>
            setCancellationFee(
              event.target.value
            )
          }
          required
        />

        {error && (
          <p className="admin-modal-error">
            {error}
          </p>
        )}

        <div className="admin-modal-actions">
          <button
            type="button"
            className="admin-modal-cancel"
            onClick={handleClose}
            disabled={submitting}
          >
            Cancel
          </button>

          <button
            type="submit"
            className="admin-modal-submit"
            disabled={submitting}
          >
            {submitting
              ? "Creating..."
              : "Add session"}
          </button>
        </div>
      </form>
    </Modal>
  );
};



export default AddSessionModal;
