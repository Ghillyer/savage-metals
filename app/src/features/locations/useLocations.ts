import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Location } from '../../types/location';

export const locationKeys = {
  all: ['locations'] as const,
  list: (includeInactive: boolean) => [...locationKeys.all, { includeInactive }] as const,
};

export function useLocations(includeInactive = false) {
  return useQuery({
    queryKey: locationKeys.list(includeInactive),
    queryFn: async () => {
      const response = await api.get<Location[]>('/locations', { params: { includeInactive } });
      return response.data;
    },
  });
}