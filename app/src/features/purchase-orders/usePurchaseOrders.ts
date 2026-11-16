import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { PurchaseOrderSummary } from '../../types/purchaseOrder';

export const purchaseOrderKeys = {
  all: ['purchase-orders'] as const,
  detail: (id: number) => [...purchaseOrderKeys.all, id] as const,
};

export function usePurchaseOrders() {
  return useQuery({
    queryKey: purchaseOrderKeys.all,
    queryFn: async () => {
      const response = await api.get<PurchaseOrderSummary[]>('/purchase-orders');
      return response.data;
    },
  });
}