using SavageMetals.Api.Core.DTOs.WorkOrders;

namespace SavageMetals.Api.Core.Interfaces;

public interface IWorkOrderService
{
    Task<IReadOnlyList<WorkOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkOrderResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<WorkOrderResponse> CreateAsync(WorkOrderRequest request, CancellationToken cancellationToken = default);
}