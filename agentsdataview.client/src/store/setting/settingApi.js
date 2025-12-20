import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { getError } from "../../utils/commonfunctions";

const BASE_URL = "/api/Settings";

export const settingApi = createApi({
    reducerPath: "settingApi",
    baseQuery: fetchBaseQuery({
        baseUrl: BASE_URL,
        prepareHeaders: (headers, { getState }) => {
            const token = getState().user.currentUser?.token;
            if (token) {
                headers.set("Authorization", `Bearer ${token}`);
            }
            return headers;
        }
    }),
    endpoints: (builder) => ({

        getSetting: builder.query({
            query: (name) => `/GetSetting?name=${name}`,
            keepUnusedDataFor: 0,
            refetchOnMountOrArgChange: true,
            transformErrorResponse: (response) => {
                return getError(response);
            },
            transformResponse: (res) =>res.data,

            transformErrorResponse: (response) => {
                return getError(response);
            },
        }),
        updateSetting: builder.mutation({
            query: (data) => ({
                url: `/UpdateSetting`,
                method: "PUT",
                body: data
            }),
            transformErrorResponse: (response) => {
                return getError(response);
            },
        }),
    }),
});

export const { useLazyGetSettingQuery, useUpdateSettingMutation } = settingApi;


