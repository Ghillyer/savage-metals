import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router';
import { Alert, Box, Button, IconButton, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import { getErrorMessage } from '../../lib/errors';
import { formatCurrency, formatWeight } from '../../lib/format';
import { useInventory } from '../inventory/useInventory';
import { useProducts } from '../products/useProducts';
import { useVendors } from '../vendors/useVendors';
import { useCreateSalesOrder } from './useCreateSalesOrder';

interface LineForm {
  key: string;
  inventoryItemId: string;
  unitPrice: string;
}

function createLine(): LineForm {
  return { key: crypto.randomUUID(), inventoryItemId: '', unitPrice: '' };
}

export function CreateSalesOrderPage() {
  const navigate = useNavigate();
  const { data: vendors = [] } = useVendors();
  const { data: inventory = [] } = useInventory('Available');
  const { data: products = [] } = useProducts();
  const createSalesOrder = useCreateSalesOrder();
  const [soNumber, setSoNumber] = useState('');
  const [customerId, setCustomerId] = useState('');
  const [containerNumber, setContainerNumber] = useState('');
  const [lines, setLines] = useState<LineForm[]>([createLine()]);

  const customers = vendors.filter((vendor) => vendor.type !== 'Supplier');
  const selectedIds = new Set(lines.map((line) => Number(line.inventoryItemId)));

  const getItem = (line: LineForm) => inventory.find((item) => item.id === Number(line.inventoryItemId));

  const calculateRevenue = (line: LineForm) => {
    const item = getItem(line);
    return item ? Math.round(item.weight * Number(line.unitPrice) * 100) / 100 || 0 : 0;
  };

  const totalRevenue = lines.reduce((total, line) => total + calculateRevenue(line), 0);

  const updateLine = (key: string, changes: Partial<LineForm>) => {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...changes } : line)));
  };

  const handleItemChange = (key: string, inventoryItemId: string) => {
    const item = inventory.find((i) => i.id === Number(inventoryItemId));
    const product = products.find((p) => p.id === item?.productId);
    updateLine(key, { inventoryItemId, unitPrice: product?.salesPrice?.toString() ?? '' });
  };

  const removeLine = (key: string) => {
    setLines((current) => current.filter((line) => line.key !== key));
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    createSalesOrder.mutate(
      {
        soNumber,
        customerId: Number(customerId),
        containerNumber,
        lines: lines.map((line) => ({
          inventoryItemId: Number(line.inventoryItemId),
          unitPrice: Number(line.unitPrice),
        })),
      },
      { onSuccess: (salesOrder) => navigate(`/sales-orders/${salesOrder.id}`) },
    );
  };

  return (
    <Box component="form" onSubmit={handleSubmit}>
      <Typography variant="h4" sx={{ mb: 3 }}>New Sales Order</Typography>
      <Stack spacing={3}>
        {createSalesOrder.isError && <Alert severity="error">{getErrorMessage(createSalesOrder.error)}</Alert>}
        <Paper sx={{ p: 3 }}>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(3, 1fr)' }, gap: 2 }}>
            <TextField label="SO number" value={soNumber} onChange={(event) => setSoNumber(event.target.value)} required />
            <TextField select label="Customer" value={customerId} onChange={(event) => setCustomerId(event.target.value)} required>
              {customers.map((customer) => (
                <MenuItem key={customer.id} value={customer.id.toString()}>{customer.name}</MenuItem>
              ))}
            </TextField>
            <TextField label="Container number" value={containerNumber} onChange={(event) => setContainerNumber(event.target.value)} required />
          </Box>
        </Paper>
        <Paper sx={{ p: 3 }}>
          <Stack spacing={3}>
            <Typography variant="h6">Items</Typography>
            {lines.map((line) => {
              const item = getItem(line);
              const options = inventory.filter((i) => i.id === item?.id || !selectedIds.has(i.id));

              return (
                <Box key={line.key} sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '3fr 1fr 1fr auto' }, gap: 2, alignItems: 'center' }}>
                  <TextField select label="Item" value={line.inventoryItemId} onChange={(event) => handleItemChange(line.key, event.target.value)} required>
                    {options.map((option) => (
                      <MenuItem key={option.id} value={option.id.toString()}>{option.tagNumber} · {option.productName} · {formatWeight(option.weight)}</MenuItem>
                    ))}
                  </TextField>
                  <TextField label="Price per lb" type="number" value={line.unitPrice} onChange={(event) => updateLine(line.key, { unitPrice: event.target.value })} slotProps={{ htmlInput: { min: 0, step: 'any', inputMode: 'decimal' } }} required />
                  <Typography sx={{ minWidth: 120, textAlign: 'right' }}>{formatCurrency(calculateRevenue(line))}</Typography>
                  <IconButton aria-label="Remove item" onClick={() => removeLine(line.key)} disabled={lines.length === 1} sx={{ width: 56, height: 56 }}>
                    <DeleteIcon />
                  </IconButton>
                </Box>
              );
            })}
            <Box>
              <Button startIcon={<AddIcon />} onClick={() => setLines((current) => [...current, createLine()])}>Add item</Button>
            </Box>
          </Stack>
        </Paper>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          <Typography variant="h6" sx={{ mr: 'auto' }}>Revenue: {formatCurrency(totalRevenue)}</Typography>
          <Button component={Link} to="/sales-orders">Cancel</Button>
          <Button type="submit" variant="contained" disabled={createSalesOrder.isPending}>Create sales order</Button>
        </Stack>
      </Stack>
    </Box>
  );
}