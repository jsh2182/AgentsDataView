export function getTextWidth(text, font) {
  const canvas = document.createElement("canvas");
  const context = canvas.getContext("2d");
  if (!context) return 0;
  context.font = font; // مثلاً "14px IRANSans"
  const metrics = context.measureText(text);
  return metrics.width;
}
export const getError = (response) => {
  let message = response.data?.Message || response.data?.message || "خطای ناشناخته رخ داده است.";
  try {
    const data = JSON.parse(message);
    message = data.Exception;
    console.error(data.StackTrace);
    return message;
  }
  catch {

  }
  return message;
}
export function keyIsLetterOrNumber(key) {
    //const key = event.key;
    if (key === ' ') return true;
    // اعداد انگلیسی
    const isEnglishDigit = /^[0-9]$/.test(key);

    // حروف انگلیسی (a-z یا A-Z)
    const isEnglishLetter = /^[a-zA-Z]$/.test(key);

    // حروف فارسی
    const isPersianLetter = /^[\u0600-\u06FF]$/.test(key);

    // اعداد فارسی (۰ تا ۹)
    const isPersianDigit = /^[\u06F0-\u06F9]$/.test(key);
    return isEnglishDigit || isEnglishLetter || isPersianLetter || isPersianDigit;
}