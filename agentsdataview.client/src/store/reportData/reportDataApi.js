import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { getError } from "../../utils/commonfunctions";
const BASE_URL = "/api/Reports";
export const reportsApi = createApi({
    reducerPath: "reportsApi",
    baseQuery: fetchBaseQuery({
        baseUrl: BASE_URL,
        prepareHeaders: (headers, { getState }) => {
            const token = getState().user?.currentUser?.token;
            if (token) {
                headers.set("Authorization", `Bearer ${token}`);
            }
            return headers;
        }
    }),
    endpoints: (builder) => ({
        getReportByProvince: builder.query({
            query: (provinceId) => `/GetReportByProvince?provinceId=${provinceId ?? ""}`,
            keepUnusedDataFor: 60,
            refetchOnMountOrArgChange: true,
            transformErrorResponse: (response, meta, arg) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),
        getReportByCompanyAndProduct: builder.query({
            query: (provinceId) => `/GetReportByCompanyAndProduct?provinceId=${provinceId ?? ""}`,
            keepUnusedDataFor: 60,
            refetchOnMountOrArgChange: true,
            transformErrorResponse: (response, meta, arg) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),
        getProfitReportByCompany: builder.query({
            query: (provinceId) => `/GetProfitReportByCompany?provinceId=${provinceId ?? ""}`,
            keepUnusedDataFor: 60,
            refetchOnMountOrArgChange: true,
            transformErrorResponse: (response, meta, arg) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),
        getProfitReportByProvince: builder.query({
            query: () => `/GetProfitReportByProvince?provinceId`,
            keepUnusedDataFor: 60,
            refetchOnMountOrArgChange: true,
            transformErrorResponse: (response, meta, arg) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),
        getReportByProvince_Cumulative: builder.query({
            query: () => "/GetReportByProvince_Cumulative",
            keepUnusedDataFor: 60,
            refetchOnMountOrArgChange: true,
            transformErrorResponse: (response, meta, arg) => {
                return getError(response);
            },
            transformResponse: (res) => res.data            
        })
    })
});
export const { useLazyGetProfitReportByCompanyQuery, 
    useLazyGetProfitReportByProvinceQuery,
     useLazyGetReportByCompanyAndProductQuery, 
     useLazyGetReportByProvinceQuery, 
     useGetReportByProvince_CumulativeQuery } = reportsApi;