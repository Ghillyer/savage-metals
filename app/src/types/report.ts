export interface InventoryReportRow {
  name: string;
  itemCount: number;
  totalWeight: number;
  totalValue: number;
}

export interface InventoryReport {
  itemCount: number;
  totalWeight: number;
  totalValue: number;
  byProduct: InventoryReportRow[];
  byLocation: InventoryReportRow[];
}