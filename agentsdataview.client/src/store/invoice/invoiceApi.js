import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { getError } from "../../utils/commonfunctions";
const BASE_URL = "/api/Invoices";
export const invoiceApi = createApi({
    reducerPath: "invoiceApi",
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
        getLastUpdate: builder.query({
            query: () => "/GetLastUpdate",
            keepUnusedDataFor: 60,
            transformErrorResponse: (response) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),  
        updateAll: builder.mutation({
            query: () => ({
                url:"/UpdateAll",
                method:"PUT",
            }),
        
            transformErrorResponse: (response) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),            
        
    })
});
export const { useGetLastUpdateQuery, useUpdateAllMutation} = invoiceApi;