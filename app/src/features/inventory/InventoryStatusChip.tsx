import { Chip } from '@mui/material';
import type { InventoryStatus } from '../../types/inventory';

interface InventoryStatusChipProps {
  status: InventoryStatus;
}

const colors = {
  Available: 'success',
  Consumed: 'default',
  Sold: 'info',
} as const;

export function InventoryStatusChip({ status }: InventoryStatusChipProps) {
  return <Chip label={status} color={colors[status]} size="small" />;
}