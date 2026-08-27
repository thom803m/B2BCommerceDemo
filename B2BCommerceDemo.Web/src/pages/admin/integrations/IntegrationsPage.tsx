import axios from "axios";
import {
    AutoAwesome,
    Inventory2,
    LocalShipping,
    ReceiptLong,
} from "@mui/icons-material";
import {
    Alert,
    Box,
    Button,
    Chip,
    CircularProgress,
    Divider,
    Grid,
    Paper,
    Stack,
    Typography,
} from "@mui/material";
import {
    type ReactNode,
    useState,
} from "react";
import {
    enrichMissingProductContent,
    syncExpectedDeliveries,
    syncOrderStatuses,
    syncRackbeatProducts,
    type IcecatEnrichmentResult,
    type IntegrationImportResult,
} from "../../../api/integrationApi";
import ConfirmDialog from "../../../components/common/ConfirmDialog";
import PageHeader from "../../../components/common/PageHeader";

type IntegrationActionId =
    | "products"
    | "deliveries"
    | "orderStatuses"
    | "icecat";

type RackbeatActionId = Exclude<
    IntegrationActionId,
    "icecat"
>;

type IntegrationAction = {
    id: IntegrationActionId;
    title: string;
    provider: "Rackbeat" | "Icecat";
    description: string;
    impact: string;
    buttonLabel: string;
    confirmTitle: string;
    confirmDescription: string;
    icon: ReactNode;
};

type LatestIntegrationResult =
    | {
        actionId: RackbeatActionId;
        completedAt: string;
        data: IntegrationImportResult;
    }
    | {
        actionId: "icecat";
        completedAt: string;
        data: IcecatEnrichmentResult;
    };

const getApiErrorMessage = (
    error: unknown,
    fallback: string
) => {
    if (
        axios.isAxiosError(error) &&
        typeof error.response?.data?.detail === "string"
    ) {
        return error.response.data.detail;
    }

    return fallback;
};

const integrationActions:
    IntegrationAction[] = [
        {
            id: "products",
            title: "Products and stock",
            provider: "Rackbeat",
            description:
                "Fetch the latest product information, prices and stock quantities from Rackbeat.",
            impact:
                "Reads from Rackbeat and updates the local webshop catalogue. It does not write to Rackbeat.",
            buttonLabel: "Sync products",
            confirmTitle:
                "Synchronize products from Rackbeat?",
            confirmDescription:
                "The webshop will read the latest product, price and stock information from Rackbeat and update the local catalogue. This action does not write anything to Rackbeat.",
            icon: <Inventory2 />,
        },
        {
            id: "deliveries",
            title: "Expected deliveries",
            provider: "Rackbeat",
            description:
                "Fetch open purchase-order information and update expected product delivery dates.",
            impact:
                "Reads purchase-order data from Rackbeat and updates local delivery dates. It does not write to Rackbeat.",
            buttonLabel: "Sync deliveries",
            confirmTitle:
                "Synchronize expected deliveries?",
            confirmDescription:
                "The webshop will read open purchase-order information from Rackbeat and update local expected delivery dates. This action does not write anything to Rackbeat.",
            icon: <LocalShipping />,
        },
        {
            id: "orderStatuses",
            title: "Order statuses",
            provider: "Rackbeat",
            description:
                "Fetch current Rackbeat order information and update the status of webshop orders.",
            impact:
                "Reads order information from Rackbeat and updates local order statuses. It does not create or modify Rackbeat orders.",
            buttonLabel: "Sync order statuses",
            confirmTitle:
                "Synchronize order statuses?",
            confirmDescription:
                "The webshop will read current order information from Rackbeat and update matching local order statuses. This action does not create or modify orders in Rackbeat.",
            icon: <ReceiptLong />,
        },
        {
            id: "icecat",
            title: "Missing product content",
            provider: "Icecat",
            description:
                "Enrich active products that are missing descriptions, specifications or images.",
            impact:
                "Reads data from Icecat and updates local product content. Content-protected products are skipped.",
            buttonLabel: "Enrich missing content",
            confirmTitle:
                "Enrich missing product content?",
            confirmDescription:
                "The webshop will contact Icecat for active products with missing content. Available descriptions, specifications and images may be added locally. Content-protected products will not be overwritten, and nothing is written to Rackbeat.",
            icon: <AutoAwesome />,
        },
    ];

