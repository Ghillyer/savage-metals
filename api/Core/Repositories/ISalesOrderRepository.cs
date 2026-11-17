using SavageMetals.Api.Core.DTOs.SalesOrders;
using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface ISalesOrderRepository
{
    Task<IReadOnlyList<SalesOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SalesOrderResponse?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> SoNumberExistsAsync(string soNumber, CancellationToken cancellationToken = default);
    Task<int?> CreateAsync(SalesOrder salesOrder, IReadOnlyList<SalesOrderLine> lines, CancellationToken cancellationToken = default);
}