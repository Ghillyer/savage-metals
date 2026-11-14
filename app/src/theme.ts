import { createTheme } from '@mui/material/styles';

export const theme = createTheme({
  typography: {
    fontSize: 16,
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
          textTransform: 'none',
        },
      },
    },
    MuiTextField: {
      defaultProps: {
        fullWidth: true,
      },
    },
    MuiListItemButton: {
      styleOverrides: {
        root: {
          minHeight: 56,
        },
      },
    },
  },
});