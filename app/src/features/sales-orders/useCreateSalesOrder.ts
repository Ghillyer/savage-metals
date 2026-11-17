import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { SalesOrder, SalesOrderRequest } from '../../types/salesOrder';
import { inventoryKeys } from '../inventory/useInventory';
import { salesOrderKeys } from './useSalesOrders';

export function useCreateSalesOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: SalesOrderRequest) => {
      const response = await api.post<SalesOrder>('/sales-orders', request);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: salesOrderKeys.all });
      await queryClient.invalidateQueries({ queryKey: inventoryKeys.all });
    },
  });
}