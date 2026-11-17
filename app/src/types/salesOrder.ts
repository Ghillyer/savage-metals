export interface SalesOrderSummary {
  id: number;
  soNumber: string;
  customerName: string;
  containerNumber: string;
  itemCount: number;
  totalWeight: number;
  revenue: number;
  cost: number;
  profit: number;
  margin: number;
  createdAtUtc: string;
}

export interface SalesOrderLine {
  id: number;
  inventoryItemId: number;
  tagNumber: string;
  productName: string;
  weight: number;
  unitPrice: number;
  revenue: number;
  cost: number;
  profit: number;
}

export interface SalesOrder {
  id: number;
  soNumber: string;
  customerId: number;
  customerName: string;
  containerNumber: string;
  createdAtUtc: string;
  totalWeight: number;
  revenue: number;
  cost: number;
  profit: number;
  margin: number;
  lines: SalesOrderLine[];
}

export interface SalesOrderLineRequest {
  inventoryItemId: number;
  unitPrice: number;
}

export interface SalesOrderRequest {
  soNumber: string;
  customerId: number;
  containerNumber: string;
  lines: SalesOrderLineRequest[];
}