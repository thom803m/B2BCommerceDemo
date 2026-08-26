import {
    Business,
    DescriptionOutlined,
    Groups,
    IntegrationInstructions,
    Inventory2,
    OpenInNew,
    PendingActions,
    PriceChange,
    ReceiptLong,
    Refresh,
} from "@mui/icons-material";
import {
    Alert,
    Box,
    Button,
    Card,
    CardActions,
    CardContent,
    Grid,
    Stack,
    Typography,
} from "@mui/material";
import {
    useCallback,
    useEffect,
    useState,
} from "react";
import { Link } from "react-router-dom";
import {
    getAdminDashboardSummary,
    type AdminDashboardSummary,
} from "../../api/dashboardApi";
import LoadingSpinner from "../../components/common/LoadingSpinner";
import PageHeader from "../../components/common/PageHeader";

const administrationAreas = [
    {
        title: "Products",
        description:
            "Manage the product catalogue, stock information and product content.",
        icon: <Inventory2 />,
        path: "/admin/products",
    },
    {
        title: "Companies",
        description:
            "Review registrations, company statuses and assigned price groups.",
        icon: <Business />,
        path: "/admin/companies",
    },
    {
        title: "Orders",
        description:
            "Review customer orders and Rackbeat synchronization information.",
        icon: <ReceiptLong />,
        path: "/admin/orders",
    },
    {
        title: "Pricing",
        description:
            "Manage price groups and company-specific product prices.",
        icon: <PriceChange />,
        path: "/admin/pricing",
    },
    {
        title: "Integrations",
        description:
            "Run Rackbeat synchronization and Icecat enrichment tasks.",
        icon: <IntegrationInstructions />,
        path: "/admin/integrations",
    },
];

