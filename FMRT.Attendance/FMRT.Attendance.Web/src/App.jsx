import {
  BrowserRouter,
  Routes,
  Route,
  NavLink,
  useLocation,
} from "react-router-dom";

import { useState } from "react";

import Dashboard from "./pages/Dashboard";
import AttendanceHistory from "./pages/AttendanceHistory";
import Calendar from "./pages/Calendar";
import Profile from "./pages/Profile";
import Login from "./pages/Login";
import HolidayManagement from "./pages/HolidayManagement";
import LeaveManagement from "./pages/LeaveManagement";
import AttendanceProcessing from "./pages/AttendanceProcessing";
import AdminLeaves from "./pages/AdminLeaves";
import Reports from "./pages/Reports";
import EmployeeSelector from "./components/EmployeeSelector";
import { useAuth } from "./context/AuthContext";
import EmployeeManagement from "./pages/EmployeeManagement";

function MenuItem({
  to,
  children,
  onClick,
}) {
  return (
    <NavLink
      to={to}
      onClick={onClick}
      className={({ isActive }) =>
        `block p-3 rounded-xl transition ${
          isActive
            ? "bg-blue-600 text-white"
            : "hover:bg-slate-800"
        }`
      }
    >
      {children}
    </NavLink>
  );
}

function AppLayout() {
  const location = useLocation();

  const {
    user,
    logout,
    portalMode,
    setPortalMode,
  } = useAuth();

  const [menuOpen, setMenuOpen] =
    useState(false);

  const isAdmin =
    user?.role === "Admin" &&
    portalMode === "admin";

  const isLoginPage =
    location.pathname === "/";

  if (isLoginPage) {
    return (
      <Routes>
        <Route
          path="/"
          element={<Login />}
        />
      </Routes>
    );
  }

  return (
    <div className="min-h-screen bg-slate-100">
      {menuOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 lg:hidden"
          onClick={() =>
            setMenuOpen(false)
          }
        />
      )}

      <div className="flex">
        <aside
          className={`fixed lg:static z-50 top-0 left-0 h-screen w-72 bg-slate-950 text-white p-6 transform transition-transform duration-300 ${
            menuOpen
              ? "translate-x-0"
              : "-translate-x-full lg:translate-x-0"
          }`}
        >
          <div className="mb-10">
            <h1 className="text-3xl font-bold">
              FMRT
            </h1>

            <p className="text-slate-400 text-sm mt-1">
              Attendance Portal
            </p>
          </div>

          <nav className="space-y-3">
            <MenuItem
              to="/dashboard"
              onClick={() =>
                setMenuOpen(false)
              }
            >
              Dashboard
            </MenuItem>

            <MenuItem
              to="/attendance"
              onClick={() =>
                setMenuOpen(false)
              }
            >
              Attendance
            </MenuItem>

            <MenuItem
              to="/calendar"
              onClick={() =>
                setMenuOpen(false)
              }
            >
              Calendar
            </MenuItem>

            <MenuItem
              to="/summary"
              onClick={() =>
                setMenuOpen(false)
              }
            >
              Reports
            </MenuItem>

            <MenuItem
              to="/profile"
              onClick={() =>
                setMenuOpen(false)
              }
            >
              Profile
            </MenuItem>

            <MenuItem
              to="/leave"
              onClick={() =>
                setMenuOpen(false)
              }
            >
              Leave
            </MenuItem>

            {isAdmin && (
              <MenuItem
                to="/holidays"
                onClick={() =>
                  setMenuOpen(false)
                }
              >
                Holidays
              </MenuItem>
            )}

            {isAdmin && (
              <MenuItem
                to="/process"
                onClick={() =>
                  setMenuOpen(false)
                }
              >
                Processing
              </MenuItem>
            )}

            {isAdmin && (
              <MenuItem
                to="/admin-leaves"
                onClick={() =>
                  setMenuOpen(false)
                }
              >
                Leave Approval
              </MenuItem>
            )}

            {isAdmin && (
             <MenuItem
             to="/employees"
             onClick={() =>
               setMenuOpen(false)
                }
            >
           Employee Master
            </MenuItem>
            )}

            <button
              onClick={() => {
                logout();
                window.location.href = "/";
              }}
              className="block w-full text-left p-3 rounded-xl text-red-300 hover:bg-red-900"
            >
              Logout
            </button>
          </nav>

          <div className="absolute bottom-6 left-6">
            <div className="text-sm text-slate-400">
              FMRT Attendance
            </div>

            <div className="text-xs text-slate-500">
              v1.0.0
            </div>
          </div>
        </aside>

        <main className="flex-1 min-h-screen">
          <header className="bg-white shadow px-4 md:px-8 py-4 flex flex-wrap items-center justify-between gap-4">
            <div className="flex items-center gap-4">
              <button
                className="lg:hidden text-2xl"
                onClick={() =>
                  setMenuOpen(!menuOpen)
                }
              >
                ☰
              </button>

              <h2 className="font-semibold text-lg md:text-xl">
                FMRT Attendance Portal
              </h2>
            </div>

<div className="flex flex-wrap items-center gap-3">

  {user?.role === "Admin" && (
    <select
      value={portalMode}
      onChange={(e) =>
        setPortalMode(
          e.target.value
        )
      }
      className="
        border
        rounded-xl
        px-3
        py-2
        bg-white
      "
    >
      <option value="employee">
        Employee Portal
      </option>

      <option value="admin">
        Admin Portal
      </option>
    </select>
  )}

  {user?.role === "Admin" &&
   portalMode === "admin" && (
    <EmployeeSelector />
  )}

  <div className="
    w-10
    h-10
    rounded-full
    bg-blue-600
    text-white
    flex
    items-center
    justify-center
    font-semibold
  ">
                {user?.name?.charAt(0) ||
                  "U"}
              </div>

              <div>
                <p className="font-semibold">
                  {user?.name || "User"}
                </p>

                <p className="text-xs text-slate-500">
                  {user?.role ??
                    "Employee"}
                  {user?.role ===
                    "Admin" &&
                    ` (${portalMode})`}
                </p>
              </div>
            </div>
          </header>

          <div className="p-4 md:p-6 lg:p-8">
            <Routes>
              <Route
                path="/dashboard"
                element={<Dashboard />}
              />

              <Route
                path="/attendance"
                element={
                  <AttendanceHistory />
                }
              />

              <Route
                path="/calendar"
                element={<Calendar />}
              />

              <Route
                path="/summary"
                element={<Reports />}
              />

              <Route
                path="/profile"
                element={<Profile />}
              />

              <Route
                path="/leave"
                element={
                  <LeaveManagement />
                }
              />

              {isAdmin && (
                <Route
                  path="/holidays"
                  element={
                    <HolidayManagement />
                  }
                />
              )}

              {isAdmin && (
                <Route
                  path="/process"
                  element={
                    <AttendanceProcessing />
                  }
                />
              )}

              {isAdmin && (
                <Route
                  path="/admin-leaves"
                  element={
                    <AdminLeaves />
                  }
                />
              )}

              {isAdmin && (
                <Route
                path="/employees"
                element={
                 <EmployeeManagement />
                   }
                  />
                  )}
            </Routes>
          </div>
        </main>
      </div>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AppLayout />
    </BrowserRouter>
  );
}