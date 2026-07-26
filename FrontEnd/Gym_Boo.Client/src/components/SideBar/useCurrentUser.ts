import { getStoredUser } from "../../api/auth";
import type { CurrentUser } from "../../types";

export const useCurrentUser = (): CurrentUser => {
  const user = getStoredUser();

  if (!user) {
    return {
      id: 0,
      name: "",
      lastName: "",
      email: "",
      role: "Member",
      initials: "",
    };
  }

  const firstInitial =
    user.name.trim().charAt(0);

  const lastInitial =
    user.lastName.trim().charAt(0);

  return {
    ...user,
    initials: `${firstInitial}${lastInitial}`.toUpperCase(),
  };
};
