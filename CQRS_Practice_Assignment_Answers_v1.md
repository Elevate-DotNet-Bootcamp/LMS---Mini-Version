# 🎯 CQRS Practice Assignment V1 (7 Queries) — Answer Key

> **Instructor Note:** This file contains the complete solutions for all 7 Query/Handler migration tasks in the V1 Assignment. You can provide this to interns after the assignment, or use it as a grading rubric.

---

## Task 1: `GetTrackByIdQuery`

**1. Query Record** (`Features/Tracks/Queries/GetTrackByIdQuery.cs`)
```csharp
using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Queries
{
    public record GetTrackByIdQuery(int Id) : IRequest<TrackDto?>;
}
```

**2. Handler** (`Features/Tracks/Handlers/GetTrackByIdQueryHandler.cs`)
```csharp
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.Mapping;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class GetTrackByIdQueryHandler 
        : IRequestHandler<GetTrackByIdQuery, TrackDto?>
    {
        private readonly IGeneralRepository<Track> _trackRepository;

        public GetTrackByIdQueryHandler(IGeneralRepository<Track> trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task<TrackDto?> Handle(GetTrackByIdQuery request, CancellationToken cancellationToken)
        {
            var track = await _trackRepository.GetByIdAsync(request.Id);
            return track?.ToDto();
        }
    }
}
```

**3. Controller Wiring** (`TrackController.cs`)
```csharp
[HttpGet("{id}")]
public async Task<ActionResult> GetById(int id)
{
    var result = await _mediator.Send(new GetTrackByIdQuery(id));
    if (result == null) return NotFound();
    return Ok(result);
}
```

---

## Task 2: `GetAllInternsQuery`

**1. Query Record** (`Features/Interns/Queries/GetAllInternsQuery.cs`)
```csharp
using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.Features.Interns.Queries
{
    public record GetAllInternsQuery : IRequest<IEnumerable<InternDto>>;
}
```

**2. Handler** (`Features/Interns/Handlers/GetAllInternsQueryHandler.cs`)
```csharp
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Features.Interns.Queries;
using LMS___Mini_Version.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Interns.Handlers
{
    public class GetAllInternsQueryHandler 
        : IRequestHandler<GetAllInternsQuery, IEnumerable<InternDto>>
    {
        private readonly IGeneralRepository<Intern> _internRepository;

        public GetAllInternsQueryHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }

        public async Task<IEnumerable<InternDto>> Handle(GetAllInternsQuery request, CancellationToken cancellationToken)
        {
            var interns = await _internRepository.GetTable()
                .Include(i => i.Track)
                .ToListAsync(cancellationToken);
                
            return interns.Select(i => i.ToDto());
        }
    }
}
```

**3. Controller Wiring** (`InternController.cs`)
```csharp
[HttpGet]
public async Task<ActionResult> GetAll()
{
    var result = await _mediator.Send(new GetAllInternsQuery());
    return Ok(result);
}
```

---

## Task 3: `GetInternByIdQuery`

**1. Query Record** (`Features/Interns/Queries/GetInternByIdQuery.cs`)
```csharp
using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.Features.Interns.Queries
{
    public record GetInternByIdQuery(int Id) : IRequest<InternDto?>;
}
```

**2. Handler** (`Features/Interns/Handlers/GetInternByIdQueryHandler.cs`)
```csharp
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Features.Interns.Queries;
using LMS___Mini_Version.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Interns.Handlers
{
    public class GetInternByIdQueryHandler 
        : IRequestHandler<GetInternByIdQuery, InternDto?>
    {
        private readonly IGeneralRepository<Intern> _internRepository;

        public GetInternByIdQueryHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }

        public async Task<InternDto?> Handle(GetInternByIdQuery request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository.GetTable()
                .Include(i => i.Track)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
                
            return intern?.ToDto();
        }
    }
}
```

**3. Controller Wiring** (`InternController.cs`)
```csharp
[HttpGet("{id}")]
public async Task<ActionResult> GetById(int id)
{
    var result = await _mediator.Send(new GetInternByIdQuery(id));
    if (result == null) return NotFound();
    return Ok(result);
}
```

---

## Task 4: `GetActiveTracksQuery`

**1. Query Record** (`Features/Tracks/Queries/GetActiveTracksQuery.cs`)
```csharp
using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Queries
{
    public record GetActiveTracksQuery : IRequest<IEnumerable<TrackDto>>;
}
```

