import { ArrowBack, DashboardOutlined, HomeOutlined, SearchOff, } from "@mui/icons-material";
import { Box, Button, Card, CardContent, Stack, Typography, } from "@mui/material";
import { Link, useLocation, useNavigate, } from "react-router-dom";

const NotFoundPage = () => {
    const location = useLocation();
    const navigate = useNavigate();

    const isAdminPage =
        location.pathname.startsWith(
            "/admin"
        );

    return (
        <Box
            sx={{
                minHeight: "60vh",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                py: 5,
            }}
        >
            <Card
                variant="outlined"
                sx={{
                    width: "100%",
                    maxWidth: 620,
                    borderRadius: 4,
                }}
            >
                <CardContent
                    sx={{
                        p: {
                            xs: 3,
                            sm: 5,
                        },
                        textAlign: "center",
                    }}
                >
                    <Box
                        sx={{
                            width: 80,
                            height: 80,
                            mx: "auto",
                            borderRadius: "50%",
                            display: "grid",
                            placeItems: "center",
                            bgcolor: "action.hover",
                            color: "text.secondary",
                        }}
                    >
                        <SearchOff
                            sx={{
                                fontSize: 44,
                            }}
                        />
                    </Box>

                    <Typography
                        variant="overline"
                        color="text.secondary"
                        sx={{
                            display: "block",
                            mt: 3,
                            fontWeight: 700,
                        }}
                    >
                        Error 404
                    </Typography>

                    <Typography
                        variant="h3"
                        component="h1"
                        sx={{
                            mt: 0.5,
                            fontWeight: 800,
                        }}
                    >
                        Page not found
                    </Typography>

                    <Typography
                        color="text.secondary"
                        sx={{
                            mt: 2,
                            maxWidth: 460,
                            mx: "auto",
                        }}
                    >
                        The page you requested does
                        not exist, may have been
                        moved or is no longer
                        available.
                    </Typography>

                    <Stack
                        direction={{
                            xs: "column",
                            sm: "row",
                        }}
                        spacing={1.5}
                        sx={{
                            mt: 4,
                            justifyContent: "center",
                        }}
                    >
                        <Button
                            variant="outlined"
                            startIcon={
                                <ArrowBack />
                            }
                            onClick={() =>
                                navigate(-1)
                            }
                        >
                            Go back
                        </Button>

                        <Button
                            component={Link}
                            to={
                                isAdminPage
                                    ? "/admin"
                                    : "/"
                            }
                            variant="contained"
                            startIcon={
                                isAdminPage ? (
                                    <DashboardOutlined />
                                ) : (
                                    <HomeOutlined />
                                )
                            }
                        >
                            {isAdminPage
                                ? "Admin dashboard"
                                : "Go to homepage"}
                        </Button>
                    </Stack>
                </CardContent>
            </Card>
        </Box>
    );
};

export default NotFoundPage;