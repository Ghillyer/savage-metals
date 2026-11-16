import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { Location } from '../../types/location';

export const locationKeys = {
  all: ['locations'] as const,
};

export function useLocations() {
  return useQuery({
    queryKey: locationKeys.all,
    queryFn: async () => {
      const response = await api.get<Location[]>('/locations');
      return response.data;
    },
  });
}