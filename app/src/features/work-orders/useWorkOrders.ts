import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { WorkOrderSummary } from '../../types/workOrder';

export const workOrderKeys = {
  all: ['work-orders'] as const,
  detail: (id: number) => [...workOrderKeys.all, id] as const,
};

export function useWorkOrders() {
  return useQuery({
    queryKey: workOrderKeys.all,
    queryFn: async () => {
      const response = await api.get<WorkOrderSummary[]>('/work-orders');
      return response.data;
    },
  });
}