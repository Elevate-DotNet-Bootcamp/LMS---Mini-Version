# 📊 LMS — Business Workflow Report

> Architecture overview: Layered Architecture with Repository, Unit of Work, Services, and Mediators.

---

## 🎯 Complex Actions (The Mediators)

Mediators are **Action Coordinators** — they orchestrate multi-step business workflows across multiple Services, then commit all changes atomically via `IUnitOfWork.CompleteAsync()`.

---

### 1. `EnrollInternMediator`
> **Endpoint:** `POST /api/enrollment`

Orchestrates the full enrollment of an intern into a track.

| Step | Service Call | Description |
|------|-------------|-------------|
| 1 | `IInternService.GetByIdAsync()` | Validate intern exists |
| 2 | `ITrackService.GetByIdAsync()` | Validate track exists and is active |
| 3 | `ITrackService.CheckCapacityAsync()` | Ensure track has available capacity |
| 4 | `IEnrollmentService.CreateEnrollmentAsync()` | Stage new enrollment (Change Tracker) |
| 5 | `IPaymentService.CreatePaymentAsync()` | Stage payment if track has fees > 0 |
| 6 | `IUnitOfWork.CompleteAsync()` | **Atomic commit** — all or nothing |

---

### 2. `CancelEnrollmentMediator`
> **Endpoint:** `POST /api/enrollment/{id}/cancel`

Orchestrates enrollment cancellation with an automatic payment refund.

| Step | Service Call | Description |
|------|-------------|-------------|
| 1 | `IEnrollmentService.GetByIdAsync()` | Fetch and validate enrollment |
| 2 | `IEnrollmentService.UpdateStatusAsync()` | Mark status as `Cancelled` (staged) |
| 3 | `IPaymentService.RefundPaymentAsync()` | Mark payment as `Refunded` (staged) |
| 4 | `IUnitOfWork.CompleteAsync()` | **Atomic commit** — cancel + refund together |

---

### 3. `TransferEnrollmentMediator`
> **Endpoint:** `POST /api/enrollment/{id}/transfer/{newTrackId}`

Orchestrates transferring an enrollment from one track to another with fee adjustment.

| Step | Service Call | Description |
|------|-------------|-------------|
| 1 | `IEnrollmentService.GetByIdAsync()` | Fetch existing enrollment |
| 2 | `ITrackService.GetByIdAsync()` | Validate new track exists and is active |
| 3 | `ITrackService.CheckCapacityAsync()` | Ensure new track has capacity |
| 4 | `IEnrollmentService.UpdateTrackAsync()` | Move enrollment to new track (staged) |
| 5 | `IPaymentService.UpdatePaymentAmountAsync()` | Adjust payment to new track's fees (staged) |
| 6 | `IUnitOfWork.CompleteAsync()` | **Atomic commit** — transfer + fee adjustment together |

---

## ⚙️ Atomic Steps (The Services)

Services handle **single-entity operations** (Steps). They never call `SaveChanges()` — changes are staged in the EF Change Tracker and committed by the caller (Mediator or Controller) via `IUnitOfWork.CompleteAsync()`.

| Service | Primary Responsibilities |
|---------|------------------------|
| `TrackService` | CRUD for tracks, `CheckCapacityAsync()` to verify enrollment slots |
| `InternService` | CRUD for interns with eager-loaded Track data |
| `EnrollmentService` | CRUD for enrollments, `UpdateStatusAsync()`, `UpdateTrackAsync()` |
| `PaymentService` | Create payments, `RefundPaymentAsync()`, `UpdatePaymentAmountAsync()` |

**Key Rules:**
- Services inject `IUnitOfWork` — never `DbContext` directly.
- Services return `DTOs` — never raw Entities.
- Services use `IQueryable` for DB-side filtering — never `IEnumerable` masking deferred execution.
- Services **never inject other services** — that would cause cyclic dependencies.

---

## 🏗️ Architecture Layer Flow

```
Client Request
     │
     ▼
┌─────────────┐   ViewModels    ┌─────────────────┐
│  Controller  │ ──────────────► │    Mediator      │ (multi-step actions)
│             │                 │ (Coordinator)    │
└──────┬──────┘                 └────────┬─────────┘
       │                                 │
       │  (simple CRUD)                  │  orchestrates
       │                                 │
       ▼                                 ▼
┌─────────────┐   DTOs          ┌─────────────────┐
│   Service    │ ◄──────────────│    Service(s)    │
│  (Steps)    │                 │    (Steps)       │
└──────┬──────┘                 └────────┬─────────┘
       │                                 │
       │  IQueryable                     │
       ▼                                 ▼
┌─────────────────────────────────────────────────┐
│              IUnitOfWork                         │
│  ┌──────────────┐  ┌──────────────────────────┐ │
│  │ Repository<T> │  │ CompleteAsync()           │ │
│  │ (Change Track)│  │ (single SaveChangesAsync)│ │
│  └──────────────┘  └──────────────────────────┘ │
└──────────────────────┬──────────────────────────┘
                       │
                       ▼
                 ┌───────────┐
                 │ Database   │
                 └───────────┘
```

