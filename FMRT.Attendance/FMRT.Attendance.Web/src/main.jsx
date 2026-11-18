import { StrictMode } from "react";
import { createRoot } from "react-dom/client";

import "./index.css";

import App from "./App.jsx";

import {
  EmployeeProvider
} from "./context/EmployeeContext";

import {
  AuthProvider
} from "./context/AuthContext";

createRoot(
  document.getElementById(
    "root"
  )
).render(
  <StrictMode>

    <AuthProvider>

      <EmployeeProvider>

        <App />

      </EmployeeProvider>

    </AuthProvider>

  </StrictMode>
);