import React, { useEffect } from "react";
import { Navigate, Outlet, useLocation, useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";

export default function PrivateRoutes() {
  const currentUser = useSelector((state) => state.user.currentUser);
  const navigate = useNavigate();
  const location = useLocation();
  // if (!(currentUser?.token?.length > 10)) {
  //   // اگر لاگین نبود، می‌فرستیم صفحه ورود
  //   return <Navigate to="/login" replace />;
  // }
  // else if (currentUser?.uName?.toLowerCase() === "super" && window.location.pathname !== "/admin-panel") {
  //   return <Navigate to="/admin-panel" replace/>
  // }

  //چون روش بالا با outlet جواب نمیداد از روش زیر استفاده شده

  useEffect(() => {
    if (!(currentUser?.token?.length > 10)) {
      navigate("/login", { replace: true });
      return;
    }
    if (currentUser?.uName?.toLowerCase() === "super" && location.pathname !== "/admin-panel") {
      navigate("/admin-panel", { replace: true });
      return;
    }
  }, [currentUser, navigate, location.pathname]);
  
  return <Outlet />;
}
