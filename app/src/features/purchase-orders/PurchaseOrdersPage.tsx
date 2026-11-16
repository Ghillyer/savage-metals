import { Link, useNavigate } from 'react-router';
import { Alert, Box, Button, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { formatCurrency, formatDateTime } from '../../lib/format';
import { usePurchaseOrders } from './usePurchaseOrders';
import { PurchaseOrderStatusChip } from './PurchaseOrdersStatusChip';

export function PurchaseOrdersPage() {
  const navigate = useNavigate();
  const { data: purchaseOrders, isPending, isError } = usePurchaseOrders();

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 3, alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h4">Purchase Orders</Typography>
        <Button component={Link} to="/purchase-orders/new" variant="contained" startIcon={<AddIcon />}>New purchase order</Button>
      </Stack>
      {isPending && <CircularProgress />}
      {isError && <Alert severity="error">Purchase orders couldn't be loaded.</Alert>}
      {purchaseOrders?.length === 0 && <Typography color="text.secondary">No purchase orders yet.</Typography>}
      {purchaseOrders && purchaseOrders.length > 0 && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>PO #</TableCell>
                <TableCell>Vendor</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Created</TableCell>
                <TableCell align="right">Total</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {purchaseOrders.map((purchaseOrder) => (
                <TableRow key={purchaseOrder.id} hover onClick={() => navigate(`/purchase-orders/${purchaseOrder.id}`)} sx={{ height: 56, cursor: 'pointer' }}>
                  <TableCell>{purchaseOrder.poNumber}</TableCell>
                  <TableCell>{purchaseOrder.vendorName}</TableCell>
                  <TableCell><PurchaseOrderStatusChip status={purchaseOrder.status} /></TableCell>
                  <TableCell>{formatDateTime(purchaseOrder.createdAtUtc)}</TableCell>
                  <TableCell align="right">{formatCurrency(purchaseOrder.total)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
}