import React, { useState } from "react";
import { Routes, Route, Navigate } from "react-router-dom";

import Home from "../pages/Home";
// import UserProfile from "../pages/user/UserProfile";
import Login from "../pages/user/Login";

import PrivateRoutes from "./PrivateRoutes";
import PublicRoutes from "./PublicRoutes";
import RegisterUser from "../pages/user/RegisterUser";
import NotFoundPage from "../pages/NotFoundPage";
import AdminPanel from "../pages/user/AdminPanel";

export default function AppRoutes() {
  return (
    <Routes>
      {/* صفحه ورود (عمومی) */}
      <Route
        path="/login"
        element={
          <PublicRoutes>
            <Login />
          </PublicRoutes>
        }
      />
      <Route
        path="/register-user"
        element={
          <PublicRoutes>
            <RegisterUser />
          </PublicRoutes>
        }
      />


      <Route element={<PrivateRoutes />}>
        <Route path="/admin-panel" element={<AdminPanel />} />
        <Route path="/" element={<Home />} />
      </Route>
      {/* <Route path="/UserProfile" element={<UserProfile />} /> */}
      {/* سایر صفحات عمومی یا خطا */}
      <Route path="/unauthorized" element={<p>دسترسی غیرمجاز</p>} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
