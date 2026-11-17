import { useMutation, useQueryClient } from '@tanstack/react-query';
import { sendPasswordResetEmail } from 'firebase/auth';
import { api } from '../../lib/api';
import { auth } from '../../lib/firebase';
import type { AppUser, CreateUserRequest } from '../../types/user';
import { userKeys } from './useUsers';

export function useCreateUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: CreateUserRequest) => {
      const response = await api.post<AppUser>('/users', request);
      await sendPasswordResetEmail(auth, response.data.email);
      return response.data;
    },
    onSettled: () => queryClient.invalidateQueries({ queryKey: userKeys.all }),
  });
}