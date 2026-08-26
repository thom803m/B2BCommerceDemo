import { AccessTimeOutlined, EmailOutlined, LocationOnOutlined, PhoneOutlined, SupportAgentOutlined, } from "@mui/icons-material";
import { Alert, Box, Button, Card, CardContent, Grid, Stack, Typography, } from "@mui/material";

const ContactPage = () => {
    return (
        <Box
            sx={{
                py: {
                    xs: 4,
                    md: 7,
                },
            }}
        >
            <Box
                sx={{
                    maxWidth: 720,
                    mx: "auto",
                    mb: {
                        xs: 4,
                        md: 6,
                    },
                    textAlign: "center",
                }}
            >
                <Box
                    sx={{
                        width: 72,
                        height: 72,
                        mx: "auto",
                        mb: 2.5,
                        borderRadius: "50%",
                        display: "grid",
                        placeItems: "center",
                        bgcolor: "primary.50",
                        color: "primary.main",
                    }}
                >
                    <SupportAgentOutlined
                        sx={{ fontSize: 40 }}
                    />
                </Box>

                <Typography
                    variant="h3"
                    component="h1"
                    sx={{ fontWeight: 800 }}
                >
                    Contact us
                </Typography>

                <Typography
                    color="text.secondary"
                    sx={{
                        mt: 1.5,
                        fontSize: {
                            xs: "1rem",
                            sm: "1.1rem",
                        },
                    }}
                >
                    Have a question about products,
                    pricing, orders or your company
                    account? Our customer service team
                    is ready to help.
                </Typography>
            </Box>

            <Alert
                severity="info"
                sx={{
                    maxWidth: 960,
                    mx: "auto",
                    mb: 3,
                }}
            >
                The contact information shown on this
                demonstration page is fictional.
            </Alert>

            <Grid
                container
                spacing={3}
                sx={{
                    maxWidth: 960,
                    mx: "auto",
                }}
            >
                <Grid size={{ xs: 12, md: 6 }}>
                    <Card
                        variant="outlined"
                        sx={{
                            height: "100%",
                            borderRadius: 4,
                        }}
                    >
                        <CardContent
                            sx={{
                                p: {
                                    xs: 3,
                                    sm: 4,
                                },
                            }}
                        >
                            <Stack spacing={3}>
                                <Box>
                                    <Typography
                                        variant="h5"
                                        component="h2"
                                        sx={{
                                            fontWeight: 800,
                                        }}
                                    >
                                        Customer service
                                    </Typography>

                                    <Typography
                                        color="text.secondary"
                                        sx={{ mt: 0.75 }}
                                    >
                                        Contact us if you
                                        need help with an
                                        order, product or
                                        your webshop account.
                                    </Typography>
                                </Box>

                                <Stack
                                    direction="row"
                                    spacing={2}
                                    sx={{
                                        alignItems: "flex-start",
                                    }}
                                >
                                    <EmailOutlined
                                        color="primary"
                                    />

                                    <Box>
                                        <Typography
                                            sx={{
                                                fontWeight: 700,
                                            }}
                                        >
                                            Email
                                        </Typography>

                                        <Button
                                            component="a"
                                            href="mailto:support@example.com"
                                            sx={{
                                                p: 0,
                                                minWidth:
                                                    "auto",
                                                textTransform:
                                                    "none",
                                            }}
                                        >
                                            support@example.com
                                        </Button>
                                    </Box>
                                </Stack>

                                <Stack
                                    direction="row"
                                    spacing={2}
                                    sx={{
                                        alignItems: "flex-start",
                                    }}
                                >
                                    <PhoneOutlined
                                        color="primary"
                                    />

                                    <Box>
                                        <Typography
                                            sx={{
                                                fontWeight: 700,
                                            }}
                                        >
                                            Telephone
                                        </Typography>

                                        <Button
                                            component="a"
                                            href="tel:+4512345678"
                                            sx={{
                                                p: 0,
                                                minWidth:
                                                    "auto",
                                                textTransform:
                                                    "none",
                                            }}
                                        >
                                            +45 12 34 56 78
                                        </Button>
                                    </Box>
                                </Stack>

                                <Stack
                                    direction="row"
                                    spacing={2}
                                    sx={{
                                        alignItems: "flex-start",
                                    }}
                                >
                                    <AccessTimeOutlined
                                        color="primary"
                                    />

                                    <Box>
                                        <Typography
                                            sx={{
                                                fontWeight: 700,
                                            }}
                                        >
                                            Opening hours
                                        </Typography>

                                        <Typography color="text.secondary">
                                            Monday–Thursday:
                                            08:00–16:00
                                        </Typography>

                                        <Typography color="text.secondary">
                                            Friday:
                                            08:00–15:00
                                        </Typography>
                                    </Box>
                                </Stack>
                            </Stack>
                        </CardContent>
                    </Card>
                </Grid>

                <Grid size={{ xs: 12, md: 6 }}>
                    <Card
                        variant="outlined"
                        sx={{
                            height: "100%",
                            borderRadius: 4,
                        }}
                    >
                        <CardContent
                            sx={{
                                p: {
                                    xs: 3,
                                    sm: 4,
                                },
                            }}
                        >
                            <Stack spacing={3}>
                                <Box>
                                    <Typography
                                        variant="h5"
                                        component="h2"
                                        sx={{
                                            fontWeight: 800,
                                        }}
                                    >
                                        B2B Commerce Demo
                                    </Typography>

                                    <Typography
                                        color="text.secondary"
                                        sx={{ mt: 0.75 }}
                                    >
                                        Fictional company
                                        information for the
                                        webshop demonstration.
                                    </Typography>
                                </Box>

                                <Stack
                                    direction="row"
                                    spacing={2}
                                    sx={{
                                        alignItems: "flex-start",
                                    }}
                                >
                                    <LocationOnOutlined
                                        color="primary"
                                    />

                                    <Box>
                                        <Typography
                                            sx={{
                                                fontWeight: 700,
                                            }}
                                        >
                                            Address
                                        </Typography>

                                        <Typography color="text.secondary">
                                            Webshopvej 10
                                        </Typography>

                                        <Typography color="text.secondary">
                                            8000 Aarhus C
                                        </Typography>

                                        <Typography color="text.secondary">
                                            Denmark
                                        </Typography>
                                    </Box>
                                </Stack>

                                <Box>
                                    <Typography
                                        sx={{
                                            fontWeight: 700,
                                        }}
                                    >
                                        Company registration
                                    </Typography>

                                    <Typography color="text.secondary">
                                        CVR: 12 34 56 78
                                    </Typography>
                                </Box>

                                <Alert severity="success">
                                    For order-related
                                    questions, please include
                                    your order number when
                                    contacting customer
                                    service.
                                </Alert>
                            </Stack>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>
        </Box>
    );
};

export default ContactPage;