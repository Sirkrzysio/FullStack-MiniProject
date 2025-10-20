# 📝 Angular + .NET 8 Todo App with Authentication (JWT)

Full-stack CRUD application built with **Angular 20** (standalone components) and **.NET 8 Web API**, featuring:
- ✅ User **registration** & **login** with JWT Authentication
- 🗂️ Full **CRUD** for Todo items
- 🗂️ Admin panel
- 🌐 **SQLite** as the database (lightweight & easy to set up)
- 🧠 Clean service structure and interceptors
- 💅 Responsive UI with modern CSS
- 🐳 Dockerized frontend & backend for easy setup
---

## 🚀 Tech Stack

**Frontend**
- Angular 20
- Standalone Components
- HttpClient, Router, FormsModule
- Custom Pipes & Interceptors

**Backend**
- .NET 8 Web API
- Entity Framework Core (SQLite)
- JWT Authentication
- Swagger UI for testing endpoints

**Database**
- SQLite — no setup required, auto-creates DB file.

**Containerization**

- Docker + Docker Compose for full-stack setup

---

## ⚡ Features

- 👤 **User Auth**
    - Register & Login with JWT
    - Role support (User / Admin)
    - Auth Guard via interceptor

- ✅ **Todo CRUD**
    - Add / Edit / Delete / Toggle completed
    - Stored in SQLite through .NET API
    - Live updates in Angular frontend

- 🌐 **Modern Angular UI**
    - Header with dynamic Login/Register/Logout buttons
    - Styled forms and todo list
    - Clean responsive layout

---

## 🛠️ Installation & Setup

### 1️⃣ Clone the repository
```bash
git clone https://github.com/Sirkrzysio/FullStack-MiniProject
cd FullStack-MiniProject

cd backend
dotnet restore
dotnet ef database update   
dotnet run

Backend domyślnie działa pod:
👉 http://localhost:5187

Swagger:
👉 http://localhost:5187/swagger

cd frontend
npm install
ng serve

Frontend działa pod:
👉 http://localhost:4200

🔐 Authentication Flow

📝 Rejestracja przez /api/auth/register

🔑 Logowanie przez /api/auth/login → zwraca JWT

🧠 Token zapisany w localStorage

🌐 Wszystkie kolejne zapytania wysyłają token przez interceptor

```
### 🐳 Installation & Setup (Docker)
### Clone the repository
```
git clone https://github.com/Sirkrzysio/FullStack-MiniProject
cd FullStack-MiniProject

 Start full stack with Docker Compose
docker-compose up --build -d


This will build both frontend and backend images from Dockerfile and start containers.

Frontend → http://localhost:4200

Backend → http://localhost:5187

Swagger UI → http://localhost:5187/swagger
```