# LMS - Mini Version

##  Overview

The **LMS - Mini Version** is an educational .NET 8 Web API project designed for mentorship and bootcamp environments. It serves as a practical playground for students to explore **RESTful APIs**, **Entity Framework Core**, and common architectural patterns.

Unlike "perfect" production code, this repository is intentionally built with "legacy-style" practices—such as synchronous blocking calls and tight coupling—to provide a hands-on refactoring experience for junior developers.

---

##  Tech Stack

* **Framework:** .NET 8.0.
* **Database:** SQL Server via Entity Framework Core.
* **Architecture:** Monolithic (Folder-based separation).
* **Documentation:** Swagger/OpenAPI.

---

##  Project Structure

Following the structure of the original "CarRentalApp" reference, this project is organized into clear domains:

* **Domain/Entities:** Contains core models like `Track` and `Intern`.
* **Persistence:** Handles database context (`AppDbContext`) and data seeding.
* **Infrastructure/Repositories:** Implements a generic repository (currently with intentional design flaws for learning).
* **Controllers:** Traditional API controllers using direct dependency injection.

---

## 📝 Key Features (Learning Modules)

1. **Track Management:** Manage training paths (e.g., .NET, Cyber Security, Angular).
2. **Intern Management:** Handle student enrollments and status tracking.
3. **Data Seeding:** Includes a `DbInitializer` with pre-configured data to test filtering and business logic.

---

## ⚠️ Educational Note: Intentional Flaws

This project contains several "Bad Practices" by design to facilitate mentor-led discussions:

* **Synchronous Methods:** All DB operations are currently synchronous (`ToList`, `Find`, `SaveChanges`) to demonstrate thread-pool starvation.
* **Direct Context Access:** Controllers originally communicated directly with the `DbContext`.
* **Generic Repository Issues:** The repository currently returns `IQueryable`, leaking database logic into the API layer.
* **Lack of DTOs:** The API exposes raw database entities, showcasing security and over-posting risks.

---

## ⚙️ Getting Started

1. **Clone the repository.**
2. **Configure Database:** Update the connection string in `appsettings.json`.
3. **Run Migrations:**
```powershell
Add-Migration InitialCreate
Update-Database

```


4. **Run Project:** Press `F5` to launch the API and explore via Swagger.

---

## 👨‍🏫 Mentorship

This project is maintained by **Amr Mohamed** , **Seif Emam**, . The goal is to evolve this code from a "Dirty" state to a "Clean Architecture" state throughout the bootcamp.
