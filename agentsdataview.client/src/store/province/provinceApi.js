import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { getError } from "../../utils/commonfunctions";
const BASE_URL = "/api/Provinces";
export const provincesApi = createApi({
    reducerPath: "provincesApi",
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
        getAllProvinces: builder.query({
            query: (filterOnUser) => `/GetALL?filterOnUser=${filterOnUser}`,
            keepUnusedDataFor: 60,
            transformErrorResponse: (response, meta, arg) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),
       
        
    })
});
export const { useLazyGetAllProvincesQuery} = provincesApi;