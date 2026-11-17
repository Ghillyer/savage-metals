import { Link, useNavigate } from 'react-router';
import { Alert, Box, Button, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { formatCurrency, formatDateTime, formatPercent } from '../../lib/format';
import { useSalesOrders } from './useSalesOrders';

export function SalesOrdersPage() {
  const navigate = useNavigate();
  const { data: salesOrders, isPending, isError } = useSalesOrders();

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 3, alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h4">Sales Orders</Typography>
        <Button component={Link} to="/sales-orders/new" variant="contained" startIcon={<AddIcon />}>New sales order</Button>
      </Stack>
      {isPending && <CircularProgress />}
      {isError && <Alert severity="error">Sales orders couldn't be loaded.</Alert>}
      {salesOrders?.length === 0 && <Typography color="text.secondary">No sales orders yet.</Typography>}
      {salesOrders && salesOrders.length > 0 && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>SO #</TableCell>
                <TableCell>Customer</TableCell>
                <TableCell align="right">Items</TableCell>
                <TableCell align="right">Revenue</TableCell>
                <TableCell align="right">Profit</TableCell>
                <TableCell align="right">Margin</TableCell>
                <TableCell>Created</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {salesOrders.map((salesOrder) => (
                <TableRow key={salesOrder.id} hover onClick={() => navigate(`/sales-orders/${salesOrder.id}`)} sx={{ height: 56, cursor: 'pointer' }}>
                  <TableCell>{salesOrder.soNumber}</TableCell>
                  <TableCell>{salesOrder.customerName}</TableCell>
                  <TableCell align="right">{salesOrder.itemCount}</TableCell>
                  <TableCell align="right">{formatCurrency(salesOrder.revenue)}</TableCell>
                  <TableCell align="right">{formatCurrency(salesOrder.profit)}</TableCell>
                  <TableCell align="right">{formatPercent(salesOrder.margin)}</TableCell>
                  <TableCell>{formatDateTime(salesOrder.createdAtUtc)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
}