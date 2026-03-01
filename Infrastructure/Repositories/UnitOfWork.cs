using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Persistence;

namespace LMS___Mini_Version.Infrastructure.Repositories
{
    /// <summary>
    /// [SRP] Unit of Work implementation — responsible ONLY for transaction management.
    /// Wraps the DbContext and commits all staged changes atomically via CompleteAsync().
    /// 
    /// Repositories are no longer created or exposed here.
    /// Each Service injects its own IGeneralRepository&lt;T&gt; directly via DI,
    /// and all repositories share the same scoped DbContext instance automatically.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Commits ALL staged changes across ALL repositories in a single DB transaction.
        /// </summary>
        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync().ConfigureAwait(false);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
