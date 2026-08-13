import { useEffect, useState } from "react";
import { ClipboardCheck, Clock3, FileText, GraduationCap, Users, BookOpen, TrendingUp } from "lucide-react";
import Page from "../components/Page";
import { Loading } from "../components/Common";
import { services } from "../services";
import { apiError } from "../lib/api";
import { useAppSelector } from "../hooks";

const configs = {
  Admin: { title:"Admin dashboard", sub:"A live overview of your academic system.", endpoint: services.dashboard.admin, cards:[["totalUsers","Users",Users],["totalTeachers","Teachers",GraduationCap],["totalStudents","Students",Users],["totalClasses","Classes",BookOpen],["totalCourses","Courses",BookOpen],["totalSubjects","Subjects",ClipboardCheck],["totalAssignments","Assignments",FileText],["totalSubmissions","Submissions",ClipboardCheck]] },
  Teacher: { title:"Teacher dashboard", sub:"Track your assignments and review workload.", endpoint: services.dashboard.teacher, cards:[["totalAssignments","Assignments",FileText],["published","Published",TrendingUp],["drafts","Drafts",FileText],["totalSubmissions","Submissions",ClipboardCheck],["pendingReviews","Pending reviews",Clock3],["graded","Graded",ClipboardCheck]] },
  Student: { title:"Student dashboard", sub:"Stay on top of deadlines and feedback.", endpoint: services.dashboard.student, cards:[["totalAssignments","Available",FileText],["pending","Pending",Clock3],["submitted","Submitted",ClipboardCheck],["graded","Graded",TrendingUp]] }
} as const;

export default function Dashboard() {
  const role=useAppSelector(s=>s.auth.user?.role) ?? "Student"; const c=configs[role]; const [data,setData]=useState<Record<string,number>>({});const[loading,setLoading]=useState(true);const[error,setError]=useState("");
  const load=()=>{setLoading(true);c.endpoint().then(setData).catch(e=>setError(apiError(e))).finally(()=>setLoading(false));}; useEffect(load,[]);
  return <Page title={c.title} subtitle={c.sub} onRefresh={load}>{loading?<Loading/>:error?<div className="alert error">{error}</div>:<div className="stats-grid">{c.cards.map(([key,label,Icon])=><div className="stat-card" key={key}><div className="stat-icon"><Icon size={20}/></div><span>{label}</span><strong>{data[key] ?? 0}</strong><small>Current total</small></div>)}</div>}
  </Page>;
}
