import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Vendor } from '../../types/vendor';

export const vendorKeys = {
  all: ['vendors'] as const,
  list: (includeInactive: boolean) => [...vendorKeys.all, { includeInactive }] as const,
};

export function useVendors(includeInactive = false) {
  return useQuery({
    queryKey: vendorKeys.list(includeInactive),
    queryFn: async () => {
      const response = await api.get<Vendor[]>('/vendors', { params: { includeInactive } });
      return response.data;
    },
  });
}