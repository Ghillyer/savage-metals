import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { InventoryReport } from '../../types/report';
import { inventoryKeys } from '../inventory/useInventory';

export function useInventoryReport() {
  return useQuery({
    queryKey: [...inventoryKeys.all, 'report'],
    queryFn: async () => {
      const response = await api.get<InventoryReport>('/reports/inventory');
      return response.data;
    },
  });
}