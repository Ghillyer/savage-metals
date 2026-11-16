using SavageMetals.Api.Core.DTOs.PurchaseOrders;

namespace SavageMetals.Api.Core.Interfaces;

public interface IPurchaseOrderService
{
    Task<IReadOnlyList<PurchaseOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<PurchaseOrderResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PurchaseOrderResponse> CreateAsync(PurchaseOrderRequest request, CancellationToken cancellationToken);
    Task<PurchaseOrderResponse> UpdateAsync(int id, PurchaseOrderRequest request, CancellationToken cancellationToken);
    Task<PurchaseOrderResponse> ReceiveAsync(int id, ReceivePurchaseOrderRequest request, CancellationToken cancellationToken);
}