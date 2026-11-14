import { Button, Container, Stack, Typography } from '@mui/material';

export default function App() {
  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Stack spacing={2}>
        <Typography variant="h4">Savage Metals</Typography>
        <Button variant="contained">Big iPad Button</Button>
      </Stack>
    </Container>
  );
}