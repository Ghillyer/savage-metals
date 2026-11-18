import { Link, useNavigate } from 'react-router';
import { Alert, Box, Button, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { formatDateTime, formatWeight } from '../../lib/format';
import { useWorkOrders } from './useWorkOrders';

export function WorkOrdersPage() {
  const navigate = useNavigate();
  const { data: workOrders, isPending, isError } = useWorkOrders();

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 3, alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h4">Work Orders</Typography>
        <Button component={Link} to="/work-orders/new" variant="contained" startIcon={<AddIcon />}>New work order</Button>
      </Stack>
      {isPending && <CircularProgress />}
      {isError && <Alert severity="error">Work orders couldn't be loaded.</Alert>}
      {workOrders?.length === 0 && <Typography color="text.secondary">No work orders yet.</Typography>}
      {workOrders && workOrders.length > 0 && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Work order</TableCell>
                <TableCell>Input</TableCell>
                <TableCell align="right">Input weight</TableCell>
                <TableCell align="right">Outputs</TableCell>
                <TableCell>Created</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {workOrders.map((workOrder) => (
                <TableRow key={workOrder.id} hover onClick={() => navigate(`/work-orders/${workOrder.id}`)} sx={{ height: 56, cursor: 'pointer' }}>
                  <TableCell>{workOrder.workOrderNumber}</TableCell>
                  <TableCell>{workOrder.inputTagNumber} · {workOrder.inputProductName}</TableCell>
                  <TableCell align="right">{formatWeight(workOrder.inputWeight)}</TableCell>
                  <TableCell align="right">{workOrder.outputCount}</TableCell>
                  <TableCell sx={{ whiteSpace: 'nowrap' }}>{formatDateTime(workOrder.createdAtUtc)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
}