import { useState } from 'react';
import { Alert, Box, Button, Chip, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import PersonAddIcon from '@mui/icons-material/PersonAdd';
import type { AppUser } from '../../types/user';
import { UserDialog } from './UserDialog';
import { useUsers } from './useUsers';

export function UsersPage() {
  const { data: users, isPending, isError } = useUsers();
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<AppUser>();

  const openDialog = (user?: AppUser) => {
    setSelectedUser(user);
    setDialogOpen(true);
  };

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 3, alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h4">Users</Typography>
        <Button variant="contained" startIcon={<PersonAddIcon />} onClick={() => openDialog()}>Invite user</Button>
      </Stack>
      {isPending && <CircularProgress />}
      {isError && <Alert severity="error">Users couldn't be loaded.</Alert>}
      {users && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Role</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {users.map((user) => (
                <TableRow key={user.id} hover onClick={() => openDialog(user)} sx={{ height: 56, cursor: 'pointer' }}>
                  <TableCell>{user.displayName}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{user.role}</TableCell>
                  <TableCell><Chip label={user.isActive ? 'Active' : 'Inactive'} color={user.isActive ? 'success' : 'default'} size="small" /></TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      {dialogOpen && <UserDialog user={selectedUser} onClose={() => setDialogOpen(false)} />}
    </Box>
  );
}