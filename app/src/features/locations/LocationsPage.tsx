import { useState } from 'react';
import { Alert, Box, Button, Chip, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import type { Location } from '../../types/location';
import { LocationDialog } from './LocationDialog';
import { useLocations } from './useLocations';

export function LocationsPage() {
  const { data: locations, isPending, isError } = useLocations(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedLocation, setSelectedLocation] = useState<Location>();

  const openDialog = (location?: Location) => {
    setSelectedLocation(location);
    setDialogOpen(true);
  };

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 3, alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h4">Locations</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => openDialog()}>New location</Button>
      </Stack>
      {isPending && <CircularProgress />}
      {isError && <Alert severity="error">Locations couldn't be loaded.</Alert>}
      {locations && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {locations.map((location) => (
                <TableRow key={location.id} hover onClick={() => openDialog(location)} sx={{ height: 56, cursor: 'pointer' }}>
                  <TableCell>{location.name}</TableCell>
                  <TableCell><Chip label={location.isActive ? 'Active' : 'Inactive'} color={location.isActive ? 'success' : 'default'} size="small" /></TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      {dialogOpen && <LocationDialog location={selectedLocation} onClose={() => setDialogOpen(false)} />}
    </Box>
  );
}