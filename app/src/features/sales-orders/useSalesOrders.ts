import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { SalesOrderSummary } from '../../types/salesOrder';

export const salesOrderKeys = {
  all: ['sales-orders'] as const,
  detail: (id: number) => [...salesOrderKeys.all, id] as const,
};

export function useSalesOrders() {
  return useQuery({
    queryKey: salesOrderKeys.all,
    queryFn: async () => {
      const response = await api.get<SalesOrderSummary[]>('/sales-orders');
      return response.data;
    },
  });
}