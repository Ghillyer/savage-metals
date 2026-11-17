import type { InventoryStatus } from './inventory';

export interface WorkOrderSummary {
  id: number;
  workOrderNumber: string;
  inputTagNumber: string;
  inputProductName: string;
  inputWeight: number;
  outputCount: number;
  createdAtUtc: string;
}

export interface WorkOrderOutput {
  id: number;
  tagNumber: string;
  productName: string;
  isScrap: boolean;
  locationName: string;
  weight: number;
  totalCost: number;
  status: InventoryStatus;
}

export interface WorkOrder {
  id: number;
  workOrderNumber: string;
  inputItemId: number;
  inputTagNumber: string;
  inputProductName: string;
  inputWeight: number;
  inputCost: number;
  createdAtUtc: string;
  outputs: WorkOrderOutput[];
}

export interface WorkOrderOutputRequest {
  tagNumber: string;
  productId: number;
  locationId: number;
  weight: number;
}

export interface WorkOrderRequest {
  inputItemId: number;
  outputs: WorkOrderOutputRequest[];
}