import { useState, type ReactNode } from 'react';
import { Link, Outlet, useLocation } from 'react-router';
import { AppBar, Box, Button, Drawer, IconButton, List, ListItemButton, ListItemIcon, ListItemText, Toolbar, Typography, useMediaQuery, useTheme } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import AssessmentIcon from '@mui/icons-material/Assessment';
import CategoryIcon from '@mui/icons-material/Category';
import LocalShippingIcon from '@mui/icons-material/LocalShipping';
import PrecisionManufacturingIcon from '@mui/icons-material/PrecisionManufacturing';
import ReceiptLongIcon from '@mui/icons-material/ReceiptLong';
import { useAuth } from '../auth/useAuth';
import { useIsAdmin } from '../auth/useIsAdmin';

const drawerWidth = 240;

interface NavItem {
  label: string;
  path: string;
  icon: ReactNode;
  adminOnly?: boolean;
}

const navItems: NavItem[] = [
  { label: 'Purchase Orders', path: '/purchase-orders', icon: <ReceiptLongIcon /> },
  { label: 'Work Orders', path: '/work-orders', icon: <PrecisionManufacturingIcon /> },
  { label: 'Sales Orders', path: '/sales-orders', icon: <LocalShippingIcon /> },
  { label: 'Reports', path: '/reports', icon: <AssessmentIcon /> },
  { label: 'Products', path: '/products', icon: <CategoryIcon />, adminOnly: true },
];

export function AppLayout() {
  const { currentUser, signOut } = useAuth();
  const location = useLocation();
  const theme = useTheme();
  const isDesktop = useMediaQuery(theme.breakpoints.up('md'));
  const [mobileOpen, setMobileOpen] = useState(false);
  const isAdmin = useIsAdmin();
  const visibleNavItems = navItems.filter((item) => !item.adminOnly || isAdmin);

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (t) => t.zIndex.drawer + 1 }}>
        <Toolbar>
          {!isDesktop && (
            <IconButton color="inherit" edge="start" aria-label="Open navigation" onClick={() => setMobileOpen(true)} sx={{ mr: 2 }}>
              <MenuIcon />
            </IconButton>
          )}
          <Typography variant="h6" sx={{ flexGrow: 1 }}>Savage Metals</Typography>
          <Typography sx={{ mr: 2, display: { xs: 'none', sm: 'block' } }}>{currentUser?.displayName} · {currentUser?.role}</Typography>
          <Button color="inherit" onClick={signOut}>Sign out</Button>
        </Toolbar>
      </AppBar>
      <Box component="nav" sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }}>
        <Drawer variant={isDesktop ? 'permanent' : 'temporary'} open={isDesktop || mobileOpen} onClose={() => setMobileOpen(false)} sx={{ '& .MuiDrawer-paper': { width: drawerWidth, boxSizing: 'border-box' } }}>
          <Toolbar />
          <List>
            {visibleNavItems.map((item) => (
              <ListItemButton key={item.path} component={Link} to={item.path} selected={location.pathname.startsWith(item.path)} onClick={() => setMobileOpen(false)}>
                <ListItemIcon>{item.icon}</ListItemIcon>
                <ListItemText primary={item.label} />
              </ListItemButton>
            ))}
          </List>
        </Drawer>
      </Box>
      <Box component="main" sx={{ flexGrow: 1, p: 3, minWidth: 0 }}>
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
}