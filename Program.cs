using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Infrastructure.Repositories;
using LMS___Mini_Version.Persistence;
using LMS___Mini_Version.Services.Implementations;
using LMS___Mini_Version.Services.Interfaces;
using MediatR;
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
            builder.Services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ─── MediatR (CQRS) ──────────────────────────────────────────
            // Replaces ALL manual Mediators and refactored Services.
            // MediatR auto-discovers all IRequestHandler<,> implementations in this assembly.

            builder.Services.AddMediatR(typeof(Program).Assembly);
            // ─── Remaining Services (not yet migrated to CQRS) ───────────
            // InternService and PaymentService are still used by some handlers.
            builder.Services.AddScoped<IInternService, InternService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

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
