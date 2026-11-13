using System.Data.Common;

namespace SavageMetals.Api.Core.Interfaces;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}