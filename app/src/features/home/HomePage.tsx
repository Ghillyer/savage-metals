import { Box, Button, Stack, Typography } from '@mui/material';
import { useAuth } from '../../auth/useAuth';

export function HomePage() {
  const { currentUser, signOut } = useAuth();

  return (
    <Box sx={{ p: 4 }}>
      <Stack spacing={3} sx={{ maxWidth: 480 }}>
        <Typography variant="h4">Hello, {currentUser?.displayName}</Typography>
        <Typography color="text.secondary">Role: {currentUser?.role}</Typography>
        <Button variant="outlined" onClick={signOut}>Sign out</Button>
      </Stack>
    </Box>
  );
}