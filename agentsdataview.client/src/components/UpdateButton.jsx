import { useEffect, useState, useRef } from "react";
import { FiRefreshCw } from "react-icons/fi";
import { useGetLastUpdateQuery, useUpdateAllMutation } from "../store/invoice/invoiceApi";
import { MoonLoader } from "react-spinners";
import { MdOutlineAccessTime } from "react-icons/md";
import { BiCircle, BiRadioCircle } from "react-icons/bi";
import { FaCircle } from "react-icons/fa6";
import useIsMobile from "../hooks/useIsMobile";

const COOLDOWN_MS = 30 * 60 * 1000;
const STORAGE_KEY = "update_button_cooldown";
const PERSIAN_DIGITS = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'];
const UpdateButton = () => {
  const isMobile = useIsMobile();
  const [timeLeft, setTimeLeft] = useState(0);
  const { data: lastUpdate } = useGetLastUpdateQuery();
  const [updateAll, { isLoading: loadingUpdateAll }] = useUpdateAllMutation();
  const timerRef = useRef(null); // Ref برای نگهداری تایمر

  // بررسی cooldown بعد از refresh
  useEffect(() => {
      const lastClick = localStorage.getItem(STORAGE_KEY);
      if (lastClick) {
          const diff = COOLDOWN_MS - (Date.now() - Number(lastClick));
          if (diff > 0) {
              setTimeLeft(Math.ceil(diff / 1000));
          }
      }
  }, []);

  // تایمر شمارش معکوس (یک بار ساخته می‌شود)
  useEffect(() => {
      if (timeLeft <= 0) return;

      if (!timerRef.current) {
          timerRef.current = setInterval(() => {
              setTimeLeft(prev => {
                  if (prev <= 1) {
                      clearInterval(timerRef.current);
                      timerRef.current = null;
                      return 0;
                  }
                  return prev - 1;
              });
          }, 1000);
      }

      return () => {
          if (timerRef.current && timeLeft <= 0) {
              clearInterval(timerRef.current);
              timerRef.current = null;
          }
      };
  }, [timeLeft]);

  const handleClick = async () => {
    localStorage.setItem(STORAGE_KEY, Date.now());
    setTimeLeft(COOLDOWN_MS / 1000);
    await updateAll().unwrap();
    window.location.reload();
  };

  const formatTime = (seconds) => {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m.toString().replace(/\d/g, (d) => PERSIAN_DIGITS[d])}:${s.toString().padStart(2, "0").replace(/\d/g, (d) => PERSIAN_DIGITS[d])}`;
  };

  return (
    <div style={{
      // position: "fixed",
      // top: "32px",
      // right: "33px",
      display: "flex",
      borderRadius: "8px",
      overflow: "hidden",
      boxShadow: "0 2px 6px rgba(0,0,0,0.15)",
      maxWidth: "fit-content"
    }}>
      {/* Label سمت چپ */}
      {lastUpdate && !isMobile &&
        <div style={{
          background: "linear-gradient(135deg, #307d9bff, #9bbffeff)",
          color: "#fff",
          padding: "4px 10px",
          fontSize: "11px",
          fontWeight: 600,
          display: "flex",
          flexDirection: "column",
          justifyContent: "center"
        }}>
          <div>آخرین بروزرسانی</div>
          <div style={{ direction: "ltr" }}>{lastUpdate.date} <FaCircle size={4} /> {lastUpdate.time}</div>
        </div>
      }

      {/* دکمه سمت راست */}
      <button
        onClick={handleClick}
        disabled={timeLeft > 0 || loadingUpdateAll}
        style={{
          border: "none", // هیچ border اضافی
          background: "linear-gradient(135deg, #354f8d, #83b3edfc)",
          color: "#fff",
          padding:isMobile ?"3px 8px": "4px 10px",
          fontSize: "13px",
          display: "flex",
          alignItems: "center",
          gap: "6px",
          cursor: timeLeft > 0 || loadingUpdateAll ? "not-allowed" : "pointer",
          boxShadow: "0 0 5px black"
        }}
        className="hover-button"
      //className="btn"
      >
        {loadingUpdateAll ? (
          <MoonLoader size={18} color="#fff" />
        ) : (
          <>
            {!isMobile && <FiRefreshCw size={18} />}
            {timeLeft > 0 ? `بعد از ${formatTime(timeLeft)}` : "بروز رسانی"}
          </>
        )}
      </button>
    </div>

  );
};

export default UpdateButton;
