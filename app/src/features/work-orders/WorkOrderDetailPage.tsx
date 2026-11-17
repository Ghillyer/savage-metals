import { Link, useParams } from 'react-router';
import { Alert, Box, Button, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { DetailField } from '../../components/DetailField';
import { getErrorMessage } from '../../lib/errors';
import { formatCurrency, formatDateTime, formatWeight } from '../../lib/format';
import { InventoryStatusChip } from '../inventory/InventoryStatusChip';
import { useWorkOrder } from './useWorkOrder';

export function WorkOrderDetailPage() {
  const id = Number(useParams().id);
  const { data: workOrder, isPending, isError, error } = useWorkOrder(id);

  if (isPending) {
    return <CircularProgress />;
  }

  if (isError) {
    return <Alert severity="error">{getErrorMessage(error)}</Alert>;
  }

  const outputWeight = workOrder.outputs.reduce((total, output) => total + output.weight, 0);
  const outputCost = workOrder.outputs.reduce((total, output) => total + output.totalCost, 0);

  return (
    <Box>
      <Button component={Link} to="/work-orders" startIcon={<ArrowBackIcon />} sx={{ mb: 2 }}>Work orders</Button>
      <Typography variant="h4" sx={{ mb: 3 }}>{workOrder.workOrderNumber}</Typography>
      <Stack spacing={3}>
        <Paper sx={{ p: 3 }}>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(3, 1fr)' }, gap: 2 }}>
            <DetailField label="Input" value={`${workOrder.inputTagNumber} · ${workOrder.inputProductName}`} />
            <DetailField label="Input weight" value={formatWeight(workOrder.inputWeight)} />
            <DetailField label="Input cost" value={formatCurrency(workOrder.inputCost)} />
            <DetailField label="Created" value={formatDateTime(workOrder.createdAtUtc)} />
          </Box>
        </Paper>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Tag</TableCell>
                <TableCell>Product</TableCell>
                <TableCell>Location</TableCell>
                <TableCell align="right">Weight</TableCell>
                <TableCell align="right">Cost</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {workOrder.outputs.map((output) => (
                <TableRow key={output.id} sx={{ height: 56 }}>
                  <TableCell>{output.tagNumber}</TableCell>
                  <TableCell>{output.productName}{output.isScrap && ' (scrap)'}</TableCell>
                  <TableCell>{output.locationName}</TableCell>
                  <TableCell align="right">{formatWeight(output.weight)}</TableCell>
                  <TableCell align="right">{formatCurrency(output.totalCost)}</TableCell>
                  <TableCell><InventoryStatusChip status={output.status} /></TableCell>
                </TableRow>
              ))}
              <TableRow sx={{ height: 56 }}>
                <TableCell colSpan={3} align="right"><strong>Total</strong></TableCell>
                <TableCell align="right"><strong>{formatWeight(outputWeight)}</strong></TableCell>
                <TableCell align="right"><strong>{formatCurrency(outputCost)}</strong></TableCell>
                <TableCell />
              </TableRow>
            </TableBody>
          </Table>
        </TableContainer>
      </Stack>
    </Box>
  );
}