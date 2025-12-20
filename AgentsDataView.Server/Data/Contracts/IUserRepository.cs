

using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;

namespace AgentsDataView.Data.Repositories
{
    public interface IUserRepository : IRepository<SystemUser>
    {
        Task<SystemUser?> FindByUserAndPass(string username, string password, CancellationToken cancellationToken);
        Task AddAsync(SystemUser user, CancellationToken cancellationToken);
        Task UpdateLastLoginDateAsync(SystemUser user, CancellationToken requestAborted);
        Task<bool> UserIsValid(int userID, CancellationToken cancellationToken);
        Task UpdateAsync(SystemUser user, CancellationToken cancellationToken);
        Task DeleteAsync(int userID, CancellationToken cancellationToken);
    }
}