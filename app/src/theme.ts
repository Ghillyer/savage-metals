import { createTheme } from '@mui/material/styles';

const brand = {
  shell: '#050816',
  page: '#0A0F22',
  surface: '#121A33',
  surfaceHeader: '#0F1730',
  border: 'rgba(148, 163, 184, 0.16)',
  blue: '#0F8BFF',
  brightBlue: '#3AA8FF',
  cyan: '#7FE3FF',
  orange: '#F5A623',
};

const headingFont = '"DM Sans Variable", "Segoe UI", sans-serif';

export const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: brand.brightBlue,
      light: brand.cyan,
      dark: brand.blue,
      contrastText: brand.shell,
    },
    secondary: {
      main: brand.orange,
      contrastText: brand.shell,
    },
    background: {
      default: brand.page,
      paper: brand.surface,
    },
    text: {
      primary: '#E6EAF2',
      secondary: '#9AA4BF',
    },
    divider: brand.border,
  },
  shape: {
    borderRadius: 10,
  },
  typography: {
    fontSize: 16,
    fontFamily: '"Manrope Variable", "Segoe UI", sans-serif',
    h4: { fontFamily: headingFont, fontWeight: 600 },
    h5: { fontFamily: headingFont, fontWeight: 600 },
    h6: { fontFamily: headingFont, fontWeight: 600 },
  },
  components: {
    MuiButton: {
      defaultProps: {
        size: 'large',
        disableElevation: true,
      },
      styleOverrides: {
        root: {
          minHeight: 56,
          fontSize: '1.1rem',
          fontWeight: 600,
          textTransform: 'none',
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          border: `1px solid ${brand.border}`,
        },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundColor: brand.shell,
          border: 'none',
          borderBottom: '1px solid rgba(127, 227, 255, 0.15)',
        },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          backgroundColor: brand.shell,
          border: 'none',
          borderRight: `1px solid ${brand.border}`,
        },
      },
    },
    MuiListItemButton: {
      styleOverrides: {
        root: {
          minHeight: 56,
          '& .MuiListItemIcon-root': {
            color: 'rgba(230, 234, 242, 0.6)',
          },
          '&:hover': {
            backgroundColor: 'rgba(255, 255, 255, 0.06)',
          },
          '&.Mui-selected': {
            backgroundColor: 'rgba(15, 139, 255, 0.18)',
            color: brand.cyan,
            '& .MuiListItemIcon-root': {
              color: brand.cyan,
            },
          },
          '&.Mui-selected:hover': {
            backgroundColor: 'rgba(15, 139, 255, 0.26)',
          },
        },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        root: {
          borderBottomColor: brand.border,
        },
        head: {
          fontWeight: 600,
          color: '#9AA4BF',
          backgroundColor: brand.surfaceHeader,
        },
      },
    },
    MuiTextField: {
      defaultProps: {
        fullWidth: true,
      },
    },
  },
});