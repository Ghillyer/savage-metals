import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Location, LocationRequest } from '../../types/location';
import { locationKeys } from './useLocations';

interface SaveLocationArgs {
  id?: number;
  request: LocationRequest;
}

export function useSaveLocation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, request }: SaveLocationArgs) => {
      const response = id
        ? await api.put<Location>(`/locations/${id}`, request)
        : await api.post<Location>('/locations', request);
      return response.data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: locationKeys.all }),
  });
}