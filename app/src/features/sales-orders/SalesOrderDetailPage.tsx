import { Link, useParams } from 'react-router';
import { Alert, Box, Button, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { DetailField } from '../../components/DetailField';
import { getErrorMessage } from '../../lib/errors';
import { formatCurrency, formatDateTime, formatPercent, formatUnitPrice, formatWeight } from '../../lib/format';
import { useSalesOrder } from './useSalesOrder';

export function SalesOrderDetailPage() {
  const id = Number(useParams().id);
  const { data: salesOrder, isPending, isError, error } = useSalesOrder(id);

  if (isPending) {
    return <CircularProgress />;
  }

  if (isError) {
    return <Alert severity="error">{getErrorMessage(error)}</Alert>;
  }

  return (
    <Box>
      <Button component={Link} to="/sales-orders" startIcon={<ArrowBackIcon />} sx={{ mb: 2 }}>Sales orders</Button>
      <Typography variant="h4" sx={{ mb: 3 }}>SO #{salesOrder.soNumber}</Typography>
      <Stack spacing={3}>
        <Paper sx={{ p: 3 }}>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(4, 1fr)' }, gap: 2 }}>
            <DetailField label="Customer" value={salesOrder.customerName} />
            <DetailField label="Container" value={salesOrder.containerNumber} />
            <DetailField label="Shipped" value={formatDateTime(salesOrder.createdAtUtc)} />
            <DetailField label="Total weight" value={formatWeight(salesOrder.totalWeight)} />
            <DetailField label="Revenue" value={formatCurrency(salesOrder.revenue)} />
            <DetailField label="Cost" value={formatCurrency(salesOrder.cost)} />
            <DetailField label="Profit" value={formatCurrency(salesOrder.profit)} />
            <DetailField label="Margin" value={formatPercent(salesOrder.margin)} />
          </Box>
        </Paper>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Tag</TableCell>
                <TableCell>Product</TableCell>
                <TableCell align="right">Weight</TableCell>
                <TableCell align="right">Price per lb</TableCell>
                <TableCell align="right">Revenue</TableCell>
                <TableCell align="right">Cost</TableCell>
                <TableCell align="right">Profit</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {salesOrder.lines.map((line) => (
                <TableRow key={line.id} sx={{ height: 56 }}>
                  <TableCell>{line.tagNumber}</TableCell>
                  <TableCell>{line.productName}</TableCell>
                  <TableCell align="right">{formatWeight(line.weight)}</TableCell>
                  <TableCell align="right">{formatUnitPrice(line.unitPrice)}</TableCell>
                  <TableCell align="right">{formatCurrency(line.revenue)}</TableCell>
                  <TableCell align="right">{formatCurrency(line.cost)}</TableCell>
                  <TableCell align="right">{formatCurrency(line.profit)}</TableCell>
                </TableRow>
              ))}
              <TableRow sx={{ height: 56 }}>
                <TableCell colSpan={2} align="right"><strong>Total</strong></TableCell>
                <TableCell align="right"><strong>{formatWeight(salesOrder.totalWeight)}</strong></TableCell>
                <TableCell />
                <TableCell align="right"><strong>{formatCurrency(salesOrder.revenue)}</strong></TableCell>
                <TableCell align="right"><strong>{formatCurrency(salesOrder.cost)}</strong></TableCell>
                <TableCell align="right"><strong>{formatCurrency(salesOrder.profit)}</strong></TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </TableContainer>
      </Stack>
    </Box>
  );
}