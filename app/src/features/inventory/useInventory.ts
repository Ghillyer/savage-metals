import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { InventoryItem, InventoryStatus } from '../../types/inventory';

export const inventoryKeys = {
  all: ['inventory'] as const,
  list: (status?: InventoryStatus) => [...inventoryKeys.all, status ?? 'all'] as const,
};

export function useInventory(status?: InventoryStatus) {
  return useQuery({
    queryKey: inventoryKeys.list(status),
    queryFn: async () => {
      const response = await api.get<InventoryItem[]>('/inventory', { params: { status } });
      return response.data;
    },
  });
}