const AdminDashboardPage = () => {
    const [summary, setSummary] =
        useState<AdminDashboardSummary | null>(
            null
        );

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    const loadDashboard =
        useCallback(async () => {
            setLoading(true);
            setError(null);

            try {
                const result =
                    await getAdminDashboardSummary();

                setSummary(result);
            } catch (error) {
                console.error(
                    "Failed to load admin dashboard",
                    error
                );

                setError(
                    "The dashboard figures could not be loaded. Please try again."
                );
            } finally {
                setLoading(false);
            }
        }, []);

    useEffect(() => {
        void loadDashboard();
    }, [loadDashboard]);

    const dashboardMetrics = summary
        ? [
            {
                title: "Pending companies",
                value:
                    summary.pendingCompanies,
                description:
                    "Waiting for approval",
                icon: <PendingActions />,
                color: "warning.main",
                backgroundColor:
                    "warning.50",
                path: "/admin/companies#pending-companies",
            },
            {
                title: "Active companies",
                value:
                    summary.activeCompanies,
                description:
                    "Approved business customers",
                icon: <Groups />,
                color: "success.main",
                backgroundColor:
                    "success.50",
                path: "/admin/companies#managed-companies",
            },
            {
                title: "Orders",
                value: summary.totalOrders,
                description:
                    "Orders in the webshop",
                icon: <ReceiptLong />,
                color: "secondary.main",
                backgroundColor:
                    "action.hover",
                path: "/admin/orders",
            },
            {
                title: "Products",
                value:
                    summary.totalProducts,
                description:
                    "Products in the catalogue",
                icon: <Inventory2 />,
                color: "primary.main",
                backgroundColor:
                    "primary.50",
                path: "/admin/products",
            },
            {
                title:
                    "Products without content",
                value:
                    summary.productsWithoutContent,
                description:
                    "Missing description and specifications",
                icon: (
                    <DescriptionOutlined />
                ),
                color: "warning.dark",
                backgroundColor:
                    "warning.50",
                path: "/admin/products",
            },
        ]
        : [];

    return (
        <Box>
            <PageHeader
                title="Admin dashboard"
                subtitle="Monitor the webshop and access the administration areas."
                action={
                    <Stack
                        direction={{
                            xs: "column",
                            sm: "row",
                        }}
                        spacing={1}
                    >
                        <Button
                            variant="outlined"
                            startIcon={<Refresh />}
                            onClick={() =>
                                void loadDashboard()
                            }
                            disabled={loading}
                        >
                            Refresh
                        </Button>

                        <Button
                            component={Link}
                            to="/products"
                            variant="outlined"
                            endIcon={<OpenInNew />}
                        >
                            View webshop
                        </Button>
                    </Stack>
                }
            />

            <Box
                sx={{
                    mb: 4,
                    p: {
                        xs: 3,
                        md: 4,
                    },
                    borderRadius: 3,
                    color:
                        "primary.contrastText",
                    background:
                        "linear-gradient(135deg, #0F172A 0%, #1E3A8A 100%)",
                }}
            >
                <Typography
                    variant="overline"
                    sx={{ opacity: 0.8 }}
                >
                    B2B Commerce Administration
                </Typography>

                <Typography
                    variant="h4"
                    component="h2"
                    sx={{
                        mt: 0.5,
                        fontWeight: 800,
                    }}
                >
                    Webshop overview
                </Typography>

                <Typography
                    sx={{
                        mt: 1.5,
                        maxWidth: 680,
                        opacity: 0.85,
                    }}
                >
                    Review the current webshop
                    activity and continue to the
                    relevant administration area.
                </Typography>
            </Box>

            {error && (
                <Alert
                    severity="error"
                    sx={{ mb: 3 }}
                    action={
                        <Button
                            color="inherit"
                            size="small"
                            onClick={() =>
                                void loadDashboard()
                            }
                        >
                            Try again
                        </Button>
                    }
                >
                    {error}
                </Alert>
            )}

            {loading && (
                <LoadingSpinner text="Loading dashboard figures..." />
            )}

            {!loading && summary && (
                <>
                    <Stack
                        spacing={0.5}
                        sx={{ mb: 3 }}
                    >
                        <Typography
                            variant="h5"
                            component="h2"
                            sx={{
                                fontWeight: 800,
                            }}
                        >
                            Current overview
                        </Typography>

                        <Typography color="text.secondary">
                            Live figures from the
                            webshop.
                        </Typography>
                    </Stack>

                    <Grid
                        container
                        spacing={2.5}
                        sx={{ mb: 5 }}
                    >
                        {dashboardMetrics.map(
                            (metric) => (
                                <Grid
                                    key={
                                        metric.title
                                    }
                                    size={{
                                        xs: 12,
                                        sm: 6,
                                        lg: 4,
                                    }}
                                >
                                    <Card
                                        component={Link}
                                        to={
                                            metric.path
                                        }
                                        variant="outlined"
                                        sx={{
                                            height:
                                                "100%",
                                            display:
                                                "block",
                                            color:
                                                "text.primary",
                                            textDecoration:
                                                "none",
                                            transition:
                                                "transform 150ms ease, box-shadow 150ms ease",
                                            "&:hover": {
                                                transform:
                                                    "translateY(-2px)",
                                                boxShadow: 3,
                                            },
                                        }}
                                    >
                                        <CardContent>
                                            <Stack
                                                direction="row"
                                                spacing={2}
                                                sx={{
                                                    alignItems:
                                                        "center",
                                                }}
                                            >
                                                <Box
                                                    sx={{
                                                        width: 52,
                                                        height: 52,
                                                        borderRadius:
                                                            2,
                                                        display:
                                                            "grid",
                                                        placeItems:
                                                            "center",
                                                        flexShrink: 0,
                                                        color:
                                                            metric.color,
                                                        bgcolor:
                                                            metric.backgroundColor,
                                                    }}
                                                >
                                                    {
                                                        metric.icon
                                                    }
                                                </Box>

                                                <Box>
                                                    <Typography
                                                        variant="h4"
                                                        sx={{
                                                            fontWeight: 800,
                                                            lineHeight: 1,
                                                        }}
                                                    >
                                                        {
                                                            metric.value
                                                        }
                                                    </Typography>

                                                    <Typography
                                                        sx={{
                                                            mt: 0.75,
                                                            fontWeight: 700,
                                                        }}
                                                    >
                                                        {
                                                            metric.title
                                                        }
                                                    </Typography>

                                                    <Typography
                                                        variant="body2"
                                                        color="text.secondary"
                                                        sx={{
                                                            mt: 0.25,
                                                        }}
                                                    >
                                                        {
                                                            metric.description
                                                        }
                                                    </Typography>
                                                </Box>
                                            </Stack>
                                        </CardContent>
                                    </Card>
                                </Grid>
                            )
                        )}
                    </Grid>
                </>
            )}

            <Stack
                spacing={0.5}
                sx={{ mb: 3 }}
            >
                <Typography
                    variant="h5"
                    component="h2"
                    sx={{ fontWeight: 800 }}
                >
                    Administration
                </Typography>

                <Typography color="text.secondary">
                    Choose an area to continue.
                </Typography>
            </Stack>

            <Grid container spacing={3}>
                {administrationAreas.map(
                    (area) => (
                        <Grid
                            key={area.title}
                            size={{
                                xs: 12,
                                md: 6,
                                xl: 4,
                            }}
                        >
                            <Card
                                variant="outlined"
                                sx={{
                                    height: "100%",
                                    display: "flex",
                                    flexDirection:
                                        "column",
                                }}
                            >
                                <CardContent
                                    sx={{
                                        flexGrow: 1,
                                    }}
                                >
                                    <Box
                                        sx={{
                                            width: 48,
                                            height: 48,
                                            borderRadius:
                                                2,
                                            bgcolor:
                                                "action.hover",
                                            color:
                                                "secondary.main",
                                            display:
                                                "grid",
                                            placeItems:
                                                "center",
                                        }}
                                    >
                                        {area.icon}
                                    </Box>

                                    <Typography
                                        variant="h6"
                                        component="h3"
                                        sx={{
                                            mt: 3,
                                            fontWeight: 800,
                                        }}
                                    >
                                        {area.title}
                                    </Typography>

                                    <Typography
                                        color="text.secondary"
                                        sx={{ mt: 1 }}
                                    >
                                        {
                                            area.description
                                        }
                                    </Typography>
                                </CardContent>

                                <CardActions
                                    sx={{
                                        px: 2,
                                        pb: 2,
                                    }}
                                >
                                    <Button
                                        component={Link}
                                        to={area.path}
                                    >
                                        Manage{" "}
                                        {area.title.toLowerCase()}
                                    </Button>
                                </CardActions>
                            </Card>
                        </Grid>
                    )
                )}
            </Grid>
        </Box>
    );
};

export default AdminDashboardPage;