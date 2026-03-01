using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Infrastructure.Repositories;
using LMS___Mini_Version.Mediators;
using LMS___Mini_Version.Persistence;
using LMS___Mini_Version.Services.Implementations;
using LMS___Mini_Version.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ─── Framework Services ───────────────────────────────────────
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ─── Database ─────────────────────────────────────────────────
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ─── Repository & Unit of Work ────────────────────────────────
            // [Trap 1 + 6 Fix] Controllers never touch DbContext.
            // All data access goes through IUnitOfWork → IGeneralRepository<T>.
            builder.Services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ─── Services (Single-Entity Steps) ───────────────────────────
            // [Trap 5 Fix] Business logic lives here, not in Controllers.
            builder.Services.AddScoped<ITrackService, TrackService>();
            builder.Services.AddScoped<IInternService, InternService>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            // ─── Mediators (Action Coordinators) ──────────────────────────
            // [Trap 5 + 6 Fix] Multi-step actions are orchestrated here.
            // ⚠️ [THE FINAL TRAP] Notice how every new action = another registration.
            //     This is the "Mediator Explosion" anti-pattern. The real fix is CQRS (MediatR).
            builder.Services.AddScoped<EnrollInternMediator>();
            builder.Services.AddScoped<CancelEnrollmentMediator>();
            builder.Services.AddScoped<TransferEnrollmentMediator>();

            var app = builder.Build();

            // ─── Seed Data ────────────────────────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();
                DbInitializer.Seed(context);
            }

            // ─── HTTP Pipeline ────────────────────────────────────────────
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
