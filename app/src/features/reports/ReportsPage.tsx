import { Alert, Box, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import { DetailField } from '../../components/DetailField';
import { formatCurrency, formatPercent, formatWeight } from '../../lib/format';
import type { InventoryReportRow } from '../../types/report';
import { useSalesOrders } from '../sales-orders/useSalesOrders';
import { useInventoryReport } from './useInventoryReport';

interface InventoryReportTableProps {
  title: string;
  label: string;
  rows: InventoryReportRow[];
}

function InventoryReportTable({ title, label, rows }: InventoryReportTableProps) {
  return (
    <Box>
      <Typography variant="h6" sx={{ mb: 1 }}>{title}</Typography>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{label}</TableCell>
              <TableCell align="right">Items</TableCell>
              <TableCell align="right">Weight</TableCell>
              <TableCell align="right">Value</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.name} sx={{ height: 56 }}>
                <TableCell>{row.name}</TableCell>
                <TableCell align="right">{row.itemCount}</TableCell>
                <TableCell align="right">{formatWeight(row.totalWeight)}</TableCell>
                <TableCell align="right">{formatCurrency(row.totalValue)}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
}

export function ReportsPage() {
  const { data: report, isPending, isError } = useInventoryReport();
  const { data: salesOrders = [] } = useSalesOrders();

  const revenue = salesOrders.reduce((total, salesOrder) => total + salesOrder.revenue, 0);
  const cost = salesOrders.reduce((total, salesOrder) => total + salesOrder.cost, 0);
  const profit = revenue - cost;
  const margin = revenue === 0 ? 0 : profit / revenue;

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3 }}>Reports</Typography>
      {isPending && <CircularProgress />}
      {isError && <Alert severity="error">The inventory report couldn't be loaded.</Alert>}
      {report && (
        <Stack spacing={4}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>Inventory on hand</Typography>
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(3, 1fr)' }, gap: 2 }}>
              <DetailField label="Items" value={report.itemCount.toString()} />
              <DetailField label="Weight" value={formatWeight(report.totalWeight)} />
              <DetailField label="Value at cost" value={formatCurrency(report.totalValue)} />
            </Box>
          </Paper>
          <InventoryReportTable title="Inventory by product" label="Product" rows={report.byProduct} />
          <InventoryReportTable title="Inventory by location" label="Location" rows={report.byLocation} />
          <Box>
            <Typography variant="h6" sx={{ mb: 1 }}>Sales profit</Typography>
            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>SO #</TableCell>
                    <TableCell>Customer</TableCell>
                    <TableCell align="right">Revenue</TableCell>
                    <TableCell align="right">Cost</TableCell>
                    <TableCell align="right">Profit</TableCell>
                    <TableCell align="right">Margin</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {salesOrders.map((salesOrder) => (
                    <TableRow key={salesOrder.id} sx={{ height: 56 }}>
                      <TableCell>{salesOrder.soNumber}</TableCell>
                      <TableCell>{salesOrder.customerName}</TableCell>
                      <TableCell align="right">{formatCurrency(salesOrder.revenue)}</TableCell>
                      <TableCell align="right">{formatCurrency(salesOrder.cost)}</TableCell>
                      <TableCell align="right">{formatCurrency(salesOrder.profit)}</TableCell>
                      <TableCell align="right">{formatPercent(salesOrder.margin)}</TableCell>
                    </TableRow>
                  ))}
                  <TableRow sx={{ height: 56 }}>
                    <TableCell colSpan={2} align="right"><strong>Total</strong></TableCell>
                    <TableCell align="right"><strong>{formatCurrency(revenue)}</strong></TableCell>
                    <TableCell align="right"><strong>{formatCurrency(cost)}</strong></TableCell>
                    <TableCell align="right"><strong>{formatCurrency(profit)}</strong></TableCell>
                    <TableCell align="right"><strong>{formatPercent(margin)}</strong></TableCell>
                  </TableRow>
                </TableBody>
              </Table>
            </TableContainer>
          </Box>
        </Stack>
      )}
    </Box>
  );
}