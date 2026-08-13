import { api } from "./lib/api";
import type {
  ApiResponse, PagedResult, User, ClassRoom, Course, Subject, Enrollment,
  TeacherAssignment, Assignment, Submission, DashboardData, Setting
} from "./types";

const get = async <T>(url: string, params?: object) => (await api.get<ApiResponse<T>>(url, { params })).data.data;
const post = async <T>(url: string, body: object) => (await api.post<ApiResponse<T>>(url, body)).data.data;
const put = async <T>(url: string, body: object) => (await api.put<ApiResponse<T>>(url, body)).data.data;
const patch = async <T>(url: string, body?: object) => (await api.patch<ApiResponse<T>>(url, body)).data.data;
const del = async (url: string) => { await api.delete(url); };

export const services = {
  users: {
    list: (params?: object) => get<PagedResult<User>>("/users", params),
    get: (id: string) => get<User>(`/users/${id}`),
    create: (b: object) => post<User>("/users", b),
    update: (id: string, b: object) => put<User>(`/users/${id}`, b),
    remove: (id: string) => del(`/users/${id}`)
  },
  classes: {
    list: (params?: object) => get<PagedResult<ClassRoom>>("/classes", params),
    create: (b: object) => post<ClassRoom>("/classes", b),
    update: (id: string, b: object) => put<ClassRoom>(`/classes/${id}`, b),
    remove: (id: string) => del(`/classes/${id}`)
  },
  courses: {
    list: (params?: object) => get<PagedResult<Course>>("/courses", params),
    create: (b: object) => post<Course>("/courses", b),
    update: (id: string, b: object) => put<Course>(`/courses/${id}`, b),
    remove: (id: string) => del(`/courses/${id}`)
  },
  subjects: {
    list: (params?: object) => get<PagedResult<Subject>>("/subjects", params),
    create: (b: object) => post<Subject>("/subjects", b),
    update: (id: string, b: object) => put<Subject>(`/subjects/${id}`, b),
    remove: (id: string) => del(`/subjects/${id}`)
  },
  enrollments: {
    list: (params?: object) => get<Enrollment[]>("/enrollments", params),
    create: (b: object) => post<Enrollment>("/enrollments", b),
    update: (id: string, b: object) => put<Enrollment>(`/enrollments/${id}`, b),
    remove: (id: string) => del(`/enrollments/${id}`)
  },
  teacherAssignments: {
    list: () => get<TeacherAssignment[]>("/teacher-assignments"),
    create: (b: object) => post<TeacherAssignment>("/teacher-assignments", b),
    remove: (id: string) => del(`/teacher-assignments/${id}`)
  },
  assignments: {
    admin: (params?: object) => get<PagedResult<Assignment>>("/assignments", params),
    mine: (params?: object) => get<PagedResult<Assignment>>("/assignments/my", params),
    student: (params?: object) => get<PagedResult<Assignment>>("/assignments/student", params),
    get: (id: string) => get<Assignment>(`/assignments/${id}`),
    create: (b: object) => post<Assignment>("/assignments", b),
    update: (id: string, b: object) => put<Assignment>(`/assignments/${id}`, b),
    publish: (id: string) => patch<void>(`/assignments/${id}/publish`),
    remove: (id: string) => del(`/assignments/${id}`)
  },
  submissions: {
    mine: (params?: object) => get<Submission[]>("/submissions/my", params),
    byAssignment: (id: string, params?: object) => get<PagedResult<Submission>>(`/submissions/assignment/${id}`, params),
    create: (b: object) => post<Submission>("/submissions", b),
    update: (id: string, b: object) => put<Submission>(`/submissions/${id}`, b),
    grade: (id: string, b: object) => patch<Submission>(`/submissions/${id}/grade`, b),
    status: (id: string, status: string) => patch<Submission>(`/submissions/${id}/status`, { status })
  },
  dashboard: {
    admin: () => get<DashboardData>("/dashboard/admin"),
    teacher: () => get<DashboardData>("/dashboard/teacher"),
    student: () => get<DashboardData>("/dashboard/student")
  },
  settings: {
    list: () => get<Setting[]>("/settings"),
    save: (key: string, b: object) => put<Setting>(`/settings/${key}`, b)
  },
  upload: async (kind: "assignment" | "submission", file: File) => {
    const form = new FormData();
    form.append("file", file);
    return (await api.post<ApiResponse<string>>(`/uploads/${kind}`, form, {
      headers: { "Content-Type": "multipart/form-data" }
    })).data.data;
  }
};
