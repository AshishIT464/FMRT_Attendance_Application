import {
  createContext,
  useContext,
  useEffect,
  useState,
} from "react";

import api from "../api/attendanceApi";
import { useAuth } from "./AuthContext";

const EmployeeContext = createContext();

export function EmployeeProvider({ children }) {
  const { user } = useAuth();

  const [employeeId, setEmployeeId] = useState(null);
  const [employee, setEmployee] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user?.email) {
      setEmployee(null);
      setEmployeeId(null);
      setLoading(false);
      return;
    }

    loadEmployee(user.email);
  }, [user]);

  async function loadEmployee(email) {
    try {
      setLoading(true);

      const response = await api.get(
        "/Employees/by-email",
        {
          params: { email },
        }
      );

      console.log(
        "Employee Mapping Response:",
        response.data
      );

      const employeeData = response.data;

      setEmployee(employeeData);
      setEmployeeId(employeeData?.employeeId ?? null);
    } catch (error) {
      console.error(
        "Employee mapping failed:",
        error
      );

      setEmployee(null);
      setEmployeeId(null);
    } finally {
      setLoading(false);
    }
  }

  return (
    <EmployeeContext.Provider
      value={{
            employeeId,
            setEmployeeId,
            employee,
            setEmployee,
            loading,
            userEmail: user?.email
            }}
    >
      {children}
    </EmployeeContext.Provider>
  );
}

export function useEmployee() {
  return useContext(EmployeeContext);
}