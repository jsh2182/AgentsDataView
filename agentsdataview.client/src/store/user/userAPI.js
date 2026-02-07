// src/store/user/userApi.js
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { setCurrentUser, logout } from "./userSlice";
import { getError } from "../../utils/commonfunctions";

const BASE_URL = "/api/User";

export const userApi = createApi({
  reducerPath: "userApi",
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
    // 🟩 لاگین
    loginUser: builder.mutation({
      query: (credentials) => ({
        url: "/Login",
        method: "POST",
        body: credentials,
      }),
      transformErrorResponse: (response, meta, arg) => {
        return getError(response);
      },
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          const token = data.data.access_token;
          const exp = Date.now() + data.data.expires_in * 1000;
          const full_name = data.data.token_for;
          const mobile = data.data.u_mobile;
          const userData = { token, exp, full_name, uName: arg.username, u_mobile: mobile };
          dispatch(setCurrentUser(userData));
        } catch {
          dispatch(logout());
        }
      },
    }),


    fetchProfile: builder.query({
      query: () => "/Profile",
      keepUnusedDataFor: 0,
      refetchOnMountOrArgChange: true,
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setCurrentUser(data));
        } catch {
          dispatch(logout());
        }
      },
    }),
    fetchUserList: builder.query({
      query: (companyId) => `/GetAll?companyId=${companyId ?? ""}`,
      providesTags: ["UserList"],
      keepUnusedDataFor: 0,
      refetchOnMountOrArgChange: true,
      transformErrorResponse: (response, meta, arg) => {
        return getError(response);
      },
      transformResponse: (res) => res.data
    }),
    createUser: builder.mutation({
      query: (data) => ({
        url: "/Create",
        body: data,
        method: "POST"
      }),
      transformErrorResponse: (response, meta, arg) => {
        return getError(response);
      },
      invalidatesTags: ["UserList"]
    }),
    deleteUser: builder.mutation({
      query: (id) => ({
        url: `/Delete?id=${id}`,
        method: "Delete"
      }),
      transformErrorResponse: (response, meta, arg) => {
        return getError(response);
      },
      invalidatesTags: ["UserList"]
    }),
    updateMe: builder.mutation({
      query: (data) => ({
        url: `/UpdateMe`,
        method: "PUT",
        body: data
      }),

      transformErrorResponse: (response, meta, arg) => {
        return getError(response);
      },
    }),
    updateUser: builder.mutation({
      query: (data) => ({
        url: `/Update`,
        method: "PUT",
        body: data
      }),
      transformErrorResponse: (response, meta, arg) => {
        return getError(response);
      },
      invalidatesTags: ["UserList"]
    }),
  }),
});

export const {
  useLoginUserMutation,
  useFetchProfileQuery,
  useLazyFetchUserListQuery,
  useCreateUserMutation,
  useDeleteUserMutation,
  useUpdateMeMutation,
  useUpdateUserMutation } = userApi;


