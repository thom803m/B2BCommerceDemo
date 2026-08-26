import { Box, CircularProgress, Typography } from "@mui/material";

type LoadingSpinnerProps = {
    text?: string;
};

const LoadingSpinner = ({ text = "Loading..." }: LoadingSpinnerProps) => {
    return (
        <Box
            sx={{
                py: 8,
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                gap: 2,
            }}
        >
            <CircularProgress />
            <Typography color="text.secondary">{text}</Typography>
        </Box>
    );
};

export default LoadingSpinner;