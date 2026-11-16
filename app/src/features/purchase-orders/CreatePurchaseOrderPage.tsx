import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router';
import { Alert, Box, Button, IconButton, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import { getErrorMessage } from '../../lib/errors';
import { formatCurrency } from '../../lib/format';
import type { MaterialGrade } from '../../types/purchaseOrder';
import { useProducts } from '../products/useProducts';
import { useCreatePurchaseOrder } from './useCreatePurchaseOrder';
import { useVendors } from '../vendors/useVendor';

interface LineForm {
  key: string;
  productId: string;
  grade: MaterialGrade;
  weight: string;
  unitPrice: string;
}

const grades: MaterialGrade[] = ['Prime', 'Secondary'];

function createLine(): LineForm {
  return { key: crypto.randomUUID(), productId: '', grade: 'Prime', weight: '', unitPrice: '' };
}

function calculateLineTotal(line: LineForm) {
  return Math.round(Number(line.weight) * Number(line.unitPrice) * 100) / 100 || 0;
}

export function CreatePurchaseOrderPage() {
  const navigate = useNavigate();
  const { data: vendors = [] } = useVendors();
  const { data: products = [] } = useProducts();
  const createPurchaseOrder = useCreatePurchaseOrder();
  const [poNumber, setPoNumber] = useState('');
  const [vendorId, setVendorId] = useState('');
  const [lines, setLines] = useState<LineForm[]>([createLine()]);

  const suppliers = vendors.filter((vendor) => vendor.type !== 'Customer');
  const orderTotal = lines.reduce((total, line) => total + calculateLineTotal(line), 0);

  const updateLine = (key: string, changes: Partial<LineForm>) => {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...changes } : line)));
  };

  const handleProductChange = (key: string, productId: string) => {
    const product = products.find((p) => p.id === Number(productId));
    updateLine(key, { productId, unitPrice: product?.purchasePrice?.toString() ?? '' });
  };

  const removeLine = (key: string) => {
    setLines((current) => current.filter((line) => line.key !== key));
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    createPurchaseOrder.mutate(
      {
        poNumber,
        vendorId: Number(vendorId),
        lines: lines.map((line) => ({
          productId: Number(line.productId),
          grade: line.grade,
          weight: Number(line.weight),
          unitPrice: Number(line.unitPrice),
        })),
      },
      { onSuccess: () => navigate('/purchase-orders') },
    );
  };

  return (
    <Box component="form" onSubmit={handleSubmit}>
      <Typography variant="h4" sx={{ mb: 3 }}>New Purchase Order</Typography>
      <Stack spacing={3}>
        {createPurchaseOrder.isError && <Alert severity="error">{getErrorMessage(createPurchaseOrder.error)}</Alert>}
        <Paper sx={{ p: 3 }}>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="PO number" value={poNumber} onChange={(event) => setPoNumber(event.target.value)} required />
            <TextField select label="Vendor" value={vendorId} onChange={(event) => setVendorId(event.target.value)} required>
              {suppliers.map((vendor) => (
                <MenuItem key={vendor.id} value={vendor.id.toString()}>{vendor.name}</MenuItem>
              ))}
            </TextField>
          </Stack>
        </Paper>
        <Paper sx={{ p: 3 }}>
          <Stack spacing={3}>
            <Typography variant="h6">Lines</Typography>
            {lines.map((line) => (
              <Box key={line.key} sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '2fr 1fr 1fr 1fr auto auto' }, gap: 2, alignItems: 'center' }}>
                <TextField select label="Product" value={line.productId} onChange={(event) => handleProductChange(line.key, event.target.value)} required>
                  {products.map((product) => (
                    <MenuItem key={product.id} value={product.id.toString()}>{product.name}</MenuItem>
                  ))}
                </TextField>
                <TextField select label="Grade" value={line.grade} onChange={(event) => updateLine(line.key, { grade: event.target.value as MaterialGrade })} required>
                  {grades.map((grade) => (
                    <MenuItem key={grade} value={grade}>{grade}</MenuItem>
                  ))}
                </TextField>
                <TextField label="Weight (lbs)" type="number" value={line.weight} onChange={(event) => updateLine(line.key, { weight: event.target.value })} slotProps={{ htmlInput: { min: 0, step: 'any', inputMode: 'decimal' } }} required />
                <TextField label="Price per lb" type="number" value={line.unitPrice} onChange={(event) => updateLine(line.key, { unitPrice: event.target.value })} slotProps={{ htmlInput: { min: 0, step: 'any', inputMode: 'decimal' } }} required />
                <Typography sx={{ minWidth: 120, textAlign: 'right' }}>{formatCurrency(calculateLineTotal(line))}</Typography>
                <IconButton aria-label="Remove line" onClick={() => removeLine(line.key)} disabled={lines.length === 1} sx={{ width: 56, height: 56 }}>
                  <DeleteIcon />
                </IconButton>
              </Box>
            ))}
            <Box>
              <Button startIcon={<AddIcon />} onClick={() => setLines((current) => [...current, createLine()])}>Add line</Button>
            </Box>
          </Stack>
        </Paper>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          <Typography variant="h6" sx={{ mr: 'auto' }}>Total: {formatCurrency(orderTotal)}</Typography>
          <Button component={Link} to="/purchase-orders">Cancel</Button>
          <Button type="submit" variant="contained" disabled={createPurchaseOrder.isPending}>Create purchase order</Button>
        </Stack>
      </Stack>
    </Box>
  );
}