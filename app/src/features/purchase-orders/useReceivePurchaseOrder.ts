import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { PurchaseOrder, ReceivePurchaseOrderRequest } from '../../types/purchaseOrder';
import { purchaseOrderKeys } from './usePurchaseOrders';

export function useReceivePurchaseOrder(id: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: ReceivePurchaseOrderRequest) => {
      const response = await api.post<PurchaseOrder>(`/purchase-orders/${id}/receive`, request);
      return response.data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: purchaseOrderKeys.all }),
  });
}