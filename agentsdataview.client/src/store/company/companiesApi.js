import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { getError } from "../../utils/commonfunctions";
const BASE_URL = "/api/Companies";
export const companiesApi = createApi({
    reducerPath: "companiesApi",
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
        getAllCompanies: builder.query({
            query: () => "/GetAll",
            providesTags: ["CompanyList"],
            keepUnusedDataFor: 60,
            transformErrorResponse: (response) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),
    createCompany: builder.mutation({
      query: (data) => ({
        url: "/Create",
        body: data,
        method: "POST"
      }),
      transformErrorResponse: (response, meta, arg) => {
        return getError(response);
      },
      invalidatesTags: ["CompanyList"]
    }),        
        updateCompany: builder.mutation({
            query: (data) => ({
                url: "/Update",
                method: "PUT",
                body: data
            }),
            invalidatesTags:["CompanyList"],
            transformErrorResponse: (response) => {
                return getError(response);
            },
            transformResponse: (res) => res.data
        }),

    })
});
export const { useLazyGetAllCompaniesQuery, useUpdateCompanyMutation, useCreateCompanyMutation } = companiesApi;