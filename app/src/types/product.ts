export interface Product {
  id: number;
  name: string;
  purchasePrice: number | null;
  salesPrice: number | null;
  isScrap: boolean;
  isActive: boolean;
}

export interface ProductRequest {
  name: string;
  purchasePrice: number | null;
  salesPrice: number | null;
  isScrap: boolean;
  isActive: boolean;
}