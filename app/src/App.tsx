import { Navigate, Route, Routes } from 'react-router';
import { LoginPage } from './auth/LoginPage';
import { RequireAuth } from './auth/RequireAuth';
import { AppLayout } from './components/AppLayout';
import { CreatePurchaseOrderPage } from './features/purchase-orders/CreatePurchaseOrderPage';
import { PurchaseOrderDetailPage } from './features/purchase-orders/PurchaseOrderDetailPage';
import { PurchaseOrdersPage } from './features/purchase-orders/PurchaseOrdersPage';
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
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}