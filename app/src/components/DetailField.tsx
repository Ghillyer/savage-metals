import { Box, Typography } from '@mui/material';

interface DetailFieldProps {
  label: string;
  value: string;
}

export function DetailField({ label, value }: DetailFieldProps) {
  return (
    <Box>
      <Typography variant="body2" color="text.secondary">{label}</Typography>
      <Typography>{value}</Typography>
    </Box>
  );
}