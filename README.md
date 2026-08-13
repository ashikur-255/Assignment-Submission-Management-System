# 🎓 Assignment-Submission-Management-System

A modern, role-based **Assignment & Submission Management System** designed for schools, colleges, and educational institutions.

The system allows administrators to manage users, classes, courses, subjects, enrollments, and teacher assignments. Teachers can create and manage assignments, while students can view assignments, submit their work, and receive marks and feedback.

The project uses **ASP.NET Core Web API + MongoDB** for the backend and **React + TypeScript + Vite** for the frontend.

---

## 📌 Project Overview

The Assignment Management System provides a centralized platform for managing the complete academic assignment lifecycle.

### Main workflow

```text
Admin
 │
 ├── Manage Users
 │      ├── Admin
 │      ├── Teachers
 │      └── Students
 │
 ├── Manage Classes
 │
 ├── Manage Courses
 │      └── Belongs to Class
 │
 ├── Manage Subjects
 │      └── Belongs to Course
 │
 ├── Manage Student Enrollments
 │
 └── Assign Teachers
        │
        └── Class + Course + Subject
                 │
                 ↓
              Teacher
                 │
                 ↓
            Assignments
                 │
                 ↓
              Students
                 │
                 ↓
             Submissions
                 │
                 ↓
              Grading
                 │
                 ↓
          Marks + Feedback
