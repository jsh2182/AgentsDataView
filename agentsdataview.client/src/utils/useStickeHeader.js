import { useEffect } from "react";

/**
 * هوک برای فیکس کردن header جدول‌ها
 * @param {React.RefObject[] | React.RefObject} refs - یک ref یا آرایه‌ای از refها
 * @param {string} bgColor -رنگ پس زمینه هدر
 */
export function useStickyHeader(refs, bgColor ="#fff") {
  useEffect(() => {
    // اگه فقط یه ref پاس داده شده، تبدیلش می‌کنیم به آرایه برای سادگی
    const refArray = Array.isArray(refs) ? refs : [refs];

    refArray.forEach((ref) => {
      const table = ref.current;
      if (!table) return;

      const thead = table.querySelector("thead");
      if (!thead) return;

      const rows = thead.querySelectorAll("tr");
      let offset = 0;

      rows.forEach((row) => {
        row.querySelectorAll("th").forEach((th) => {
          th.style.position = "sticky";
          th.style.top = offset + "px";
          th.style.background = bgColor;
          th.style.zIndex = 2;
          th.style.border ="0.5px solid gray"
        });
        offset += row.offsetHeight;
      });
    });
  }, [refs]);
}
