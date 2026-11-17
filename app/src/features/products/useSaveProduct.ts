import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Product, ProductRequest } from '../../types/product';
import { productKeys } from './useProducts';

interface SaveProductArgs {
  id?: number;
  request: ProductRequest;
}

export function useSaveProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, request }: SaveProductArgs) => {
      const response = id
        ? await api.put<Product>(`/products/${id}`, request)
        : await api.post<Product>('/products', request);
      return response.data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: productKeys.all }),
  });
}