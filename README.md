# 🎓 Assignment-Submission-Management-System

A modern, role-based **Assignment & Submission Management System** designed for schools, colleges, and educational institutions.

The system allows administrators to manage users, classes, courses, subjects, enrollments, and teacher assignments. Teachers can create and manage assignments, while students can view assignments, submit their work, and receive marks and feedback.

The project uses **ASP.NET Core Web API + MongoDB** for the backend and **React + TypeScript + Vite** for the frontend.

---

# 🚀 Live Demo

### 🌐 Frontend

![Admin Dashboard]<img width="2848" height="1538" alt="Admin" src="https://github.com/user-attachments/assets/47223bcb-1bb3-4b3c-ac6f-328bfb6d7d56" />

![Teacher Dashboard]<img width="2854" height="1532" alt="Teacher" src="https://github.com/user-attachments/assets/395c8622-e630-43fb-ad97-244baab72674" />

![Student Dashboard]<img width="2858" height="1542" alt="Student" src="https://github.com/user-attachments/assets/f89ff173-6ad5-4b38-b128-f75c39912949" />

" />

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

✨ Main Features
🔐 Authentication & Authorization
User registration
User login
JWT authentication
Refresh token support
Role-based authorization
Protected API endpoints
Admin / Teacher / Student roles
Active/inactive user management
Secure password hashing
Token expiration and revocation

👨‍💼 Admin Features

Administrators have complete control over the academic system.

User Management
Create users
Update users
Delete users
Search users
View users
Change user roles
Activate/deactivate users
Manage teachers
Manage students

👨‍🏫 Teacher Features

Teachers can manage assignments associated with their assigned academic areas.

Assignment Management

Teachers can:

Create assignments
Edit assignments
View assignments
Publish assignments
Close assignments
Delete assignments
Set deadlines
Set maximum marks
Add descriptions
Upload attachments
Allow/disallow updates before deadline

👨‍🎓 Student Features

Students can:

View enrolled classes/courses
View available assignments
View assignment details
Submit assignments
Update submissions before deadline when allowed
Upload submission attachments
View submission status
View marks
View teacher feedback

📊 Dashboard

The system provides role-specific dashboards.

Admin Dashboard

Displays information such as:

Total users
Total teachers
Total students
Total classes
Total courses
Total subjects
Total assignments
Total submissions
Teacher Dashboard

Provides teacher-specific:

Assignment statistics
Submission statistics
Grading information
Academic assignments
Student Dashboard

Provides student-specific:

Available assignments
Submitted assignments
Pending assignments
Grades
Feedback

📎 File Upload

The system supports file uploads for:

Assignment attachments
Student submission attachments


🛡️ Validation

The backend validates:

Required fields
Email addresses
Maximum/minimum lengths
Decimal mark ranges
User roles
Academic relationships
Duplicate enrollments
Duplicate teacher assignments
Assignment ownership
Student enrollment
Course/class relationships
Subject/course relationships

🗄️ Database
MongoDB

📁 Project Structure
AssignmentManagementSystem/
│
├── AssignmentManagementSystem.API/
│   │
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── UsersController.cs
│   │   ├── ClassesController.cs
│   │   ├── CoursesController.cs
│   │   ├── SubjectsController.cs
│   │   ├── EnrollmentsController.cs
│   │   ├── TeacherAssignmentsController.cs
│   │   ├── AssignmentsController.cs
│   │   ├── SubmissionsController.cs
│   │   ├── DashboardController.cs
│   │   ├── SettingsController.cs
│   │   └── UploadsController.cs
│   │
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── AssignmentManagementSystem.Core/
│   │
│   ├── DTOs/
│   │   └── Requests & Responses
│   │
│   ├── Models/
│   │   ├── MongoEntity.cs
│   │   ├── User.cs
│   │   ├── ClassRoom.cs
│   │   ├── Course.cs
│   │   ├── Subject.cs
│   │   ├── StudentEnrollment.cs
│   │   ├── TeacherAssignment.cs
│   │   ├── Assignment.cs
│   │   └── Submission.cs
│   │
│   └── Interfaces/
│       ├── IRepository.cs
│       └── Service Interfaces
│
├── AssignmentManagementSystem.Infrastructure/
│   │
│   ├── Data/
│   │   ├── MongoContext.cs
│   │   └── MongoIndexes.cs
│   │
│   ├── Repositories/
│   │   └── MongoRepository.cs
│   │
│   └── Seed/
│       └── DatabaseSeeder.cs
│
├── assignment-management-frontend/
│   │
│   ├── src/
│   │   ├── components/
│   │   ├── features/
│   │   ├── hooks/
│   │   ├── layouts/
│   │   ├── lib/
│   │   ├── pages/
│   │   ├── services.ts
│   │   ├── types.ts
│   │   └── main.tsx
│   │
│   ├── public/
│   ├── package.json
│   ├── tsconfig.json
│   └── vite.config.ts
│
└── README.md

⚠️ Assumptions
The project currently assumes:

MongoDB is available and accessible from the backend.
Users authenticate using JWT access tokens.
Each user has one primary role.
Classes are created before courses.
Courses are created before subjects.
Students must exist before enrollment.
Teachers must exist before teacher assignment.
A course belongs to one class.
A subject belongs to one course.
A teacher assignment belongs to one teacher, class, course, and subject.
Students should be enrolled before submitting assignments.
Teachers can only manage assignments associated with their authorized academic areas.
The frontend development server runs on port 5173.
The backend development server runs on port 7085.
MongoDB is used as the primary application database.

⭐ Project Status

🟢 Authentication              Completed
🟢 User Management             Completed
🟢 Class Management            Completed
🟢 Course Management           Completed
🟢 Subject Management          Completed
🟢 Student Enrollment          Completed
🟢 Teacher Assignment          Completed
🟢 Assignment Management       Completed
🟢 Submission Management       Completed
🟢 Grading                     Completed
🟢 Dashboard                   Completed
🟢 JWT Authentication          Completed
🟢 MongoDB Integration         Completed
🟢 Swagger API Documentation   Completed
🟢 Role-Based Authorization     Completed

🎯 Future Improvements

Potential future features:

📧 Email notifications
🔔 Real-time notifications
📱 Mobile application
📊 Advanced analytics
📄 PDF report generation
📊 Excel export
📅 Assignment calendar
🔔 Deadline reminders
💬 Teacher/student messaging
📝 Rich-text assignment editor
☁️ Cloud file storage
🔎 Advanced search
🌍 Multi-language support
🧪 Expanded automated test coverage
