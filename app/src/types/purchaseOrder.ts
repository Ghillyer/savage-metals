export type PurchaseOrderStatus = 'Open' | 'Received';

export type MaterialGrade = 'Prime' | 'Secondary';

export interface PurchaseOrderSummary {
  id: number;
  poNumber: string;
  vendorName: string;
  status: PurchaseOrderStatus;
  createdAtUtc: string;
  total: number;
}

export interface PurchaseOrderLine {
  id: number;
  productId: number;
  productName: string;
  grade: MaterialGrade;
  weight: number;
  unitPrice: number;
  lineTotal: number;
}

export interface PurchaseOrder {
  id: number;
  poNumber: string;
  vendorId: number;
  vendorName: string;
  status: PurchaseOrderStatus;
  createdAtUtc: string;
  receivedAtUtc: string | null;
  total: number;
  lines: PurchaseOrderLine[];
}

export interface PurchaseOrderLineRequest {
  productId: number;
  grade: MaterialGrade;
  weight: number;
  unitPrice: number;
}

export interface PurchaseOrderRequest {
  poNumber: string;
  vendorId: number;
  lines: PurchaseOrderLineRequest[];
}