// src/store/store.js
import { configureStore } from "@reduxjs/toolkit";
import userReducer from "./user/userSlice";
import { userApi } from "./user/userAPI";
import { reportsApi } from "./reportData/reportDataApi";
import { provincesApi } from "./province/provinceApi";
import { invoiceApi } from "./invoice/invoiceApi";
import { citiesApi } from "./city/cityApi";
import { companiesApi } from "./company/companiesApi";
import { settingApi } from "./setting/settingApi";

const store = configureStore({
  reducer: {
    user: userReducer,
    [userApi.reducerPath]: userApi.reducer,
    [reportsApi.reducerPath]: reportsApi.reducer,
    [provincesApi.reducerPath]: provincesApi.reducer,
    [invoiceApi.reducerPath]: invoiceApi.reducer,
    [citiesApi.reducerPath]: citiesApi.reducer,
    [companiesApi.reducerPath]: companiesApi.reducer,
    [settingApi.reducerPath]: settingApi.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware()
      .concat(userApi.middleware)
      .concat(reportsApi.middleware)
      .concat(provincesApi.middleware)
      .concat(invoiceApi.middleware)
      .concat(citiesApi.middleware).
      concat(companiesApi.middleware)
      .concat(settingApi.middleware)
  ,
});
export default store;
