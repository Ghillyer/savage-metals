import { useRef, useState, type FormEvent } from 'react';
import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, MenuItem, Stack, Switch, TextField } from '@mui/material';
import { getErrorMessage } from '../../lib/errors';
import type { Vendor, VendorType } from '../../types/vendor';
import { useSaveVendor } from './useSaveVendor';

interface VendorDialogProps {
  vendor?: Vendor;
  onClose: () => void;
}

const vendorTypes: VendorType[] = ['Supplier', 'Customer', 'Both'];

export function VendorDialog({ vendor, onClose }: VendorDialogProps) {
  const saveVendor = useSaveVendor();
  const nameRef = useRef<HTMLInputElement>(null);
  const [name, setName] = useState(vendor?.name ?? '');
  const [type, setType] = useState<VendorType>(vendor?.type ?? 'Supplier');
  const [isActive, setIsActive] = useState(vendor?.isActive ?? true);

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    saveVendor.mutate({ id: vendor?.id, request: { name, type, isActive } }, { onSuccess: onClose });
  };

  return (
    <Dialog open onClose={onClose} fullWidth maxWidth="sm" slotProps={{ transition: { onEntered: () => nameRef.current?.focus() } }}>
      <Box component="form" onSubmit={handleSubmit}>
        <DialogTitle>{vendor ? 'Edit vendor' : 'New vendor'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ pt: 1 }}>
            {saveVendor.isError && <Alert severity="error">{getErrorMessage(saveVendor.error)}</Alert>}
            <TextField label="Name" value={name} onChange={(event) => setName(event.target.value)} required inputRef={nameRef} />
            <TextField select label="Type" value={type} onChange={(event) => setType(event.target.value as VendorType)} required>
              {vendorTypes.map((vendorType) => (
                <MenuItem key={vendorType} value={vendorType}>{vendorType}</MenuItem>
              ))}
            </TextField>
            <FormControlLabel control={<Switch checked={isActive} onChange={(event) => setIsActive(event.target.checked)} />} label="Active" />
          </Stack>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3 }}>
          <Button onClick={onClose}>Cancel</Button>
          <Button type="submit" variant="contained" disabled={saveVendor.isPending}>Save</Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}