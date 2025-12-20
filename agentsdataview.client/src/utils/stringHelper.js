export const toPersianNumberIfPersianText = (input) => {
  if (!input) {
    return "";
  }
  const str = input.toString();
  const hasLetter = /[a-zA-Z]/.test(str); // اگه حروف انگلیسی داره
  if (hasLetter) return str; // انگلیسیه → تغییر نده
  return str.replace(/\d/g, (d) => "۰۱۲۳۴۵۶۷۸۹"[d]); // در بقیه موارد فارسی کن
};
