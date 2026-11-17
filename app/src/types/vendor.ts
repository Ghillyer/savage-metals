export type VendorType = 'Supplier' | 'Customer' | 'Both';

export interface Vendor {
  id: number;
  name: string;
  type: VendorType;
  isActive: boolean;
}

export interface VendorRequest {
  name: string;
  type: VendorType;
  isActive: boolean;
}