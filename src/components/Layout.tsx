import { useEffect, useMemo } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { BookOpen, ChevronRight, ClipboardList, GraduationCap, LayoutDashboard, LogOut, Menu, Moon, Settings, Sun, Users, X } from "lucide-react";
import { useAppDispatch, useAppSelector } from "../hooks";
import { clearToast, closeSidebar, toggleSidebar, toggleTheme } from "../features/ui/uiSlice";
import { logout } from "../features/auth/authSlice";
import { Toast } from "./Common";

const navByRole = {
  Admin: [
    ["/admin", "Dashboard", LayoutDashboard],
    ["/admin/users", "Users", Users],
    ["/admin/classes", "Classes", GraduationCap],
    ["/admin/courses", "Courses", BookOpen],
    ["/admin/subjects", "Subjects", ClipboardList],
    ["/admin/enrollments", "Enrollments", Users],
    ["/admin/teacher-assignments", "Teacher Assignments", Users],
    ["/admin/assignments", "Assignments", ClipboardList],
    ["/admin/settings", "Settings", Settings]
  ],
  Teacher: [
    ["/teacher", "Dashboard", LayoutDashboard],
    ["/teacher/assignments", "My Assignments", ClipboardList]
  ],
  Student: [
    ["/student", "Dashboard", LayoutDashboard],
    ["/student/assignments", "Assignments", ClipboardList],
    ["/student/submissions", "My Submissions", BookOpen]
  ]
} as const;

export default function Layout() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const user = useAppSelector(s => s.auth.user);
  const { sidebarOpen, darkMode, toast } = useAppSelector(s => s.ui);
  const items = useMemo(() => navByRole[user?.role ?? "Student"], [user?.role]);

  useEffect(() => {
    document.documentElement.classList.toggle("dark", darkMode);
  }, [darkMode]);

  const signOut = async () => {
    await dispatch(logout());
    navigate("/login");
  };

  return <div className="app-shell">
    <aside className={`sidebar ${sidebarOpen ? "open" : ""}`}>
      <div className="brand"><div className="brand-mark">EA</div><div><strong>EduAssign</strong><small>Academic workspace</small></div><button className="mobile-only icon-btn" onClick={() => dispatch(closeSidebar())}><X/></button></div>
      <nav>{items.map(([path, label, Icon]) => <NavLink key={path} to={path} end={path.split("/").length === 2} onClick={() => dispatch(closeSidebar())}><Icon size={18}/><span>{label}</span><ChevronRight size={15} className="nav-arrow"/></NavLink>)}</nav>
      <div className="sidebar-bottom">
        <button onClick={() => dispatch(toggleTheme())}>{darkMode ? <Sun size={18}/> : <Moon size={18}/>}<span>{darkMode ? "Light mode" : "Dark mode"}</span></button>
        <button onClick={signOut}><LogOut size={18}/><span>Sign out</span></button>
      </div>
    </aside>
    {sidebarOpen && <div className="mobile-overlay" onClick={() => dispatch(closeSidebar())}/>}
    <main className="main">
      <header className="topbar"><button className="mobile-only icon-btn" onClick={() => dispatch(toggleSidebar())}><Menu/></button><div className="topbar-title"><span>{user?.role} workspace</span><strong>Welcome back, {user?.firstName}</strong></div><div className="profile-mini"><div className="avatar">{user?.firstName?.[0]}{user?.lastName?.[0]}</div><div className="profile-text"><b>{user?.firstName} {user?.lastName}</b><small>{user?.email}</small></div></div></header>
      <section className="content"><Outlet/></section>
    </main>
    {toast && <Toast type={toast.type} message={toast.message} onClose={() => dispatch(clearToast())}/>}
  </div>;
}