---

## ⚠️ The Current Architectural Flaw: Mediator Explosion

> **Look at the `EnrollmentController` constructor:**

```csharp
public EnrollmentController(
    IEnrollmentService enrollmentService,     // reads
    EnrollInternMediator enrollMediator,       // action 1
    CancelEnrollmentMediator cancelMediator,   // action 2
    TransferEnrollmentMediator transferMediator // action 3
)
```

**The Problem:**
Because we created a "Class per Action" Mediator pattern, **every new business action requires**:
1. A new Mediator class
2. A new DI registration in `Program.cs`
3. A new constructor parameter in the Controller

This violates the **Open/Closed Principle** — adding new functionality forces modifications to existing classes. As the system scales to 10+ actions, the constructor becomes unmaintainable ("Constructor Over-Injection").

**The Real Solution — CQRS with MediatR:**

Replace all manual Mediators with the **Command/Query Responsibility Segregation** pattern using the `MediatR` library:

```csharp
// BEFORE: 4 constructor parameters (and growing)
public EnrollmentController(
    IEnrollmentService enrollmentService,
    EnrollInternMediator enrollMediator,
    CancelEnrollmentMediator cancelMediator,
    TransferEnrollmentMediator transferMediator)

// AFTER: 1 constructor parameter (forever)
public EnrollmentController(IMediator mediator)
```

Each action becomes a self-contained **Command** (e.g., `EnrollInternCommand`, `CancelEnrollmentCommand`) with its own **Handler**. The `IMediator` dispatches commands to the correct handler automatically — the Controller never needs to know which handler handles what.

---

## 📁 Final Project Structure

```
LMS - Mini Version/
├── Domain/
│   ├── Entities/          Track, Intern, Enrollment, Payment
│   ├── Enums/             EnrollmentStatus, PaymentMethod, PaymentStatus
│   └── Repositories/      IGeneralRepository<T>, IUnitOfWork
├── Infrastructure/
│   ├── Persistence/       AppDbContext, DbInitializer
│   └── Repositories/      GeneralRepository<T>, UnitOfWork
├── DTOs/                  TrackDto, InternDto, EnrollmentDto, PaymentDto, CreateEnrollmentDto
├── ViewModels/
│   ├── Track/             Summary, Detail, Create, Update
│   ├── Intern/            Summary, Detail, Create, Update
│   ├── Enrollment/        EnrollmentViewModel, EnrollInternViewModel
│   └── Payment/           PaymentViewModel
├── Mapping/               MappingExtensions (Entity ↔ DTO ↔ ViewModel)
├── Services/
│   ├── Interfaces/        ITrackService, IInternService, IEnrollmentService, IPaymentService
│   └── Implementations/   TrackService, InternService, EnrollmentService, PaymentService
├── Mediators/             EnrollInternMediator, CancelEnrollmentMediator, TransferEnrollmentMediator
│                          MediatorResult, EnrollmentResultDto
├── Controllers/           TrackController, InternController, EnrollmentController, PaymentController
└── Program.cs
```

---

## 🧩 Traps Summary

| # | Trap Name | Status | Fix Applied |
|---|-----------|--------|-------------|
| 1 | DbContext Leak & Tight Coupling | ✅ Fixed | Repository Pattern + UoW |
| 2 | Exposing Domain Models | ✅ Fixed | DTOs + ViewModels + Manual Mapping |
| 3 | Synchronous Blocking | ✅ Fixed | Full `async/await` with `ConfigureAwait(false)` |
| 4 | IQueryable Memory Leak | ✅ Fixed | `IQueryable` in Repos → `ToListAsync()` in Services |
| 5 | Business Logic Leakage | ✅ Fixed | Services (Steps) + Mediators (Actions) |
| 6 | SaveChanges Transaction Dilemma | ✅ Fixed | Unit of Work `CompleteAsync()` |
| 7 | **Mediator Explosion** | ⚠️ **Intentional** | Paves the way for CQRS / MediatR |
