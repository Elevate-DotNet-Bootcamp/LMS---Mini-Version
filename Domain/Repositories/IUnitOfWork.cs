namespace LMS___Mini_Version.Domain.Repositories
{
    /// <summary>
    /// [SRP] Unit of Work is responsible ONLY for transaction management.
    /// It commits all staged changes across repositories in a single atomic transaction.
    /// Repositories are injected directly into Services via DI — NOT exposed here.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commits all staged changes to the database in one transaction.
        /// </summary>
        Task<int> CompleteAsync();
    }
}
