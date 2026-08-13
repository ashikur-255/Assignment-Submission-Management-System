import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAppSelector } from "../hooks";
import type { Role } from "../types";

export default function ProtectedRoute({ roles }: { roles?: Role[] }) {
  const { user, initialized } = useAppSelector(s => s.auth);
  const location = useLocation();
  if (!initialized) return <div className="page-center">Loading...</div>;
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  if (roles && !roles.includes(user.role)) return <Navigate to={`/${user.role.toLowerCase()}`} replace />;
  return <Outlet />;
}
