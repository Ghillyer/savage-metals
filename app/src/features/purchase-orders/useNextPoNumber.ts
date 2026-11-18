import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { NextPurchaseOrderNumber } from '../../types/purchaseOrder';
import { purchaseOrderKeys } from './usePurchaseOrders';

export function useNextPoNumber() {
  return useQuery({
    queryKey: [...purchaseOrderKeys.all, 'next-number'],
    queryFn: async () => {
      const response = await api.get<NextPurchaseOrderNumber>('/purchase-orders/next-number');
      return response.data.poNumber;
    },
  });
}