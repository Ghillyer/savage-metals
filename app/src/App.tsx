import { Navigate, Route, Routes } from 'react-router';
import { LoginPage } from './auth/LoginPage';
import { RequireAdmin } from './auth/RequireAdmin';
import { RequireAuth } from './auth/RequireAuth';
import { AppLayout } from './components/AppLayout';
import { CreatePurchaseOrderPage } from './features/purchase-orders/CreatePurchaseOrderPage';
import { PurchaseOrderDetailPage } from './features/purchase-orders/PurchaseOrderDetailPage';
import { ProductsPage } from './features/products/ProductsPage';
import { PurchaseOrdersPage } from './features/purchase-orders/PurchaseOrdersPage';
import { ReportsPage } from './features/reports/ReportsPage';
import { CreateSalesOrderPage } from './features/sales-orders/CreateSalesOrderPage';
import { SalesOrderDetailPage } from './features/sales-orders/SalesOrderDetailPage';
import { SalesOrdersPage } from './features/sales-orders/SalesOrdersPage';
import { CreateWorkOrderPage } from './features/work-orders/CreateWorkOrderPage';
import { WorkOrderDetailPage } from './features/work-orders/WorkOrderDetailPage';
import { WorkOrdersPage } from './features/work-orders/WorkOrdersPage';

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<RequireAuth />}>
        <Route element={<AppLayout />}>
          <Route path="/" element={<Navigate to="/purchase-orders" replace />} />
          <Route path="/purchase-orders" element={<PurchaseOrdersPage />} />
          <Route path="/purchase-orders/new" element={<CreatePurchaseOrderPage />} />
          <Route path="/purchase-orders/:id" element={<PurchaseOrderDetailPage />} />
          <Route path="/work-orders" element={<WorkOrdersPage />} />
          <Route path="/work-orders/new" element={<CreateWorkOrderPage />} />
          <Route path="/work-orders/:id" element={<WorkOrderDetailPage />} />
          <Route path="/sales-orders" element={<SalesOrdersPage />} />
          <Route path="/sales-orders/new" element={<CreateSalesOrderPage />} />
          <Route path="/sales-orders/:id" element={<SalesOrderDetailPage />} />
          <Route path="/reports" element={<ReportsPage />} />
          <Route element={<RequireAdmin />}>
            <Route path="/products" element={<ProductsPage />} />
          </Route>
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}