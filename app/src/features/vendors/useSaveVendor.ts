import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Vendor, VendorRequest } from '../../types/vendor';
import { vendorKeys } from './useVendors';

interface SaveVendorArgs {
  id?: number;
  request: VendorRequest;
}

export function useSaveVendor() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, request }: SaveVendorArgs) => {
      const response = id
        ? await api.put<Vendor>(`/vendors/${id}`, request)
        : await api.post<Vendor>('/vendors', request);
      return response.data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: vendorKeys.all }),
  });
}