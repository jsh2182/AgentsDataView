# AgentsDataView

**Full-Stack Web Application** built with **.NET 8** and **React + Vite**.

---

## 🚀 Project Overview

AgentsDataView is a modern full-stack web application that combines a **scalable .NET 8 backend API** with a **React (Vite) frontend**. The project demonstrates best practices in full-stack development, secure API design, and responsive, interactive UI.

---

## 🧩 Key Features

- **Frontend:** React 19 + Vite SPA with routing via React Router  
- **State Management:** Redux Toolkit + React Hooks + React Hook Form  
- **Styling & UI:** Bootstrap 5, React Bootstrap, React Icons, React Spinners  
- **Charts & Visualization:** Recharts for dynamic data representation  
- **PWA Support:** vite-plugin-pwa  
- **Backend:** ASP.NET Core Web API (.NET 8) with JWT Authentication  
- **ORM & DB:** Entity Framework Core + SQL Server  
- **Logging:** NLog (file + database)  
- **API Documentation:** Swagger / Swashbuckle with annotations  

---

## 🛠️ Tech Stack

| Layer        | Technology |
|--------------|------------|
| Backend      | .NET 8, ASP.NET Core Web API, JWT Authentication |
| Frontend     | React 19, Vite, Redux Toolkit, React Router, React Hook Form, Bootstrap 5 |
| Charts / UI  | Recharts, React Spinners, React Icons |
| ORM & DB     | Entity Framework Core, SQL Server |
| Logging      | NLog, NLog.Database, NLog.Web.AspNetCore |
| API Docs     | Swashbuckle / Swagger |
| PWA          | vite-plugin-pwa |

---

## 📦 Setup & Run

### Backend
```bash
cd backend
dotnet restore
dotnet build
dotnet run
### Frontend
cd ../agentsdataview.client
npm install
npm run dev
SPA runs via ASP.NET Core SPA Proxy on https://localhost:5173.
