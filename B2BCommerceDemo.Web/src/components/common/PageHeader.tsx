import { Box, Typography } from "@mui/material";
import type { ReactNode } from "react";

type PageHeaderProps = {
    title: string;
    subtitle?: string;
    action?: ReactNode;
};

const PageHeader = ({ title, subtitle, action }: PageHeaderProps) => {
    return (
        <Box
            sx={{
                mb: 4,
                display: "flex",
                flexDirection: { xs: "column", md: "row" },
                justifyContent: "space-between",
                alignItems: { xs: "flex-start", md: "center" },
                gap: 2,
            }}
        >
            <Box>
                <Typography
                    variant="h4"
                    component="h1"
                    sx={{ fontWeight: 800 }}
                >
                    {title}
                </Typography>

                {subtitle && (
                    <Typography
                        color="text.secondary"
                        sx={{ mt: 1, maxWidth: 720 }}
                    >
                        {subtitle}
                    </Typography>
                )}
            </Box>

            {action && <Box>{action}</Box>}
        </Box>
    );
};

export default PageHeader;