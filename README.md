# 📊 AgentsDataView

A modern **full‑stack web application** with a scalable .NET 8 backend and a React + Vite frontend — built for interactive data visualization and management.

---

## 🧩 Project Overview

**AgentsDataView** is a full‑stack solution that combines:

✔ a **.NET 8 ASP.NET Core Web API** backend
✔ **React (Vite)** frontend with SPA architecture
✔ Secure JWT authentication
✔ Responsive UI with charts and state‑of‑the‑art UX

This project demonstrates best practices in API design, frontend state management, clean architecture, and modern web development workflows. ([github.com](https://github.com/jsh2182/AgentsDataView))

---

## 🚀 Key Features

* **Frontend:** React 19 + Vite SPA with routing using React Router
* **State Management:** Redux Toolkit + React Hooks + React Hook Form
* **Styling & UI:** Bootstrap 5, React Bootstrap, React Icons
* **Charts & Visualization:** Recharts for dynamic visual data representation
* **PWA Support:** vite-plugin-pwa
* **Backend API:** .NET 8 ASP.NET Core Web API with JWT authentication
* **Database:** Entity Framework Core with SQL Server
* **Logging:** NLog (file + database)
* **API Doc:** Swagger / Swashbuckle

---

## 🛠 Architecture

```
AgentsDataView.sln
├── AgentsDataView.Server         # ASP.NET Core Web API (.NET 8)
├── agentsdataview.client         # React + Vite frontend
├── README.md
├── .editorconfig
├── .gitignore
```

Promotes:
✔ Separation of concerns
✔ Scalable frontend/backend integration
✔ Easy extensibility

---

## 📦 Tech Stack

| Layer        | Technology                   |
| ------------ | ---------------------------- |
| Backend      | .NET 8 ASP.NET Core Web API  |
| Frontend     | React 19 + Vite              |
| State        | Redux Toolkit                |
| UI & Styling | Bootstrap 5, React Bootstrap |
| Charts       | Recharts                     |
| ORM          | Entity Framework Core        |
| Database     | SQL Server                   |
| Auth         | JWT                          |
| Logging      | NLog                         |
| API Docs     | Swagger / Swashbuckle        |

---

## 📌 Getting Started

### Prerequisites

Install:

* .NET 8 SDK
* Node.js (for frontend tooling)
* SQL Server or another compatible database

---

### Setup

#### Backend

1. **Clone the repo**

   ```bash
   git clone https://github.com/jsh2182/AgentsDataView.git
   cd AgentsDataView.Server
   ```

2. **Restore and build**

   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the API**

   ```bash
   dotnet run
   ```

#### Frontend

1. Open a new shell:

   ```bash
   cd ../agentsdataview.client
   npm install
   npm run dev
   ```

2. The SPA will run via ASP.NET Core SPA Proxy at:

   ```
   https://localhost:5173
   ```

---

## 🧪 Testing

You can add **unit tests** and **integration tests** for:

* Backend controllers and services
* Frontend components and state logic

(Test frameworks like xUnit, Jest, or React Testing Library recommended.)

---

## 🎯 Use Cases

This solution is suited for:

✅ Dashboard and data visualization systems
✅ Enterprise‑grade full‑stack applications
✅ Demonstration of modern React + .NET integration
✅ Learning example for SPA + API architecture

---

## 👨‍💻 Developer Notes

* Clean separation between backend API and SPA frontend
* Easy to extend for multi‑tenant or real-time features
* Ready to plug additional modules (e.g., role/permission, reporting)

---

## 🚀 What’s Next?

Future improvements:

✔ End‑to-end tests
✔ CI/CD automation with GitHub Actions
✔ Deployment to cloud (Azure / AWS / DigitalOcean)
✔ Additional analytics dashboards
