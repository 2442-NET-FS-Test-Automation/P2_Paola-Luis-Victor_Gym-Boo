import type { ReactNode } from "react";
import "./Modal.css";

interface ModalProps {
  isOpen: boolean;
  title: string;
  children: ReactNode;
  onClose: () => void;
}

const Modal = ({
  isOpen,
  title,
  children,
  onClose,
}: ModalProps) => {
  if (!isOpen) {
    return null;
  }

  return (
    <div
      className="modal-backdrop"
      role="presentation"
      onMouseDown={onClose}
    >
      <section
        className="modal-card"
        role="dialog"
        aria-modal="true"
        aria-labelledby="modal-title"
        onMouseDown={(event) =>
          event.stopPropagation()
        }
      >
        <h2 id="modal-title">{title}</h2>

        {children}
      </section>
    </div>
  );
};

export default Modal;