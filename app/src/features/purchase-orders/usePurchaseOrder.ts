import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { PurchaseOrder } from '../../types/purchaseOrder';
import { purchaseOrderKeys } from './usePurchaseOrders';

export function usePurchaseOrder(id: number) {
  return useQuery({
    queryKey: purchaseOrderKeys.detail(id),
    queryFn: async () => {
      const response = await api.get<PurchaseOrder>(`/purchase-orders/${id}`);
      return response.data;
    },
  });
}