import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../lib/api';
import type { AppUser, UpdateUserRequest } from '../../types/user';
import { userKeys } from './useUsers';

interface UpdateUserArgs {
  id: number;
  request: UpdateUserRequest;
}

export function useUpdateUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, request }: UpdateUserArgs) => {
      const response = await api.put<AppUser>(`/users/${id}`, request);
      return response.data;
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: userKeys.all }),
  });
}