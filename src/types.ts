export type Role = "Admin" | "Teacher" | "Student";
export type AssignmentStatus = "Draft" | "Published" | "Closed";
export type SubmissionStatus = "Submitted" | "Late" | "Graded" | "Returned";

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  role: Role;
  isActive: boolean;
  createdAt: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
  accessTokenExpiresAt: string;
  refreshTokenExpiresAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface ClassRoom {
  id: string;
  name: string;
  code: string;
  description: string;
  isActive: boolean;
  createdAt: string;
}

export interface Course {
  id: string;
  name: string;
  code: string;
  description: string;
  classId: string;
  isActive: boolean;
  createdAt: string;
}

export interface Subject {
  id: string;
  name: string;
  code: string;
  description: string;
  courseId: string;
  isActive: boolean;
  createdAt: string;
}

export interface Enrollment {
  id: string;
  studentId: string;
  classId: string;
  courseId: string;
  isActive: boolean;
  createdAt: string;
}

export interface TeacherAssignment {
  id: string;
  teacherId: string;
  classId: string;
  courseId: string;
  subjectId: string;
  createdAt: string;
}

export interface Assignment {
  id: string;
  title: string;
  description: string;
  teacherId: string;
  classId: string;
  courseId: string;
  subjectId: string;
  deadline: string;
  maximumMarks: number;
  allowUpdateBeforeDeadline: boolean;
  attachmentUrl?: string | null;
  status: AssignmentStatus;
  createdAt: string;
  updatedAt: string;
  publishedAt?: string | null;
}

export interface Submission {
  id: string;
  assignmentId: string;
  studentId: string;
  answer: string;
  attachmentUrl?: string | null;
  submittedAt: string;
  status: SubmissionStatus;
  marks?: number | null;
  feedback?: string | null;
  gradedAt?: string | null;
  gradedBy?: string | null;
}

export interface DashboardData {
  [key: string]: number;
}

export interface Setting {
  id: string;
  key: string;
  value: string;
  description: string;
}
