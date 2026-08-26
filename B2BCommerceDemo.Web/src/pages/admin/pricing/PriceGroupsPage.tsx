import { Paid, PriceChange, Refresh, Save, } from "@mui/icons-material";
import { Alert, Box, Button, Grid, Paper, Snackbar, Stack, TextField, Typography, } from "@mui/material";
import { useCallback, useEffect, useState, } from "react";
import { Link, } from "react-router-dom";
import { getPriceGroups, updatePriceGroup, type PriceGroup, } from "../../../api/priceGroupApi";
import EmptyState from "../../../components/common/EmptyState";
import LoadingSpinner from "../../../components/common/LoadingSpinner";
import PageHeader from "../../../components/common/PageHeader";

type PriceGroupDraft = {
    name: string;
    percentageAdjustment: string;
};

const createDrafts = (
    priceGroups: PriceGroup[]
): Record<number, PriceGroupDraft> => {
    return Object.fromEntries(
        priceGroups.map((priceGroup) => [
            priceGroup.id,
            {
                name: priceGroup.name,
                percentageAdjustment: String(
                    priceGroup.percentageAdjustment
                ),
            },
        ])
    );
};

const PriceGroupsPage = () => {
    const [priceGroups, setPriceGroups] =
        useState<PriceGroup[]>([]);

    const [drafts, setDrafts] =
        useState<
            Record<number, PriceGroupDraft>
        >({});

    const [
        processingPriceGroupId,
        setProcessingPriceGroupId,
    ] = useState<number | null>(null);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState<string | null>(null);

    const loadPriceGroups =
        useCallback(async () => {
            setLoading(true);
            setError(null);

            try {
                const result =
                    await getPriceGroups();

                setPriceGroups(result);
                setDrafts(
                    createDrafts(result)
                );
            } catch (error) {
                console.error(
                    "Failed to load price groups",
                    error
                );

                setError(
                    "The price groups could not be loaded. Please try again."
                );
            } finally {
                setLoading(false);
            }
        }, []);

    useEffect(() => {
        void loadPriceGroups();
    }, [loadPriceGroups]);

    const updateDraft = (
        priceGroupId: number,
        values: Partial<PriceGroupDraft>
    ) => {
        setDrafts((current) => ({
            ...current,
            [priceGroupId]: {
                ...current[priceGroupId],
                ...values,
            },
        }));
    };

    const hasChanges = (
        priceGroup: PriceGroup
    ) => {
        const draft =
            drafts[priceGroup.id];

        if (!draft) {
            return false;
        }

        return (
            draft.name.trim() !==
                priceGroup.name ||
            Number(
                draft.percentageAdjustment
            ) !==
                priceGroup.percentageAdjustment
        );
    };

    const handleSave = async (
        priceGroup: PriceGroup
    ) => {
        const draft =
            drafts[priceGroup.id];

        if (!draft) {
            return;
        }

        const name = draft.name.trim();

        const percentageAdjustment =
            Number(
                draft.percentageAdjustment
            );

        if (!name) {
            setError(
                "The price group name is required."
            );
            return;
        }

        if (
            !Number.isFinite(
                percentageAdjustment
            )
        ) {
            setError(
                "The percentage adjustment must be a valid number."
            );
            return;
        }

        setProcessingPriceGroupId(
            priceGroup.id
        );

        setError(null);
        setSuccessMessage(null);

        try {
            const updatedPriceGroup =
                await updatePriceGroup(
                    priceGroup.id,
                    {
                        name,
                        percentageAdjustment,
                    }
                );

            setPriceGroups((current) =>
                current.map((item) =>
                    item.id ===
                    updatedPriceGroup.id
                        ? updatedPriceGroup
                        : item
                )
            );

            setDrafts((current) => ({
                ...current,
                [updatedPriceGroup.id]: {
                    name:
                        updatedPriceGroup.name,
                    percentageAdjustment:
                        String(
                            updatedPriceGroup
                                .percentageAdjustment
                        ),
                },
            }));

            setSuccessMessage(
                `${updatedPriceGroup.name} was updated successfully.`
            );
        } catch (error) {
            console.error(
                "Failed to update price group",
                error
            );

            setError(
                "The price group could not be updated. Please try again."
            );
        } finally {
            setProcessingPriceGroupId(
                null
            );
        }
    };

    return (
        <Box>
            <PageHeader
                title="Pricing"
                subtitle="Manage price group adjustments and company-specific product prices."
                action={
                    <Stack
                        direction={{
                            xs: "column",
                            sm: "row",
                        }}
                        spacing={1}
                    >
                        <Button
                            component={Link}
                            to="/admin/pricing/company-prices"
                            variant="contained"
                            startIcon={<PriceChange />}
                        >
                            Company prices
                        </Button>

                        <Button
                            variant="outlined"
                            startIcon={<Refresh />}
                            onClick={() =>
                                void loadPriceGroups()
                            }
                            disabled={loading}
                        >
                            Refresh
                        </Button>
                    </Stack>
                }
            />

            <Alert
                severity="info"
                sx={{ mb: 3 }}
            >
                Price group adjustments are
                applied to product prices for
                every company assigned to the
                group. Positive values increase
                the price, while negative values
                reduce it.
            </Alert>

            {error && (
                <Alert
                    severity="error"
                    sx={{ mb: 3 }}
                    action={
                        <Button
                            color="inherit"
                            size="small"
                            onClick={() =>
                                void loadPriceGroups()
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
                <LoadingSpinner text="Loading price groups..." />
            )}

            {!loading &&
                !error &&
                priceGroups.length === 0 && (
                    <EmptyState
                        title="No price groups found"
                        description="There are currently no price groups to manage."
                        icon={<Paid />}
                    />
                )}

            {!loading &&
                priceGroups.length > 0 && (
                    <>
                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={1}
                            sx={{
                                mb: 3,
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
                                    Price groups
                                </Typography>

                                <Typography
                                    color="text.secondary"
                                    sx={{ mt: 0.5 }}
                                >
                                    Update the name and
                                    percentage adjustment
                                    for each group.
                                </Typography>
                            </Box>

                            <Typography
                                color="text.secondary"
                            >
                                {priceGroups.length}{" "}
                                {priceGroups.length === 1
                                    ? "group"
                                    : "groups"}
                            </Typography>
                        </Stack>

                        <Grid
                            container
                            spacing={3}
                        >
                            {priceGroups.map(
                                (priceGroup) => {
                                    const draft =
                                        drafts[
                                            priceGroup.id
                                        ];

                                    const processing =
                                        processingPriceGroupId ===
                                        priceGroup.id;

                                    return (
                                        <Grid
                                            key={
                                                priceGroup.id
                                            }
                                            size={{
                                                xs: 12,
                                                lg: 4,
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
                                                    spacing={3}
                                                    sx={{
                                                        height:
                                                            "100%",
                                                    }}
                                                >
                                                    <Box>
                                                        <Typography
                                                            variant="h6"
                                                            component="h3"
                                                            sx={{
                                                                fontWeight: 800,
                                                            }}
                                                        >
                                                            {
                                                                priceGroup.name
                                                            }
                                                        </Typography>

                                                        <Typography
                                                            variant="body2"
                                                            color="text.secondary"
                                                        >
                                                            ID:{" "}
                                                            {
                                                                priceGroup.id
                                                            }
                                                        </Typography>
                                                    </Box>

                                                    <TextField
                                                        fullWidth
                                                        label="Group name"
                                                        value={
                                                            draft?.name ??
                                                            ""
                                                        }
                                                        onChange={(
                                                            event
                                                        ) =>
                                                            updateDraft(
                                                                priceGroup.id,
                                                                {
                                                                    name:
                                                                        event
                                                                            .target
                                                                            .value,
                                                                }
                                                            )
                                                        }
                                                        disabled={
                                                            processing
                                                        }
                                                    />

                                                    <TextField
                                                        fullWidth
                                                        type="number"
                                                        label="Percentage adjustment"
                                                        value={
                                                            draft
                                                                ?.percentageAdjustment ??
                                                            ""
                                                        }
                                                        onChange={(
                                                            event
                                                        ) =>
                                                            updateDraft(
                                                                priceGroup.id,
                                                                {
                                                                    percentageAdjustment:
                                                                        event
                                                                            .target
                                                                            .value,
                                                                }
                                                            )
                                                        }
                                                        disabled={
                                                            processing
                                                        }
                                                        helperText="For example: 5 adds 5%, while -5 subtracts 5%."
                                                        slotProps={{
                                                            htmlInput:
                                                                {
                                                                    step: 0.01,
                                                                },
                                                        }}
                                                    />

                                                    <Button
                                                        variant="contained"
                                                        startIcon={
                                                            <Save />
                                                        }
                                                        onClick={() =>
                                                            void handleSave(
                                                                priceGroup
                                                            )
                                                        }
                                                        disabled={
                                                            processing ||
                                                            !hasChanges(
                                                                priceGroup
                                                            )
                                                        }
                                                        sx={{
                                                            mt:
                                                                "auto",
                                                        }}
                                                    >
                                                        {processing
                                                            ? "Saving..."
                                                            : "Save changes"}
                                                    </Button>
                                                </Stack>
                                            </Paper>
                                        </Grid>
                                    );
                                }
                            )}
                        </Grid>
                    </>
                )}

            <Snackbar
                open={
                    successMessage !== null
                }
                autoHideDuration={5000}
                onClose={() =>
                    setSuccessMessage(null)
                }
                anchorOrigin={{
                    vertical: "bottom",
                    horizontal: "center",
                }}
            >
                <Alert
                    severity="success"
                    variant="filled"
                    onClose={() =>
                        setSuccessMessage(null)
                    }
                >
                    {successMessage}
                </Alert>
            </Snackbar>
        </Box>
    );
};

export default PriceGroupsPage;