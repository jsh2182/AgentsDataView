// src/components/UserPanel.jsx
import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchProfile, loginUser, logout } from "../store/user/userSlice";
import { fetchUserSettingsWithCallback } from "../store/user/userAPI";

const UserPanel = () => {
    const dispatch = useDispatch();
    const { currentUser, loading, error } = useSelector((state) => state.user);

    const [settings, setSettings] = useState({});

    useEffect(() => {
        dispatch(fetchProfile());
        fetchUserSettingsWithCallback(setSettings); // برای useState
    }, [dispatch]);

    const handleLogin = () => {
        dispatch(loginUser({ username: "admin", password: "123456" }));
    };

    const handleLogout = () => {
        dispatch(logout());
    };

    return (
        <div className="p-4 border rounded">
            <h2 className="text-lg font-bold mb-2">پنل کاربر</h2>

            {loading.fetchProfile && <p>در حال دریافت اطلاعات پروفایل...</p>}
            {loading.loginUser && <p>در حال ورود...</p>}
            {error.fetchProfile && <p className="text-red-500">{error.fetchProfile}</p>}
            {error.loginUser && <p className="text-red-500">{error.loginUser}</p>}

            {currentUser ? (
                <div>
                    <p>خوش آمدید، {currentUser.fullName}</p>
                    <button onClick={handleLogout} className="btn">خروج</button>
                </div>
            ) : (
                <button onClick={handleLogin} className="btn">ورود</button>
            )}

            <hr className="my-4" />

            <h3 className="font-semibold">تنظیمات</h3>
            <pre className="bg-gray-100 p-2 rounded text-sm">
                {JSON.stringify(settings, null, 2)}
            </pre>
        </div>
    );
};

export default UserPanel;
