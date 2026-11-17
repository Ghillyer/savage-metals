using SavageMetals.Api.Core.DTOs.SalesOrders;

namespace SavageMetals.Api.Core.Interfaces;

public interface ISalesOrderService
{
    Task<IReadOnlyList<SalesOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SalesOrderResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SalesOrderResponse> CreateAsync(SalesOrderRequest request, CancellationToken cancellationToken = default);
}