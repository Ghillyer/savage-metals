import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { PurchaseOrder, PurchaseOrderRequest } from '../../types/purchaseOrder';
import { purchaseOrderKeys } from './usePurchaseOrders';

export function useCreatePurchaseOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: PurchaseOrderRequest) => {
      const response = await api.post<PurchaseOrder>('/purchase-orders', request);
      return response.data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: purchaseOrderKeys.all }),
  });
}