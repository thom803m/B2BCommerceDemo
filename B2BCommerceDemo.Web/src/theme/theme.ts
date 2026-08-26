import { createTheme } from "@mui/material/styles";

export const theme = createTheme({
    palette: {
        primary: {
            main: "#0F172A",
        },
        secondary: {
            main: "#2563EB",
        },
        background: {
            default: "#F8FAFC",
            paper: "#FFFFFF",
        },
    },
    typography: {
        fontFamily: "Inter, Arial, sans-serif",
        h1: {
            fontWeight: 700,
        },
        h2: {
            fontWeight: 700,
        },
        button: {
            textTransform: "none",
            fontWeight: 600,
        },
    },
    shape: {
        borderRadius: 12,
    },
});