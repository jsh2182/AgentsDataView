// utilities/apiHelper.js
import axios from "axios";
/**
 * نسخه پیشرفته مناسب برای استفاده با createAsyncThunk و مدیریت خطا در Redux
 * @param {'get'|'post'|'put'|'delete'} method
 * @param {string} url
 * @param {object} data - برای get: query / برای post: body
 * @param {function} rejectWithValue - از createAsyncThunk
 * @param {function} [onSuccess] - کال‌بک موفقیت
 * @param {function} [onError] - کال‌بک خطا
 * @param {AbortSignal} [signal] - سیگنال لغو درخواست (اختیاری)
 */
export async function callApiRedux(
  method,
  url,
  data,
  rejectWithValue,
  onSuccess,
  onError,
  signal
) {
  try {
    const currUser = JSON.parse(localStorage.getItem("currUser"));
    const token = currUser?.token;
    const fullUrl = `/${url.replace(/^\/+/, "")}`;
    const config = {
      method: method?.toLowerCase?.() || "get",
      url: fullUrl,
      headers: {
        Authorization: token ? `Bearer ${token}` : undefined,
      },
      signal,
    };

    if (data) {
      if (config.method === "get") {
        config.params = data;
      } else {
        config.data = data;
      }
    }

    const res = await axios(config);

    if (!res.data.isSuccess) {
      onError?.(res.data);
      return rejectWithValue?.(res.data.message || "خطای منطقی از سرور");
    }

    onSuccess?.(res.data);
    return res.data;
  } catch (error) {
    const message = getErrorMessage(error);
    onError?.(message);
    if (axios.isCancel(error)) {
      return rejectWithValue("درخواست لغو شد.");
    }
    return rejectWithValue?.(message);
  }
}

/**
 * نسخه ساده برای استفاده مستقل فقط با callback
 * @param {'get'|'post'|'put'|'delete'} method
 * @param {string} url
 * @param {object} data - برای get: query / برای post: body
 * @param {function} [onSuccess] - کال‌بک موفقیت
 * @param {function} [onError] - کال‌بک خطا
 * @param {AbortSignal} [signal] - سیگنال لغو درخواست (اختیاری)
 */
export async function callApiSimple(
  method,
  url,
  data,
  onSuccess,
  onError,
  signal
) {
  try {
    const currUser = JSON.parse(localStorage.getItem("currUser"));
    const token = currUser?.token;

    const config = {
      method: method?.toLowerCase?.() || "get",
      url,
      headers: {
        Authorization: token ? `Bearer ${token}` : undefined,
      },
      signal,
    };

    if (data) {
      if (config.method === "get") {
        config.params = data;
      } else {
        config.data = data;
      }
    }

    const res = await axios(config);

    if (!res.data.isSuccess) {
      onError?.(res.data.message || "خطای منطقی از سرور");
      return;
    }

    onSuccess?.(res.data);
    return res.data;
  } catch (error) {
    if (axios.isCancel(error)) {
      const message = getErrorMessage(error);
      onError?.(message);
      return;
    }
    const message = getErrorMessage(error);
    onError?.(message);
  }
}

export const apiModalResultType = {
  info: "info",
  confirm: "confirm",
  warning: "warning",
  error: "error",
};
function getErrorMessage(error) {
  const message =
    error.response?.data?.message ||
    error.response?.data?.Message ||
    error.response?.statusText ||
    error.message ||
    "خطای ناشناخته";
  try {
    const errorObj = JSON.parse(message);
    if (errorObj?.Exception) {
      return errorObj.Exception;
    }
  } catch (e) {}
  return message;
}
