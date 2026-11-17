import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { SalesOrder } from '../../types/salesOrder';
import { salesOrderKeys } from './useSalesOrders';

export function useSalesOrder(id: number) {
  return useQuery({
    queryKey: salesOrderKeys.detail(id),
    queryFn: async () => {
      const response = await api.get<SalesOrder>(`/sales-orders/${id}`);
      return response.data;
    },
  });
}