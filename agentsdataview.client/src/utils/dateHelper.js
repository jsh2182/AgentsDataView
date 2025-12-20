import axios from "axios";

export async function fetchIranHolidaysFromOfficialApi(requestedYear = 1404) {
  const storageKeyPrefix = "iran_holidays_";

  // پیدا کردن کلید ذخیره شده
  const savedKeys = Object.keys(localStorage).filter((k) =>
    k.startsWith(storageKeyPrefix)
  );

  // حذف کلید قبلی اگر سال متفاوت بود
  savedKeys.forEach((key) => {
    const savedYear = key.replace(storageKeyPrefix, "");
    if (savedYear !== String(requestedYear)) {
      localStorage.removeItem(key);
    }
  });

  const currentKey = `${storageKeyPrefix}${requestedYear}`;

  // بررسی کش سال جاری
  const cached = localStorage.getItem(currentKey);
  if (cached) {
    try {
      return JSON.parse(cached);
    } catch {
      localStorage.removeItem(currentKey);
    }
  }

  // واکشی داده جدید و ذخیره
  try {
    const { data } = await axios.get(
      `https://api.persian-calendar.ir/api/v1/calendar/${requestedYear}/holidays`
    );
    const holidayMap = {};
    data?.data?.forEach((item) => {
      holidayMap[item.date] = item.holidayDesription;
    });

    localStorage.setItem(currentKey, JSON.stringify(holidayMap));
    return holidayMap;
  } catch (err) {
    console.error("خطا در دریافت تعطیلات:", err);
    return {};
  }
}
