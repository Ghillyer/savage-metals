import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { WorkOrder, WorkOrderRequest } from '../../types/workOrder';
import { inventoryKeys } from '../inventory/useInventory';
import { workOrderKeys } from './useWorkOrders';

export function useCreateWorkOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: WorkOrderRequest) => {
      const response = await api.post<WorkOrder>('/work-orders', request);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: workOrderKeys.all });
      await queryClient.invalidateQueries({ queryKey: inventoryKeys.all });
    },
  });
}