using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AppUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppUser?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(AppUser user, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppUser user, CancellationToken cancellationToken = default);
}