import type { CurrentUser } from "../../types";

// TODO: reemplazar por un hook conectado a auth real (context/store)
// cuando el backend de autenticación esté disponible.
export const useCurrentUser = (): CurrentUser => ({
    id: 4,
    name: "Jordan Martinez",
    role: "MEMBER",
    initials: "JM",
});