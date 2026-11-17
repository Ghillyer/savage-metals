using SavageMetals.Api.Core.DTOs.WorkOrders;
using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface IWorkOrderRepository
{
    Task<IReadOnlyList<WorkOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkOrderResponse?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<int?> CreateAsync(WorkOrder workOrder, IReadOnlyList<InventoryItem> outputs, CancellationToken cancellationToken = default);
}