import { Navigate } from "react-router-dom";
import { useSelector } from "react-redux";

export default function PublicRoutes({ children }) {
  const currentUser = useSelector((state) => state.user.currentUser);

  if (currentUser?.token?.length > 10) {
    // اگر کاربر لاگین بود، اجازه ورود به صفحه عمومی مثل login نمی‌ده و می‌فرسته صفحه اصلی
    return <Navigate to="/" replace />;
  }

  return children;
}
