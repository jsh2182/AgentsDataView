import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { logout } from "../store/user/userSlice";

export default function useTokenWatcher() {
  const dispatch = useDispatch();
  const currentUser = useSelector((state) => state.user.currentUser);

  useEffect(() => {
    //const token = localStorage.getItem("token");
    if (!(currentUser?.token?.length > 10)) return;

    try {
      const { exp } = currentUser;
      const now = Date.now();
      const timeLeft = Number(exp) - now;

      if (timeLeft <= 0) {
        dispatch(logout());
      } else {
        const timeout = setTimeout(() => {
          dispatch(logout());
        }, timeLeft);

        return () => clearTimeout(timeout);
      }
    } catch {
      dispatch(logout());
    }
  }, [dispatch, currentUser]);
}
