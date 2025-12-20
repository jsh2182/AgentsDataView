import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import plugin from "@vitejs/plugin-react";
import fs from "fs";
import path from "path";
import child_process from "child_process";
import { env } from "process";
import { VitePWA } from "vite-plugin-pwa";
// تعیین مسیر گواهینامه دات‌نت (ویندوز یا لینوکس/مک)
const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ""
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

// نام فایل گواهینامه
const certificateName = "agentsdataview.client"
// مسیر فایل کلید و گواهینامه
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

// تلاش برای ساخت گواهینامه در صورت عدم وجود
try {
    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        const result = child_process.spawnSync(
            "dotnet",
            [
                "dev-certs",
                "https",
                "--export-path",
                certFilePath,
                "--format",
                "Pem",
                "--no-password",
            ],
            { stdio: "inherit" }
        );
        if (result.status !== 0) {
            console.error(
                "خطا در ساخت گواهینامه توسعه دات‌نت. مطمئن شوید dotnet نصب و در PATH سیستم است."
            );
            process.exit(1);
        }
    }
} catch (error) {
    console.error("خطا در دسترسی یا ساخت فایل‌های گواهینامه:", error);
    process.exit(1);
}

// تعیین آدرس بک‌اند دات‌نت برای پروکسی
const target = env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS
        ? env.ASPNETCORE_URLS.split(";")[0]
        : "https://localhost:7037";

// گرفتن پورت فرانت‌اند از متغیر محیطی یا مقدار پیش‌فرض
const frontendPort = env.VITE_PORT || 5173;

// یک بار خواندن کلید و گواهینامه برای استفاده در HTTPS
const key = fs.readFileSync(keyFilePath);
const cert = fs.readFileSync(certFilePath);

export default defineConfig({
    plugins: [
        plugin(),
        VitePWA({
            registerType: "autoUpdate",
            includeAssets: ["favicon.svg", "robots.txt", "apple-touch-icon.png"],
            manifest: {
                name: "TIP Dashboard",
                short_name: "TIP Dashboard",
                description: "TIP Dashboard Progressive Web App",
                theme_color: "#ffffff",
                icons: [
                    {
                        src: "pwa-192x192.png",
                        sizes: "192x192",
                        type: "image/png",
                    },
                    {
                        src: "pwa-512x512.png",
                        sizes: "512x512",
                        type: "image/png",
                    },
                    {
                        src: "pwa-512x512.png",
                        sizes: "512x512",
                        type: "image/png",
                        purpose: "any maskable",
                    },
                ],
            },
        }),
    ],
    resolve: {
        alias: {
            "@": fileURLToPath(new URL("./src", import.meta.url)),
        },
    },
    server: {
        // پروکسی درخواست‌های /api به بک‌اند
        proxy: {
            "/api": {
                target,
                changeOrigin: true,
                secure: false,
            },
            "/api_docs": {
                target,
                changeOrigin: true,
                secure: false,
            },
        },
        port: Number(frontendPort),
        https: {
            key,
            cert,
        },
    },
});

