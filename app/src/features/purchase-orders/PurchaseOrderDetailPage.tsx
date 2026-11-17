import { useState, type FormEvent } from 'react';
import { Link, useParams } from 'react-router';
import { Alert, Box, Button, CircularProgress, MenuItem, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { DetailField } from '../../components/DetailField';
import { getErrorMessage } from '../../lib/errors';
import { formatCurrency, formatDateTime, formatUnitPrice, formatWeight } from '../../lib/format';
import { useLocations } from '../locations/useLocations';
import { usePurchaseOrder } from './usePurchaseOrder';
import { useReceivePurchaseOrder } from './useReceivePurchaseOrder';
import { PurchaseOrderStatusChip } from './PurchaseOrdersStatusChip';

interface ReceiveLineForm {
  tagNumber: string;
  locationId: string;
}

const emptyReceiveLine: ReceiveLineForm = { tagNumber: '', locationId: '' };

export function PurchaseOrderDetailPage() {
  const id = Number(useParams().id);
  const { data: purchaseOrder, isPending, isError, error } = usePurchaseOrder(id);
  const { data: locations = [] } = useLocations();
  const receivePurchaseOrder = useReceivePurchaseOrder(id);
  const [receiveLines, setReceiveLines] = useState<Record<number, ReceiveLineForm>>({});

  if (isPending) {
    return <CircularProgress />;
  }

  if (isError) {
    return <Alert severity="error">{getErrorMessage(error)}</Alert>;
  }

  const getReceiveLine = (lineId: number) => receiveLines[lineId] ?? emptyReceiveLine;

  const updateReceiveLine = (lineId: number, changes: Partial<ReceiveLineForm>) => {
    setReceiveLines((current) => ({ ...current, [lineId]: { ...(current[lineId] ?? emptyReceiveLine), ...changes } }));
  };

  const handleReceive = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    receivePurchaseOrder.mutate({
      lines: purchaseOrder.lines.map((line) => ({
        purchaseOrderLineId: line.id,
        tagNumber: getReceiveLine(line.id).tagNumber,
        locationId: Number(getReceiveLine(line.id).locationId),
      })),
    });
  };

  return (
    <Box>
      <Button component={Link} to="/purchase-orders" startIcon={<ArrowBackIcon />} sx={{ mb: 2 }}>Purchase orders</Button>
      <Stack direction="row" spacing={2} sx={{ mb: 3, alignItems: 'center' }}>
        <Typography variant="h4">PO #{purchaseOrder.poNumber}</Typography>
        <PurchaseOrderStatusChip status={purchaseOrder.status} />
      </Stack>
      <Stack spacing={3}>
        <Paper sx={{ p: 3 }}>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(3, 1fr)' }, gap: 2 }}>
            <DetailField label="Vendor" value={purchaseOrder.vendorName} />
            <DetailField label="Created" value={formatDateTime(purchaseOrder.createdAtUtc)} />
            <DetailField label="Received" value={purchaseOrder.receivedAtUtc ? formatDateTime(purchaseOrder.receivedAtUtc) : 'Not yet received'} />
          </Box>
        </Paper>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Product</TableCell>
                <TableCell>Grade</TableCell>
                <TableCell align="right">Weight</TableCell>
                <TableCell align="right">Price per lb</TableCell>
                <TableCell align="right">Total</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {purchaseOrder.lines.map((line) => (
                <TableRow key={line.id} sx={{ height: 56 }}>
                  <TableCell>{line.productName}</TableCell>
                  <TableCell>{line.grade}</TableCell>
                  <TableCell align="right">{formatWeight(line.weight)}</TableCell>
                  <TableCell align="right">{formatUnitPrice(line.unitPrice)}</TableCell>
                  <TableCell align="right">{formatCurrency(line.lineTotal)}</TableCell>
                </TableRow>
              ))}
              <TableRow sx={{ height: 56 }}>
                <TableCell colSpan={4} align="right"><strong>Total</strong></TableCell>
                <TableCell align="right"><strong>{formatCurrency(purchaseOrder.total)}</strong></TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </TableContainer>
        {purchaseOrder.status === 'Open' && (
          <Paper sx={{ p: 3 }}>
            <Box component="form" onSubmit={handleReceive}>
              <Stack spacing={3}>
                <Typography variant="h6">Receive</Typography>
                {receivePurchaseOrder.isError && <Alert severity="error">{getErrorMessage(receivePurchaseOrder.error)}</Alert>}
                {purchaseOrder.lines.map((line) => (
                  <Box key={line.id} sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '2fr 1fr 1fr' }, gap: 2, alignItems: 'center' }}>
                    <Box>
                      <Typography>{line.productName}</Typography>
                      <Typography variant="body2" color="text.secondary">{line.grade} · {formatWeight(line.weight)}</Typography>
                    </Box>
                    <TextField label="Tag number" value={getReceiveLine(line.id).tagNumber} onChange={(event) => updateReceiveLine(line.id, { tagNumber: event.target.value })} required />
                    <TextField select label="Location" value={getReceiveLine(line.id).locationId} onChange={(event) => updateReceiveLine(line.id, { locationId: event.target.value })} required>
                      {locations.map((location) => (
                        <MenuItem key={location.id} value={location.id.toString()}>{location.name}</MenuItem>
                      ))}
                    </TextField>
                  </Box>
                ))}
                <Box>
                  <Button type="submit" variant="contained" disabled={receivePurchaseOrder.isPending}>Receive purchase order</Button>
                </Box>
              </Stack>
            </Box>
          </Paper>
        )}
      </Stack>
    </Box>
  );
}