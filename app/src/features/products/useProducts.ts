import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Product } from '../../types/product';

export const productKeys = {
  all: ['products'] as const,
};

export function useProducts() {
  return useQuery({
    queryKey: productKeys.all,
    queryFn: async () => {
      const response = await api.get<Product[]>('/products');
      return response.data;
    },
  });
}