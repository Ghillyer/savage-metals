using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
}