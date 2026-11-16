import { Chip } from '@mui/material';
import type { PurchaseOrderStatus } from '../../types/purchaseOrder';

interface PurchaseOrderStatusChipProps {
  status: PurchaseOrderStatus;
}

export function PurchaseOrderStatusChip({ status }: PurchaseOrderStatusChipProps) {
  return <Chip label={status} color={status === 'Open' ? 'warning' : 'success'} size="small" />;
}