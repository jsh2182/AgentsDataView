import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { getError } from "../../utils/commonfunctions";
const BASE_URL = "/api/Cities";
export const citiesApi = createApi({
    reducerPath: "citiesApi",
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
        getAllCities: builder.query({
            query: (provinceId) => `/GetALL?provinceId=${provinceId}`,
            keepUnusedDataFor: 60,
            transformErrorResponse: (response, meta, arg) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),


    })
});
export const { useLazyGetAllCitiesQuery } = citiesApi;