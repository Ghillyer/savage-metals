import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Product } from '../../types/product';

export const productKeys = {
  all: ['products'] as const,
  list: (includeInactive: boolean) => [...productKeys.all, { includeInactive }] as const,
};

export function useProducts(includeInactive = false) {
  return useQuery({
    queryKey: productKeys.list(includeInactive),
    queryFn: async () => {
      const response = await api.get<Product[]>('/products', { params: { includeInactive } });
      return response.data;
    },
  });
}