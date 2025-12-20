import {
  FaLaptop,
  FaCog,
  FaShoppingBasket,
  FaHeadset,
  FaCreditCard,
  FaGlobe,
  FaMobileAlt,
} from "react-icons/fa";
import { Link } from "react-router-dom";

export default function NotFoundPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-sky-100 via-cyan-100 to-cyan-200 dark:from-gray-800 dark:via-gray-900 dark:to-gray-950 px-6">
      <div className="max-w-md text-center bg-white/70 dark:bg-white/10 backdrop-blur-sm rounded-2xl p-8 shadow-lg border border-white/40 dark:border-white/10">
        <div className="flex justify-center gap-4 text-cyan-600 dark:text-cyan-400 text-5xl mb-6">
          {/* <FaSearchMinus className="" title="Page Not Found" />
          <MdSupportAgent className="" title="Support" /> */}
        </div>
        <h1 className="text-4xl font-bold text-gray-800 dark:text-white mb-4">
          صفحه درخواستی یافت نشد.
        </h1>
        <p className="text-gray-600 dark:text-gray-300 text-base mb-6">
          متأسفانه صفحه‌ای که دنبال آن بودید پیدا نشد. اگر به پشتیبانی نیاز
          دارید، تیم خدمات پس از فروش همراه شماست.
        </p>
        <Link
          to="/"
          className="inline-block mt-2 px-6 py-2 rounded-full bg-cyan-600 hover:bg-cyan-700 text-white text-sm transition-all"
        >
          بازگشت به صفحه اصلی
        </Link>
      </div>
    </div>
  );
}

function GlobeWithRotatingIcons({
  className = "text-cyan-600",
  size = 160,
  iconSize = 28,
  rotationDuration = 20, // ثانیه
}) {
  const wrapperStyle = {
    width: size,
    height: size,
    position: "relative",
  };

  // زاویه های آیکون‌ها (۶ عدد)
  const iconPositions = [
    { angle: 270 }, // بالا - لپ تاپ
    { angle: 210 }, // بالا چپ - موبایل
    { angle: 150 }, // چپ - چرخ دنده
    { angle: 90 }, // پایین چپ - سبد خرید
    { angle: 30 }, // پایین راست - هدفون
    { angle: 330 }, // بالا راست - کارت بانکی
  ];

  const icons = [
    { Icon: FaLaptop, label: "لپ تاپ" },
    { Icon: FaMobileAlt, label: "موبایل" },
    { Icon: FaCog, label: "چرخ دنده" },
    { Icon: FaShoppingBasket, label: "سبد خرید" },
    { Icon: FaHeadset, label: "هدفون" },
    { Icon: FaCreditCard, label: "کارت بانکی" },
  ];

  const radius = size / 2 + 10; // شعاع چرخش

  return (
    <div
      className={`${className} relative`}
      style={wrapperStyle}
      aria-label="Globe with rotating service and sales icons"
      role="img"
    >
      {/* کره زمین ثابت وسط */}
      <FaGlobe
        size={size * 0.7}
        className="mx-auto block absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"
        aria-hidden="true"
      />

      {/* کانتینر آیکون‌ها که میچرخه */}
      <div
        className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2"
        style={{
          width: size,
          height: size,
          animation: `spin ${rotationDuration}s linear infinite`,
        }}
      >
        {icons.map(({ Icon, label }, i) => {
          const angle = iconPositions[i].angle;
          const rad = (angle * Math.PI) / 180;
          const x = radius * Math.cos(rad);
          const y = radius * Math.sin(rad);

          return (
            <div
              key={i}
              aria-label={label}
              title={label}
              className="absolute"
              style={{
                top: size / 2 - y - iconSize / 2,
                left: size / 2 + x - iconSize / 2,
                width: iconSize,
                height: iconSize,
              }}
            >
              <Icon size={iconSize} />
            </div>
          );
        })}
      </div>

      <style>{`
        @keyframes spin {
          from { transform: rotate(0deg); }
          to { transform: rotate(360deg); }
        }
      `}</style>
    </div>
  );
}
