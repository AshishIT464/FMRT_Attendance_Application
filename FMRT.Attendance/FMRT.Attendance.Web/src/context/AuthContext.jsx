import {
  createContext,
  useContext,
  useEffect,
  useState,
} from "react";

import {
  PublicClientApplication,
} from "@azure/msal-browser";

import {
  msalConfig,
  loginRequest,
} from "../authConfig";

const msalInstance =
  new PublicClientApplication(
    msalConfig
  );

const AuthContext = createContext();

export function AuthProvider({
  children,
}) {
  const [user, setUser] =
    useState(null);

  const [loading, setLoading] =
    useState(true);

  const [portalMode, setPortalMode] =
    useState("employee");

  useEffect(() => {
    initialize();
  }, []);

  async function initialize() {
    try {
      await msalInstance.initialize();

      const response =
        await msalInstance.handleRedirectPromise();

      if (response?.account) {
        msalInstance.setActiveAccount(
          response.account
        );
      }

      const account =
        msalInstance.getActiveAccount() ||
        msalInstance.getAllAccounts()[0];

      if (account) {
        msalInstance.setActiveAccount(
          account
        );

        const adminEmails = [
          "ashish.gupta@fmrt.iitk.ac.in",
        ];

        setUser({
          name: account.name,
          email: account.username,
          role: adminEmails.includes(
            account.username.toLowerCase()
          )
            ? "Admin"
            : "Employee",
        });

        console.log("Account:", account);
        console.log("Claims:", account.idTokenClaims);
        console.log("Department:", account.idTokenClaims?.department);
        console.log("Job Title:", account.idTokenClaims?.jobTitle);
        console.log("All Claims:", account.idTokenClaims);

        console.log(
          "Logged In User:",
          account.name,
          account.username
        );
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  }

  async function login() {
    try {
      await msalInstance.loginRedirect(
        loginRequest
      );
    } catch (error) {
      console.error(error);
    }
  }

  async function logout() {
    try {
      await msalInstance.logoutPopup();

      setUser(null);

      setPortalMode(
        "employee"
      );
    } catch (error) {
      console.error(error);
    }
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        login,
        logout,
        portalMode,
        setPortalMode,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(
    AuthContext
  );
}