import { Inventory2, } from "@mui/icons-material";
import { Box, Typography, } from "@mui/material";
import type { ReactNode, } from "react";

type EmptyStateProps = {
    title: string;
    description?: string;
    action?: ReactNode;
    icon?: ReactNode;
};

const EmptyState = ({
    title,
    description,
    action,
    icon,
}: EmptyStateProps) => {
    return (
        <Box
            sx={{
                py: 8,
                px: 3,
                textAlign: "center",
                border: "1px dashed",
                borderColor: "divider",
                borderRadius: 3,
                bgcolor: "background.paper",
            }}
        >
            <Box
                sx={{
                    mb: 2,
                    color: "grey.400",

                    "& svg": {
                        fontSize: 56,
                    },
                }}
            >
                {icon ?? <Inventory2 />}
            </Box>

            <Typography
                variant="h6"
                component="h2"
                sx={{ fontWeight: 800 }}
            >
                {title}
            </Typography>

            {description && (
                <Typography
                    color="text.secondary"
                    sx={{
                        mt: 1,
                        mb: action ? 3 : 0,
                    }}
                >
                    {description}
                </Typography>
            )}

            {action}
        </Box>
    );
};

export default EmptyState;