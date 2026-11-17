import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Vendor } from '../../types/vendor';

export const vendorKeys = {
  all: ['vendors'] as const,
};

export function useVendors() {
  return useQuery({
    queryKey: vendorKeys.all,
    queryFn: async () => {
      const response = await api.get<Vendor[]>('/vendors');
      return response.data;
    },
  });
}