import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Login() {

  const navigate =
    useNavigate();

  const {
    user,
    login,
    loading
  } = useAuth();

  useEffect(() => {

    if (user) {

      navigate(
        "/dashboard"
      );

    }

  }, [user, navigate]);

  if (loading) {

    return (
      <div className="
        min-h-screen
        flex
        items-center
        justify-center
        bg-slate-100
      ">
        <div className="
          bg-white
          rounded-3xl
          shadow-xl
          p-10
        ">
          Loading...
        </div>
      </div>
    );

  }

  return (

    <div className="
      min-h-screen
      flex
      items-center
      justify-center
      bg-slate-100
      px-4
    ">

      <div className="
        bg-white
        rounded-3xl
        shadow-xl
        p-10
        max-w-md
        w-full
      ">

        <h1 className="
          text-4xl
          font-bold
          text-center
          mb-3
        ">
          IITK - FMRT Attendance
        </h1>

        <p className="
          text-center
          text-slate-500
          mb-10
        ">
          Employee Attendance Portal
        </p>

        <div className="
          text-center
          mb-8
        ">
          <div className="
            text-6xl
          ">
            🏢
          </div>
        </div>

        <button
          onClick={login}
          className="
            w-full
            bg-blue-600
            text-white
            py-4
            rounded-xl
            text-lg
            font-semibold

            hover:bg-blue-700
            transition
          "
        >
          Sign in with Microsoft
        </button>

        <p className="
          text-center
          mt-6
          text-sm
          text-slate-400
        ">
          Microsoft 365 Single Sign-On
        </p>

      </div>

    </div>

  );
}