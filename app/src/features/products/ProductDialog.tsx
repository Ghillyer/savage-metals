import { useState, type FormEvent } from 'react';
import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel, Stack, Switch, TextField } from '@mui/material';
import { getErrorMessage } from '../../lib/errors';
import type { Product } from '../../types/product';
import { useSaveProduct } from './useSaveProduct';

interface ProductDialogProps {
  product?: Product;
  onClose: () => void;
}

function toInput(value: number | null | undefined) {
  return value?.toString() ?? '';
}

function toNumber(value: string) {
  return value.trim() === '' ? null : Number(value);
}

export function ProductDialog({ product, onClose }: ProductDialogProps) {
  const saveProduct = useSaveProduct();
  const [name, setName] = useState(product?.name ?? '');
  const [purchasePrice, setPurchasePrice] = useState(toInput(product?.purchasePrice));
  const [salesPrice, setSalesPrice] = useState(toInput(product?.salesPrice));
  const [isScrap, setIsScrap] = useState(product?.isScrap ?? false);
  const [isActive, setIsActive] = useState(product?.isActive ?? true);

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    saveProduct.mutate(
      {
        id: product?.id,
        request: {
          name,
          purchasePrice: toNumber(purchasePrice),
          salesPrice: toNumber(salesPrice),
          isScrap,
          isActive,
        },
      },
      { onSuccess: onClose },
    );
  };

  return (
    <Dialog open onClose={onClose} fullWidth maxWidth="sm">
      <Box component="form" onSubmit={handleSubmit}>
        <DialogTitle>{product ? 'Edit product' : 'New product'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ pt: 1 }}>
            {saveProduct.isError && <Alert severity="error">{getErrorMessage(saveProduct.error)}</Alert>}
            <TextField label="Name" value={name} onChange={(event) => setName(event.target.value)} required autoFocus />
            <TextField label="Purchase price per lb" type="number" value={purchasePrice} onChange={(event) => setPurchasePrice(event.target.value)} slotProps={{ htmlInput: { min: 0, step: 'any', inputMode: 'decimal' } }} />
            <TextField label="Sales price per lb" type="number" value={salesPrice} onChange={(event) => setSalesPrice(event.target.value)} slotProps={{ htmlInput: { min: 0, step: 'any', inputMode: 'decimal' } }} />
            <FormControlLabel control={<Switch checked={isScrap} onChange={(event) => setIsScrap(event.target.checked)} />} label="Scrap" />
            <FormControlLabel control={<Switch checked={isActive} onChange={(event) => setIsActive(event.target.checked)} />} label="Active" />
          </Stack>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 3 }}>
          <Button onClick={onClose}>Cancel</Button>
          <Button type="submit" variant="contained" disabled={saveProduct.isPending}>Save</Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}