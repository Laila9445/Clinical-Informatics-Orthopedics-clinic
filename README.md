# Clinical Informatics – Orthopedics Clinic
A full-stack clinic management system for an orthopedics practice. It was designed with orthopedic specialist. Patients can book and manage appointments, upload medical images and receive prescriptions. Doctors run consultations, issue prescriptions, manage their schedule and send referrals.
The repo has two parts:
| Folder | Description |
|--------|-------------|
| [`backend/`](backend) | ASP.NET Core 8 Web API (JWT auth, EF Core, Swagger) |
| [`frontend/`](frontend) | React Native / Expo app (iOS, Android, web) written in TypeScript |
## Features
### Patients
- Register, log in and view their profile
- Search doctors' available slots, then book, reschedule and cancel appointments
- View appointments and consultation history
- Upload, list, download and delete medical images (X-rays and similar)
- View prescriptions and download them as PDF
### Doctors
- Dashboard with today's appointments and all appointments
- Search patients and view patient records and images
- Manage the weekly schedule (create and delete schedule entries)
- Start, update and end consultations with notes
- Create single or bulk prescriptions, generate a PDF and send it
- Create referrals (including to external patients), track referral status and send them
- Send appointment reminder notifications
### Platform
- JWT authentication with role-based authorization (`Patient`, `Doctor`)
- In-app notifications, email (MailKit) and SMS service abstractions
- FluentValidation request validation and AutoMapper DTO mapping
- Global exception handling middleware and Serilog file logging
- Rate limiting and CORS configuration
- Repository and Unit of Work pattern with EF Core migrations
## Tech stack
**Backend:** .NET 8, ASP.NET Core Web API, Entity Framework Core 8 (SQLite and SQL Server providers), ASP.NET Identity, JWT Bearer, AutoMapper, FluentValidation, QuestPDF, MailKit, Serilog, Swagger (Swashbuckle).
**Frontend:** Expo 54, React Native 0.81, React 19, TypeScript, Expo Router, React Navigation, Axios, react-native-calendars, react-native-toast-message.
## Project structure
```
.
├── backend/
│   ├── Controllers/     # Auth, Appointments, Consultations, Doctors, MedicalImages,
│   │                    # Notifications, Prescriptions, Referrals
│   ├── Services/        # Business logic (+ interfaces), PDF, email, SMS, file storage
│   ├── Repositories/    # Generic repository and Unit of Work
│   ├── Models/          # Entities
│   ├── Data/            # ClinicDbContext and DTOs
│   ├── Validators/      # FluentValidation validators
│   ├── Enums/           # UserRole, AppointmentStatus, ConsultationStatus, PrescriptionStatus, ...
│   ├── Middleware/      # Global exception handler
│   ├── Migrations/      # EF Core migrations
│   ├── Program.cs
│   └── *.md             # SETUP, ARCHITECTURE, API_TESTING, DEPLOYMENT, ...
├── frontend/
│   ├── app/             # Screens (login, signup, tabs)
│   ├── components/ hooks/ constants/
│   ├── context/         # AuthContext
│   ├── images/          # Use-case, class, sequence and ER diagrams
│   └── package.json
├── .github/workflows/   # CI workflows
└── Software_Project.sln
```
## API overview
All routes are under `/api` and need a Bearer token unless noted.
| Controller | Endpoints | Role |
|-----------|-----------|------|
| Auth | `POST register`, `POST login` (public), `GET profile` | Any |
| Appointments | `GET available-slots`, `POST book`, `PUT reschedule`, `PUT cancel`, `GET my-appointments`, `GET {id}` | Patient (`{id}`: any) |
| Doctors | `GET` (list), `GET/PUT profile`, `GET appointments`, `GET appointments/today`, `GET patients/search`, `GET patients/{id}`, `GET patients/{id}/images`, `GET/POST schedule`, `DELETE schedule/{id}` | Doctor (list: Patient) |
| Consultations | `POST start`, `PUT {id}`, `PUT {id}/end`, `GET {id}`, `GET patient/history` | Doctor (history: Patient) |
| Prescriptions | `POST`, `POST bulk`, `POST {id}/send`, `GET my-prescriptions`, `GET {id}`, `GET {id}/pdf` | Doctor / Patient |
| Medical images | `POST upload`, `GET`, `GET {id}/download`, `DELETE {id}` | Patient |
| Referrals | `POST`, `GET doctor/{id}`, `GET patient/{externalId}`, `GET {id}`, `PUT {id}/status`, `POST {id}/send` | Doctor |
| Notifications | `GET`, `PUT {id}/read`, `DELETE {id}`, `POST appointment-reminder/{appointmentId}` | Any (reminder: Doctor) |
### Status values
- **Appointment:** Scheduled, Confirmed, CheckedIn, InProgress, Completed, Cancelled, NoShow, Rescheduled
- **Consultation:** InProgress, Completed, FollowUpRequired, Cancelled
- **Prescription:** Pending, Processing, ReadyForPickup, Dispensed, Cancelled, Expired
