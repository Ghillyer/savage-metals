import { useQuery } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { AppUser } from '../../types/user';

export const userKeys = {
  all: ['users'] as const,
};

export function useUsers() {
  return useQuery({
    queryKey: userKeys.all,
    queryFn: async () => {
      const response = await api.get<AppUser[]>('/users');
      return response.data;
    },
  });
}