import { useState } from 'react';
import { Alert, Box, Button, Chip, CircularProgress, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { formatUnitPrice } from '../../lib/format';
import type { Product } from '../../types/product';
import { ProductDialog } from './ProductDialog';
import { useProducts } from './useProducts';

function formatPrice(value: number | null) {
  return value === null ? '—' : formatUnitPrice(value);
}

export function ProductsPage() {
  const { data: products, isPending, isError } = useProducts(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product>();

  const openDialog = (product?: Product) => {
    setSelectedProduct(product);
    setDialogOpen(true);
  };

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 3, alignItems: 'center', justifyContent: 'space-between' }}>
        <Typography variant="h4">Products</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => openDialog()}>New product</Button>
      </Stack>
      {isPending && <CircularProgress />}
      {isError && <Alert severity="error">Products couldn't be loaded.</Alert>}
      {products && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell align="right">Purchase price</TableCell>
                <TableCell align="right">Sales price</TableCell>
                <TableCell>Scrap</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {products.map((product) => (
                <TableRow key={product.id} hover onClick={() => openDialog(product)} sx={{ height: 56, cursor: 'pointer' }}>
                  <TableCell>{product.name}</TableCell>
                  <TableCell align="right">{formatPrice(product.purchasePrice)}</TableCell>
                  <TableCell align="right">{formatPrice(product.salesPrice)}</TableCell>
                  <TableCell>{product.isScrap ? 'Yes' : ''}</TableCell>
                  <TableCell><Chip label={product.isActive ? 'Active' : 'Inactive'} color={product.isActive ? 'success' : 'default'} size="small" /></TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      {dialogOpen && <ProductDialog product={selectedProduct} onClose={() => setDialogOpen(false)} />}
    </Box>
  );
}