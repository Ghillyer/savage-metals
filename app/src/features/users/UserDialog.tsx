import { useRef, useState, type FormEvent } from 'react';
import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, MenuItem, Stack, Switch, TextField, Typography } from '@mui/material';
import { useAuth } from '../../auth/useAuth';
import { getErrorMessage } from '../../lib/errors';
import type { AppUser, UserRole } from '../../types/user';
import { useCreateUser } from './useCreateUser';
import { useUpdateUser } from './useUpdateUser';

interface UserDialogProps {
  user?: AppUser;
  onClose: () => void;
}

const roles: UserRole[] = ['Purchaser', 'Admin'];

export function UserDialog({ user, onClose }: UserDialogProps) {
  const { currentUser } = useAuth();
  const createUser = useCreateUser();
  const updateUser = useUpdateUser();
  const firstFieldRef = useRef<HTMLInputElement>(null);
  const [email, setEmail] = useState('');
  const [displayName, setDisplayName] = useState(user?.displayName ?? '');
  const [role, setRole] = useState<UserRole>(user?.role ?? 'Purchaser');
  const [isActive, setIsActive] = useState(user?.isActive ?? true);

  const isSelf = user?.id === currentUser?.id;
  const mutation = user ? updateUser : createUser;

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (user) {
      updateUser.mutate({ id: user.id, request: { displayName, role, isActive } }, { onSuccess: onClose });
    } else {
      createUser.mutate({ email, displayName, role }, { onSuccess: onClose });
    }
  };

  return (
    <Dialog open onClose={onClose} fullWidth maxWidth="sm" slotProps={{ transition: { onEntered: () => firstFieldRef.current?.focus() } }}>
      <Box component="form" onSubmit={handleSubmit}>
        <DialogTitle>{user ? 'Edit user' : 'Invite user'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ pt: 1 }}>
            {mutation.isError && <Alert severity="error">{getErrorMessage(mutation.error)}</Alert>}
            {user ? (
              <TextField label="Email" value={user.email} disabled />
            ) : (
              <TextField label="Email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} required inputRef={firstFieldRef} />
            )}
            <TextField label="Name" value={displayName} onChange={(event) => setDisplayName(event.target.value)} required inputRef={user ? firstFieldRef : undefined} />
            <TextField select label="Role" value={role} onChange={(event) => setRole(event.target.value as UserRole)} disabled={isSelf} required>
              {roles.map((option) => (
                <MenuItem key={option} value={option}>{option}</MenuItem>
              ))}
            </TextField>
            {user && <FormControlLabel control={<Switch checked={isActive} onChange={(event) => setIsActive(event.target.checked)} disabled={isSelf} />} label="Active" />}
            {isSelf && <Typography variant="body2" color="text.secondary">You can't change your own role or deactivate your own account.</Typography>}
            {!user && <Typography variant="body2" color="text.secondary">They'll get an email with a link to set their password.</Typography>}
          </Stack>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3 }}>
          <Button onClick={onClose}>Cancel</Button>
          <Button type="submit" variant="contained" disabled={mutation.isPending}>{user ? 'Save' : 'Send invite'}</Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}