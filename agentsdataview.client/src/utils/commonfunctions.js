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