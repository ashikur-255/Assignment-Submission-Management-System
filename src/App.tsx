import { Navigate, Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import ProtectedRoute from "./components/ProtectedRoute";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import Dashboard from "./pages/Dashboard";
import Users from "./pages/admin/Users";
import Academic from "./pages/admin/Academic";
import { Enrollments, TeacherAssignments } from "./pages/admin/Relationships";
import AdminAssignments from "./pages/admin/Assignments";
import Settings from "./pages/admin/Settings";
import TeacherAssignmentsPage from "./pages/teacher/Assignments";
import TeacherSubmissions from "./pages/teacher/Submissions";
import StudentAssignments from "./pages/student/Assignments";
import StudentAssignmentDetail from "./pages/student/AssignmentDetail";
import StudentSubmissions from "./pages/student/Submissions";

export default function App() {
  return <Routes>
    <Route path="/login" element={<Login/>}/>
    <Route path="/register" element={<Register/>}/>
    <Route element={<ProtectedRoute/>}><Route element={<Layout/>}>
      <Route path="/admin" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<Dashboard/>}/></Route>
      <Route path="/admin/users" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<Users/>}/></Route>
      <Route path="/admin/classes" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<Academic kind="classes"/>}/></Route>
      <Route path="/admin/courses" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<Academic kind="courses"/>}/></Route>
      <Route path="/admin/subjects" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<Academic kind="subjects"/>}/></Route>
      <Route path="/admin/enrollments" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<Enrollments/>}/></Route>
      <Route path="/admin/teacher-assignments" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<TeacherAssignments/>}/></Route>
      <Route path="/admin/assignments" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<AdminAssignments/>}/></Route>
      <Route path="/admin/settings" element={<ProtectedRoute roles={["Admin"]}/>}><Route index element={<Settings/>}/></Route>

      <Route path="/teacher" element={<ProtectedRoute roles={["Teacher"]}/>}><Route index element={<Dashboard/>}/></Route>
      <Route path="/teacher/assignments" element={<ProtectedRoute roles={["Teacher"]}/>}><Route index element={<TeacherAssignmentsPage/>}/></Route>
      <Route path="/teacher/assignments/:id/submissions" element={<ProtectedRoute roles={["Teacher"]}/>}><Route index element={<TeacherSubmissions/>}/></Route>

      <Route path="/student" element={<ProtectedRoute roles={["Student"]}/>}><Route index element={<Dashboard/>}/></Route>
      <Route path="/student/assignments" element={<ProtectedRoute roles={["Student"]}/>}><Route index element={<StudentAssignments/>}/></Route>
      <Route path="/student/assignments/:id" element={<ProtectedRoute roles={["Student"]}/>}><Route index element={<StudentAssignmentDetail/>}/></Route>
      <Route path="/student/submissions" element={<ProtectedRoute roles={["Student"]}/>}><Route index element={<StudentSubmissions/>}/></Route>
    </Route></Route>
    <Route path="/" element={<Navigate to="/login" replace/>}/>
    <Route path="*" element={<Navigate to="/login" replace/>}/>
  </Routes>;
}
