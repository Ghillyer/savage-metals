import { useState, type FormEvent } from 'react';
import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, Stack, Switch, TextField } from '@mui/material';
import { getErrorMessage } from '../../lib/errors';
import type { Location } from '../../types/location';
import { useSaveLocation } from './useSaveLocation';

interface LocationDialogProps {
  location?: Location;
  onClose: () => void;
}

export function LocationDialog({ location, onClose }: LocationDialogProps) {
  const saveLocation = useSaveLocation();
  const [name, setName] = useState(location?.name ?? '');
  const [isActive, setIsActive] = useState(location?.isActive ?? true);

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    saveLocation.mutate({ id: location?.id, request: { name, isActive } }, { onSuccess: onClose });
  };

  return (
    <Dialog open onClose={onClose} fullWidth maxWidth="sm">
      <Box component="form" onSubmit={handleSubmit}>
        <DialogTitle>{location ? 'Edit location' : 'New location'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ pt: 1 }}>
            {saveLocation.isError && <Alert severity="error">{getErrorMessage(saveLocation.error)}</Alert>}
            <TextField label="Name" value={name} onChange={(event) => setName(event.target.value)} required />
            <FormControlLabel control={<Switch checked={isActive} onChange={(event) => setIsActive(event.target.checked)} />} label="Active" />
          </Stack>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3 }}>
          <Button onClick={onClose}>Cancel</Button>
          <Button type="submit" variant="contained" disabled={saveLocation.isPending}>Save</Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}