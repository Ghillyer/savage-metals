using SavageMetals.Api.Core.DTOs.PurchaseOrders;
using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface IPurchaseOrderRepository
{
    Task<IReadOnlyList<PurchaseOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<PurchaseOrderResponse?> GetDetailAsync(int id, CancellationToken cancellationToken);
    Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PurchaseOrderLine>> GetLinesAsync(int purchaseOrderId, CancellationToken cancellationToken);
    Task<bool> PoNumberExistsAsync(string poNumber, int? excludeId, CancellationToken cancellationToken);
    Task<int?> GetMaxNumericPoNumberAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(PurchaseOrder purchaseOrder, IReadOnlyList<PurchaseOrderLine> lines, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(PurchaseOrder purchaseOrder, IReadOnlyList<PurchaseOrderLine> lines, CancellationToken cancellationToken);
    Task<bool> ReceiveAsync(int purchaseOrderId, DateTime receivedAtUtc, IReadOnlyList<InventoryItem> items, CancellationToken cancellationToken);
}