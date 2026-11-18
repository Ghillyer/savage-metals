import { useState, type FormEvent } from 'react';
import { Navigate, useLocation, type Location } from 'react-router';
import { Alert, Box, Button, Paper, Stack, TextField, Typography } from '@mui/material';
import { LoadingScreen } from '../components/LoadingScreen';
import { useAuth } from './useAuth';

interface LoginLocationState {
  from?: Location;
}

export function LoginPage() {
  const { currentUser, loading, signIn } = useAuth();
  const location = useLocation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const redirectTo = (location.state as LoginLocationState | null)?.from?.pathname ?? '/';

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setSubmitting(true);

    try {
      await signIn(email, password);
    } catch {
      setError('Invalid email or password.');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <LoadingScreen />;
  }

  if (currentUser) {
    return <Navigate to={redirectTo} replace />;
  }

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', p: 2, background: 'linear-gradient(160deg, #050816 40%, #0D1A4F)' }}>
      <Paper sx={{ p: 4, width: '100%', maxWidth: 420 }}>
        <Box component="form" onSubmit={handleSubmit}>
          <Stack spacing={3}>
            <Typography variant="h4" sx={{ fontWeight: 700, letterSpacing: 1 }}>SAVAGE <Box component="span" sx={{ color: 'secondary.main', fontSize: '0.7em', letterSpacing: 3 }}>METALS</Box></Typography>
            {error && <Alert severity="error">{error}</Alert>}
            <TextField label="Email" type="email" autoComplete="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
            <TextField label="Password" type="password" autoComplete="current-password" value={password} onChange={(event) => setPassword(event.target.value)} required />
            <Button type="submit" variant="contained" disabled={submitting}>Sign in</Button>
          </Stack>
        </Box>
      </Paper>
    </Box>
  );
}