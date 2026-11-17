import { useState } from 'react';
import { Alert, Box, Button, Chip, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import type { Vendor } from '../../types/vendor';
import { useVendors } from './useVendors';
import { VendorDialog } from './VendorDialog';

export function VendorsPage() {
  const { data: vendors, isPending, isError } = useVendors(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedVendor, setSelectedVendor] = useState<Vendor>();

  const openDialog = (vendor?: Vendor) => {
    setSelectedVendor(vendor);
    setDialogOpen(true);
  };

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 3, alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h4">Vendors</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => openDialog()}>New vendor</Button>
      </Stack>
      {isPending && <CircularProgress />}
      {isError && <Alert severity="error">Vendors couldn't be loaded.</Alert>}
      {vendors && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {vendors.map((vendor) => (
                <TableRow key={vendor.id} hover onClick={() => openDialog(vendor)} sx={{ height: 56, cursor: 'pointer' }}>
                  <TableCell>{vendor.name}</TableCell>
                  <TableCell>{vendor.type}</TableCell>
                  <TableCell><Chip label={vendor.isActive ? 'Active' : 'Inactive'} color={vendor.isActive ? 'success' : 'default'} size="small" /></TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      {dialogOpen && <VendorDialog vendor={selectedVendor} onClose={() => setDialogOpen(false)} />}
    </Box>
  );
}