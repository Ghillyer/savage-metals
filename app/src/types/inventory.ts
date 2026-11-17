export type InventoryStatus = 'Available' | 'Consumed' | 'Sold';

export interface InventoryItem {
  id: number;
  tagNumber: string;
  productId: number;
  productName: string;
  isScrap: boolean;
  locationId: number;
  locationName: string;
  weight: number;
  totalCost: number;
  status: InventoryStatus;
  createdAtUtc: string;
}