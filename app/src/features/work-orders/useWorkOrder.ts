import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { WorkOrder } from '../../types/workOrder';
import { workOrderKeys } from './useWorkOrders';

export function useWorkOrder(id: number) {
  return useQuery({
    queryKey: workOrderKeys.detail(id),
    queryFn: async () => {
      const response = await api.get<WorkOrder>(`/work-orders/${id}`);
      return response.data;
    },
  });
}