export type PurchaseOrderStatus = 'Open' | 'Received';

export interface PurchaseOrderSummary {
  id: number;
  poNumber: string;
  vendorName: string;
  status: PurchaseOrderStatus;
  createdAtUtc: string;
  total: number;
}