**2. Handler** (`Features/Tracks/Handlers/GetActiveTracksQueryHandler.cs`)
```csharp
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class GetActiveTracksQueryHandler 
        : IRequestHandler<GetActiveTracksQuery, IEnumerable<TrackDto>>
    {
        private readonly IGeneralRepository<Track> _trackRepository;

        public GetActiveTracksQueryHandler(IGeneralRepository<Track> trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task<IEnumerable<TrackDto>> Handle(GetActiveTracksQuery request, CancellationToken cancellationToken)
        {
            var tracks = await _trackRepository.GetTable()
                .Where(t => t.IsActive)
                .Include(t => t.Enrollments)
                .ToListAsync(cancellationToken);
                
            return tracks.Select(t => t.ToDto());
        }
    }
}
```

**3. Controller Wiring** (`TrackController.cs`)
```csharp
[HttpGet("active")]
public async Task<ActionResult> GetActiveTracks()
{
    var result = await _mediator.Send(new GetActiveTracksQuery());
    return Ok(result);
}
```

---

## Task 5: `GetEnrollmentsByInternQuery`

**1. Query Record** (`Features/Enrollments/Queries/GetEnrollmentsByInternQuery.cs`)
```csharp
using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Queries
{
    public record GetEnrollmentsByInternQuery(int InternId) : IRequest<IEnumerable<EnrollmentDto>>;
}
```

**2. Handler** (`Features/Enrollments/Handlers/GetEnrollmentsByInternQueryHandler.cs`)
```csharp
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Features.Enrollments.Queries;
using LMS___Mini_Version.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    public class GetEnrollmentsByInternQueryHandler 
        : IRequestHandler<GetEnrollmentsByInternQuery, IEnumerable<EnrollmentDto>>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public GetEnrollmentsByInternQueryHandler(IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<EnrollmentDto>> Handle(GetEnrollmentsByInternQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository.GetTable()
                .Include(e => e.Track)
                .Include(e => e.Intern)
                .Where(e => e.InternId == request.InternId)
                .ToListAsync(cancellationToken);
                
            return enrollments.Select(e => e.ToDto());
        }
    }
}
```

**3. Controller Wiring** (`EnrollmentController.cs`)
```csharp
[HttpGet("intern/{internId}")]
public async Task<ActionResult> GetByIntern(int internId)
{
    var result = await _mediator.Send(new GetEnrollmentsByInternQuery(internId));
    return Ok(result);
}
```

---

## Task 6: `GetPaymentByIdQuery`

**1. Query Record** (`Features/Payments/Queries/GetPaymentByIdQuery.cs`)
```csharp
using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.Features.Payments.Queries
{
    public record GetPaymentByIdQuery(int Id) : IRequest<PaymentDto?>;
}
```

**2. Handler** (`Features/Payments/Handlers/GetPaymentByIdQueryHandler.cs`)
```csharp
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Features.Payments.Queries;
using LMS___Mini_Version.Mapping;
using MediatR;

namespace LMS___Mini_Version.Features.Payments.Handlers
{
    public class GetPaymentByIdQueryHandler 
        : IRequestHandler<GetPaymentByIdQuery, PaymentDto?>
    {
        private readonly IGeneralRepository<Payment> _paymentRepository;

        public GetPaymentByIdQueryHandler(IGeneralRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentDto?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            return payment?.ToDto();
        }
    }
}
```

**3. Controller Wiring** (`PaymentController.cs`)
```csharp
[HttpGet("{id}")]
public async Task<ActionResult> GetById(int id)
{
    var result = await _mediator.Send(new GetPaymentByIdQuery(id));
    if (result == null) return NotFound();
    return Ok(result);
}
```

---

## Task 7: `GetPendingPaymentsQuery`

**1. Query Record** (`Features/Payments/Queries/GetPendingPaymentsQuery.cs`)
```csharp
using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.Features.Payments.Queries
{
    public record GetPendingPaymentsQuery : IRequest<IEnumerable<PaymentDto>>;
}
```

**2. Handler** (`Features/Payments/Handlers/GetPendingPaymentsQueryHandler.cs`)
```csharp
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Features.Payments.Queries;
using LMS___Mini_Version.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Payments.Handlers
{
    public class GetPendingPaymentsQueryHandler 
        : IRequestHandler<GetPendingPaymentsQuery, IEnumerable<PaymentDto>>
    {
        private readonly IGeneralRepository<Payment> _paymentRepository;

        public GetPendingPaymentsQueryHandler(IGeneralRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PaymentDto>> Handle(GetPendingPaymentsQuery request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetTable()
                .Where(p => p.Status == PaymentStatus.Pending)
                .Include(p => p.Enrollment)
                .ToListAsync(cancellationToken);
                
            return payments.Select(p => p.ToDto());
        }
    }
}
```

**3. Controller Wiring** (`PaymentController.cs`)
```csharp
[HttpGet("pending")]
public async Task<ActionResult> GetPending()
{
    var result = await _mediator.Send(new GetPendingPaymentsQuery());
    return Ok(result);
}
```
