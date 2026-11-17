import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router';
import { Alert, Box, Button, IconButton, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import { getErrorMessage } from '../../lib/errors';
import { formatCurrency, formatWeight } from '../../lib/format';
import { useInventory } from '../inventory/useInventory';
import { useLocations } from '../locations/useLocations';
import { useProducts } from '../products/useProducts';
import { useCreateWorkOrder } from './useCreateWorkOrder';

interface OutputForm {
  key: string;
  tagNumber: string;
  productId: string;
  locationId: string;
  weight: string;
}

function createOutput(): OutputForm {
  return { key: crypto.randomUUID(), tagNumber: '', productId: '', locationId: '', weight: '' };
}

export function CreateWorkOrderPage() {
  const navigate = useNavigate();
  const { data: inventory = [] } = useInventory('Available');
  const { data: products = [] } = useProducts();
  const { data: locations = [] } = useLocations();
  const createWorkOrder = useCreateWorkOrder();
  const [inputItemId, setInputItemId] = useState('');
  const [outputs, setOutputs] = useState<OutputForm[]>([createOutput()]);

  const inputs = inventory.filter((item) => !item.isScrap);
  const input = inputs.find((item) => item.id === Number(inputItemId));
  const outputWeight = outputs.reduce((total, output) => total + (Number(output.weight) || 0), 0);
  const remainingWeight = input ? Math.round((input.weight - outputWeight) * 10000) / 10000 : 0;

  const updateOutput = (key: string, changes: Partial<OutputForm>) => {
    setOutputs((current) => current.map((output) => (output.key === key ? { ...output, ...changes } : output)));
  };

  const removeOutput = (key: string) => {
    setOutputs((current) => current.filter((output) => output.key !== key));
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    createWorkOrder.mutate(
      {
        inputItemId: Number(inputItemId),
        outputs: outputs.map((output) => ({
          tagNumber: output.tagNumber,
          productId: Number(output.productId),
          locationId: Number(output.locationId),
          weight: Number(output.weight),
        })),
      },
      { onSuccess: (workOrder) => navigate(`/work-orders/${workOrder.id}`) },
    );
  };

  return (
    <Box component="form" onSubmit={handleSubmit}>
      <Typography variant="h4" sx={{ mb: 3 }}>New Work Order</Typography>
      <Stack spacing={3}>
        {createWorkOrder.isError && <Alert severity="error">{getErrorMessage(createWorkOrder.error)}</Alert>}
        <Paper sx={{ p: 3 }}>
          <Stack spacing={2}>
            <TextField select label="Input item" value={inputItemId} onChange={(event) => setInputItemId(event.target.value)} required>
              {inputs.map((item) => (
                <MenuItem key={item.id} value={item.id.toString()}>{item.tagNumber} · {item.productName} · {formatWeight(item.weight)}</MenuItem>
              ))}
            </TextField>
            {input && <Typography color="text.secondary">{input.locationName} · Cost {formatCurrency(input.totalCost)}</Typography>}
          </Stack>
        </Paper>
        <Paper sx={{ p: 3 }}>
          <Stack spacing={3}>
            <Typography variant="h6">Outputs</Typography>
            {outputs.map((output) => (
              <Box key={output.key} sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 2fr 1fr 1fr auto' }, gap: 2, alignItems: 'center' }}>
                <TextField label="Tag number" value={output.tagNumber} onChange={(event) => updateOutput(output.key, { tagNumber: event.target.value })} required />
                <TextField select label="Product" value={output.productId} onChange={(event) => updateOutput(output.key, { productId: event.target.value })} required>
                  {products.map((product) => (
                    <MenuItem key={product.id} value={product.id.toString()}>{product.name}</MenuItem>
                  ))}
                </TextField>
                <TextField select label="Location" value={output.locationId} onChange={(event) => updateOutput(output.key, { locationId: event.target.value })} required>
                  {locations.map((location) => (
                    <MenuItem key={location.id} value={location.id.toString()}>{location.name}</MenuItem>
                  ))}
                </TextField>
                <TextField label="Weight (lbs)" type="number" value={output.weight} onChange={(event) => updateOutput(output.key, { weight: event.target.value })} slotProps={{ htmlInput: { min: 0, step: 'any', inputMode: 'decimal' } }} required />
                <IconButton aria-label="Remove output" onClick={() => removeOutput(output.key)} disabled={outputs.length === 1} sx={{ width: 56, height: 56 }}>
                  <DeleteIcon />
                </IconButton>
              </Box>
            ))}
            <Box>
              <Button startIcon={<AddIcon />} onClick={() => setOutputs((current) => [...current, createOutput()])}>Add output</Button>
            </Box>
          </Stack>
        </Paper>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          {input && <Typography variant="h6" sx={{ mr: 'auto', color: remainingWeight === 0 ? 'success.main' : 'warning.main' }}>Remaining: {formatWeight(remainingWeight)}</Typography>}
          <Button component={Link} to="/work-orders" sx={{ ml: 'auto' }}>Cancel</Button>
          <Button type="submit" variant="contained" disabled={createWorkOrder.isPending}>Run work order</Button>
        </Stack>
      </Stack>
    </Box>
  );
}