const formatCompletedAt = (
    value: string
) => {
    return new Intl.DateTimeFormat(
        "da-DK",
        {
            dateStyle: "medium",
            timeStyle: "medium",
        }
    ).format(new Date(value));
};

const IntegrationsPage = () => {
    const [
        selectedActionId,
        setSelectedActionId,
    ] = useState<
        IntegrationActionId | null
    >(null);

    const [
        runningActionId,
        setRunningActionId,
    ] = useState<
        IntegrationActionId | null
    >(null);

    const [
        latestResult,
        setLatestResult,
    ] =
        useState<
            LatestIntegrationResult | null
        >(null);

    const [error, setError] =
        useState<string | null>(null);

    const selectedAction =
        integrationActions.find(
            (action) =>
                action.id ===
                selectedActionId
        ) ?? null;

    const latestAction =
        integrationActions.find(
            (action) =>
                action.id ===
                latestResult?.actionId
        ) ?? null;

    const handleConfirmRun =
        async () => {
            if (
                !selectedActionId ||
                runningActionId
            ) {
                return;
            }

            const actionId =
                selectedActionId;

            setSelectedActionId(null);
            setRunningActionId(actionId);
            setLatestResult(null);
            setError(null);

            try {
                const completedAt =
                    new Date().toISOString();

                switch (actionId) {
                    case "products": {
                        const data =
                            await syncRackbeatProducts();

                        setLatestResult({
                            actionId,
                            completedAt,
                            data,
                        });

                        break;
                    }

                    case "deliveries": {
                        const data =
                            await syncExpectedDeliveries();

                        setLatestResult({
                            actionId,
                            completedAt,
                            data,
                        });

                        break;
                    }

                    case "orderStatuses": {
                        const data =
                            await syncOrderStatuses();

                        setLatestResult({
                            actionId,
                            completedAt,
                            data,
                        });

                        break;
                    }

                    case "icecat": {
                        const data =
                            await enrichMissingProductContent();

                        setLatestResult({
                            actionId,
                            completedAt,
                            data,
                        });

                        break;
                    }
                }
            } catch (error) {
                console.error(
                    "Integration action failed",
                    error
                );

                setError(
                    getApiErrorMessage(
                        error,
                        "The integration action could not be completed. Please try again."
                    )
                );
            } finally {
                setRunningActionId(null);
            }
        };

    const warnings =
        latestResult?.data.warnings ?? [];

    return (
        <Box>
            <PageHeader
                title="Integrations"
                subtitle="Run and review manual synchronization tasks for Rackbeat and Icecat."
            />

            <Alert
                severity="warning"
                sx={{ mb: 3 }}
            >
                These actions contact external
                services and may update data in
                the webshop. The Rackbeat actions
                shown here only read from
                Rackbeat—they do not create,
                edit or delete data in Rackbeat.
            </Alert>

            {error && (
                <Alert
                    severity="error"
                    sx={{ mb: 3 }}
                    onClose={() =>
                        setError(null)
                    }
                >
                    {error}
                </Alert>
            )}

            <Grid container spacing={3}>
                {integrationActions.map(
                    (action) => {
                        const running =
                            runningActionId ===
                            action.id;

                        return (
                            <Grid
                                key={action.id}
                                size={{
                                    xs: 12,
                                    md: 6,
                                }}
                            >
                                <Paper
                                    variant="outlined"
                                    sx={{
                                        height:
                                            "100%",
                                        p: {
                                            xs: 2,
                                            md: 3,
                                        },
                                    }}
                                >
                                    <Stack
                                        spacing={2.5}
                                        sx={{
                                            height:
                                                "100%",
                                        }}
                                    >
                                        <Stack
                                            direction="row"
                                            spacing={2}
                                            sx={{
                                                alignItems:
                                                    "flex-start",
                                            }}
                                        >
                                            <Box
                                                sx={{
                                                    width: 48,
                                                    height: 48,
                                                    borderRadius:
                                                        2,
                                                    display:
                                                        "grid",
                                                    placeItems:
                                                        "center",
                                                    bgcolor:
                                                        action.provider ===
                                                            "Rackbeat"
                                                            ? "secondary.main"
                                                            : "primary.main",
                                                    color:
                                                        action.provider ===
                                                            "Rackbeat"
                                                            ? "secondary.contrastText"
                                                            : "primary.contrastText",
                                                    flexShrink: 0,
                                                }}
                                            >
                                                {
                                                    action.icon
                                                }
                                            </Box>

                                            <Box
                                                sx={{
                                                    minWidth: 0,
                                                }}
                                            >
                                                <Typography
                                                    variant="h6"
                                                    component="h2"
                                                    sx={{
                                                        fontWeight: 800,
                                                    }}
                                                >
                                                    {
                                                        action.title
                                                    }
                                                </Typography>

                                                <Chip
                                                    label={
                                                        action.provider
                                                    }
                                                    size="small"
                                                    variant="outlined"
                                                    sx={{
                                                        mt: 1,
                                                    }}
                                                />
                                            </Box>
                                        </Stack>

                                        <Typography>
                                            {
                                                action.description
                                            }
                                        </Typography>

                                        <Alert
                                            severity="info"
                                            variant="outlined"
                                        >
                                            {
                                                action.impact
                                            }
                                        </Alert>

                                        <Box
                                            sx={{
                                                flexGrow: 1,
                                            }}
                                        />

                                        <Button
                                            variant="contained"
                                            startIcon={
                                                running ? (
                                                    <CircularProgress
                                                        size={
                                                            18
                                                        }
                                                        color="inherit"
                                                    />
                                                ) : (
                                                    action.icon
                                                )
                                            }
                                            onClick={() =>
                                                setSelectedActionId(
                                                    action.id
                                                )
                                            }
                                            disabled={
                                                runningActionId !==
                                                null
                                            }
                                            fullWidth
                                        >
                                            {running
                                                ? "Running..."
                                                : action.buttonLabel}
                                        </Button>
                                    </Stack>
                                </Paper>
                            </Grid>
                        );
                    }
                )}
            </Grid>

            {latestResult &&
                latestAction && (
                    <Paper
                        variant="outlined"
                        sx={{
                            mt: 4,
                            p: {
                                xs: 2,
                                md: 3,
                            },
                        }}
                    >
                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={1}
                            sx={{
                                alignItems: {
                                    xs: "flex-start",
                                    sm: "center",
                                },
                                justifyContent:
                                    "space-between",
                            }}
                        >
                            <Box>
                                <Typography
                                    variant="h5"
                                    component="h2"
                                    sx={{
                                        fontWeight: 800,
                                    }}
                                >
                                    Latest result
                                </Typography>

                                <Typography
                                    color="text.secondary"
                                    sx={{ mt: 0.5 }}
                                >
                                    {
                                        latestAction.title
                                    }{" "}
                                    ·{" "}
                                    {formatCompletedAt(
                                        latestResult.completedAt
                                    )}
                                </Typography>
                            </Box>

                            <Chip
                                label="Completed"
                                color="success"
                                variant="outlined"
                            />
                        </Stack>

                        <Divider sx={{ my: 3 }} />

                        {latestResult.actionId ===
                            "icecat" ? (
                            <Grid
                                container
                                spacing={2}
                            >
                                {[
                                    {
                                        label:
                                            "Checked",
                                        value:
                                            latestResult
                                                .data
                                                .checked,
                                    },
                                    {
                                        label:
                                            "Fully enriched",
                                        value:
                                            latestResult
                                                .data
                                                .fullyEnriched,
                                    },
                                    {
                                        label:
                                            "Partially enriched",
                                        value:
                                            latestResult
                                                .data
                                                .partiallyEnriched,
                                    },
                                    {
                                        label:
                                            "Full Icecat required",
                                        value:
                                            latestResult
                                                .data
                                                .fullIcecatRequired,
                                    },
                                    {
                                        label:
                                            "Not found",
                                        value:
                                            latestResult
                                                .data
                                                .notFound,
                                    },
                                    {
                                        label:
                                            "Failed",
                                        value:
                                            latestResult
                                                .data
                                                .failed,
                                    },
                                ].map((item) => (
                                    <Grid
                                        key={
                                            item.label
                                        }
                                        size={{
                                            xs: 6,
                                            md: 4,
                                            lg: 2,
                                        }}
                                    >
                                        <Box
                                            sx={{
                                                p: 2,
                                                borderRadius:
                                                    2,
                                                bgcolor:
                                                    "action.hover",
                                                textAlign:
                                                    "center",
                                            }}
                                        >
                                            <Typography
                                                variant="h5"
                                                sx={{
                                                    fontWeight: 800,
                                                }}
                                            >
                                                {
                                                    item.value
                                                }
                                            </Typography>

                                            <Typography
                                                variant="body2"
                                                color="text.secondary"
                                            >
                                                {
                                                    item.label
                                                }
                                            </Typography>
                                        </Box>
                                    </Grid>
                                ))}
                            </Grid>
                        ) : (
                            <Grid
                                container
                                spacing={2}
                            >
                                {[
                                    {
                                        label:
                                            "Created",
                                        value:
                                            latestResult
                                                .data
                                                .created,
                                    },
                                    {
                                        label:
                                            "Updated",
                                        value:
                                            latestResult
                                                .data
                                                .updated,
                                    },
                                    {
                                        label:
                                            "Skipped",
                                        value:
                                            latestResult
                                                .data
                                                .skipped,
                                    },
                                    {
                                        label:
                                            "Warnings",
                                        value:
                                            latestResult
                                                .data
                                                .warnings
                                                .length,
                                    },
                                ].map((item) => (
                                    <Grid
                                        key={
                                            item.label
                                        }
                                        size={{
                                            xs: 6,
                                            md: 3,
                                        }}
                                    >
                                        <Box
                                            sx={{
                                                p: 2,
                                                borderRadius:
                                                    2,
                                                bgcolor:
                                                    "action.hover",
                                                textAlign:
                                                    "center",
                                            }}
                                        >
                                            <Typography
                                                variant="h5"
                                                sx={{
                                                    fontWeight: 800,
                                                }}
                                            >
                                                {
                                                    item.value
                                                }
                                            </Typography>

                                            <Typography
                                                variant="body2"
                                                color="text.secondary"
                                            >
                                                {
                                                    item.label
                                                }
                                            </Typography>
                                        </Box>
                                    </Grid>
                                ))}
                            </Grid>
                        )}

                        {warnings.length > 0 && (
                            <>
                                <Divider
                                    sx={{ my: 3 }}
                                />

                                <Alert severity="warning">
                                    <Typography
                                        sx={{
                                            mb: 1,
                                            fontWeight: 700,
                                        }}
                                    >
                                        Warnings
                                    </Typography>

                                    <Box
                                        component="ul"
                                        sx={{
                                            my: 0,
                                            pl: 2.5,
                                        }}
                                    >
                                        {warnings
                                            .slice(
                                                0,
                                                20
                                            )
                                            .map(
                                                (
                                                    warning,
                                                    index
                                                ) => (
                                                    <li
                                                        key={`${warning}-${index}`}
                                                    >
                                                        {
                                                            warning
                                                        }
                                                    </li>
                                                )
                                            )}
                                    </Box>

                                    {warnings.length >
                                        20 && (
                                            <Typography
                                                variant="body2"
                                                sx={{
                                                    mt: 1,
                                                }}
                                            >
                                                And{" "}
                                                {warnings.length -
                                                    20}{" "}
                                                more
                                                warnings.
                                                Review the
                                                backend
                                                logs for
                                                full
                                                details.
                                            </Typography>
                                        )}
                                </Alert>
                            </>
                        )}
                    </Paper>
                )}

            <ConfirmDialog
                open={
                    selectedAction !== null
                }
                title={
                    selectedAction
                        ?.confirmTitle ?? ""
                }
                description={
                    selectedAction
                        ?.confirmDescription ?? ""
                }
                confirmLabel={
                    selectedAction
                        ?.buttonLabel ?? "Run"
                }
                onClose={() =>
                    setSelectedActionId(null)
                }
                onConfirm={() =>
                    void handleConfirmRun()
                }
            />
        </Box>
    );
};

export default IntegrationsPage;