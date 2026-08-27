import axios from "axios";
import { ArrowBack, AutoAwesome, Save, } from "@mui/icons-material";
import { Alert, Box, Button, Chip, Divider, FormControlLabel, Grid, Paper, Stack, Switch, TextField, Typography, } from "@mui/material";
import { useEffect, useState, } from "react";
import { Link, useLocation, useParams, } from "react-router-dom";
import { enrichProduct, getProductById, updateProductContent, type Product, } from "../../../api/productApi";
import AdminProductImageManager from "../../../components/admin/products/AdminProductImageManager";
import ConfirmDialog from "../../../components/common/ConfirmDialog";
import LoadingSpinner from "../../../components/common/LoadingSpinner";
import PageHeader from "../../../components/common/PageHeader";

type ProductNavigationState = {
    productListUrl?: string;
};

type ProductContentFormValues = {
    description: string;
    specificationsJson: string;
    contentLocked: boolean;
};

const defaultFormValues:
    ProductContentFormValues = {
    description: "",
    specificationsJson: "",
    contentLocked: false,
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

const formatDate = (
    value?: string | null
) => {
    if (!value) {
        return "Never synchronized";
    }

    const dateOnlyMatch = value.match(
        /^(\d{2})-(\d{2})-(\d{4})$/
    );

    if (dateOnlyMatch) {
        const [
            ,
            day,
            month,
            year,
        ] = dateOnlyMatch;

        const date = new Date(
            Date.UTC(
                Number(year),
                Number(month) - 1,
                Number(day)
            )
        );

        return new Intl.DateTimeFormat(
            "da-DK",
            {
                dateStyle: "medium",
                timeZone: "UTC",
            }
        ).format(date);
    }

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
        return "Unknown date";
    }

    return new Intl.DateTimeFormat(
        "da-DK",
        {
            dateStyle: "medium",
            timeStyle: "short",
        }
    ).format(date);
};

const AdminProductContentPage = () => {
    const { id } = useParams();
    const location = useLocation();

    const productId = Number(id);

    const [product, setProduct] =
        useState<Product | null>(null);

    const [form, setForm] =
        useState<ProductContentFormValues>({
            ...defaultFormValues,
        });

    const [
        enrichmentDialogOpen,
        setEnrichmentDialogOpen,
    ] = useState(false);

    const [enriching, setEnriching] =
        useState(false);

    const [loading, setLoading] =
        useState(true);

    const [saving, setSaving] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState<string | null>(null);

    const [
        specificationsError,
        setSpecificationsError,
    ] = useState<string | null>(null);

    const navigationState =
        location.state as
        | ProductNavigationState
        | null;

    const productListUrl =
        navigationState?.productListUrl ??
        "/admin/products";

    useEffect(() => {
        const loadProduct = async () => {
            setLoading(true);
            setError(null);

            if (
                !Number.isInteger(productId) ||
                productId <= 0
            ) {
                setError(
                    "The selected product ID is invalid."
                );

                setLoading(false);
                return;
            }

            try {
                const result =
                    await getProductById(
                        productId
                    );

                setProduct(result);

                setForm({
                    description:
                        result.description ??
                        "",
                    specificationsJson:
                        result.specificationsJson ??
                        "",
                    contentLocked:
                        result.contentLocked,
                });
            } catch (error) {
                console.error(
                    "Failed to load product content",
                    error
                );

                setError(
                    "The product content could not be loaded. The product may no longer exist."
                );
            } finally {
                setLoading(false);
            }
        };

        void loadProduct();
    }, [productId]);

    const validateSpecifications = () => {
        const value =
            form.specificationsJson.trim();

        if (!value) {
            setSpecificationsError(null);
            return true;
        }

        try {
            JSON.parse(value);

            setSpecificationsError(null);
            return true;
        } catch {
            setSpecificationsError(
                "Specifications must contain valid JSON."
            );

            return false;
        }
    };

    const handleSave = async () => {
        if (
            saving ||
            enriching ||
            !validateSpecifications()
        ) {
            return;
        }

        setSaving(true);
        setError(null);
        setSuccessMessage(null);

        try {
            const updatedProduct =
                await updateProductContent(
                    productId,
                    {
                        description:
                            form.description.trim() ||
                            null,
                        specificationsJson:
                            form.specificationsJson.trim() ||
                            null,
                        contentLocked:
                            form.contentLocked,
                    }
                );

            setProduct(updatedProduct);

            setForm({
                description:
                    updatedProduct.description ??
                    "",
                specificationsJson:
                    updatedProduct.specificationsJson ??
                    "",
                contentLocked:
                    updatedProduct.contentLocked,
            });

            setSuccessMessage(
                "The product content was updated successfully."
            );
        } catch (error) {
            console.error(
                "Failed to update product content",
                error
            );

            setError(
                "The product content could not be updated. Please try again."
            );
        } finally {
            setSaving(false);
        }
    };

    const handleConfirmEnrichment =
        async () => {
            if (
                !product ||
                product.contentLocked ||
                enriching || 
                saving
            ) {
                return;
            }

            setEnriching(true);
            setError(null);
            setSuccessMessage(null);

            try {
                const enrichedProduct =
                    await enrichProduct(
                        product.id
                    );

                setProduct(
                    enrichedProduct
                );

                setForm({
                    description:
                        enrichedProduct.description ??
                        "",
                    specificationsJson:
                        enrichedProduct.specificationsJson ??
                        "",
                    contentLocked:
                        enrichedProduct.contentLocked,
                });

                setSpecificationsError(null);
                setEnrichmentDialogOpen(false);

                setSuccessMessage(
                    "The product was enriched successfully with content from Icecat."
                );
            } catch (error) {
                console.error(
                    "Failed to enrich product",
                    error
                );

                setEnrichmentDialogOpen(false);

                setError(
                    getApiErrorMessage(
                        error,
                        "The product could not be enriched. Please try again."
                    )
                );
            } finally {
                setEnriching(false);
            }
        };

    return (
        <Box>
            <PageHeader
                title="Product content"
                subtitle={
                    product
                        ? `Manage descriptions, specifications and Icecat information for ${product.name}.`
                        : "Manage product content and Icecat information."
                }
                action={
                    <Button
                        component={Link}
                        to={productListUrl}
                        variant="outlined"
                        startIcon={
                            <ArrowBack />
                        }
                    >
                        Back to products
                    </Button>
                }
            />

            {error && (
                <Alert
                    severity="error"
                    sx={{ mb: 3 }}
                >
                    {error}
                </Alert>
            )}

            {successMessage && (
                <Alert
                    severity="success"
                    sx={{ mb: 3 }}
                    onClose={() =>
                        setSuccessMessage(null)
                    }
                >
                    {successMessage}
                </Alert>
            )}

            {loading && (
                <LoadingSpinner text="Loading product content..." />
            )}

            {!loading && product && (
                <Stack spacing={3}>
                    <Paper
                        variant="outlined"
                        sx={{
                            p: {
                                xs: 2,
                                md: 3,
                            },
                        }}
                    >
                        <Stack
                            direction={{
                                xs: "column",
                                md: "row",
                            }}
                            spacing={2}
                            sx={{
                                alignItems: {
                                    xs: "stretch",
                                    md: "center",
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
                                    Content status
                                </Typography>

                                <Typography
                                    color="text.secondary"
                                    sx={{ mt: 0.5 }}
                                >
                                    Review where the current
                                    content came from and when
                                    Icecat last updated it.
                                </Typography>
                            </Box>

                            <Button
                                variant="contained"
                                startIcon={<AutoAwesome />}
                                onClick={() =>
                                    setEnrichmentDialogOpen(
                                        true
                                    )
                                }
                                disabled={
                                    enriching ||
                                    saving ||
                                    product.contentLocked
                                }
                            >
                                {enriching
                                    ? "Enriching..."
                                    : "Enrich from Icecat"}
                            </Button>
                        </Stack>

                        <Divider sx={{ my: 3 }} />

                        <Grid
                            container
                            spacing={3}
                        >
                            <Grid
                                size={{
                                    xs: 12,
                                    sm: 6,
                                    lg: 3,
                                }}
                            >
                                <Typography
                                    variant="body2"
                                    color="text.secondary"
                                >
                                    Content source
                                </Typography>

                                <Chip
                                    label={
                                        product.contentSource ??
                                        "Not assigned"
                                    }
                                    size="small"
                                    variant="outlined"
                                    sx={{ mt: 1 }}
                                />
                            </Grid>

                            <Grid
                                size={{
                                    xs: 12,
                                    sm: 6,
                                    lg: 3,
                                }}
                            >
                                <Typography
                                    variant="body2"
                                    color="text.secondary"
                                >
                                    Content protection
                                </Typography>

                                <Chip
                                    label={
                                        product.contentLocked
                                            ? "Locked"
                                            : "Unlocked"
                                    }
                                    color={
                                        product.contentLocked
                                            ? "warning"
                                            : "default"
                                    }
                                    size="small"
                                    variant="outlined"
                                    sx={{ mt: 1 }}
                                />
                            </Grid>

                            <Grid
                                size={{
                                    xs: 12,
                                    sm: 6,
                                    lg: 3,
                                }}
                            >
                                <Typography
                                    variant="body2"
                                    color="text.secondary"
                                >
                                    Icecat product ID
                                </Typography>

                                <Typography
                                    sx={{
                                        mt: 1,
                                        fontWeight: 700,
                                    }}
                                >
                                    {product.icecatProductId ??
                                        "Not assigned"}
                                </Typography>
                            </Grid>

                            <Grid
                                size={{
                                    xs: 12,
                                    sm: 6,
                                    lg: 3,
                                }}
                            >
                                <Typography
                                    variant="body2"
                                    color="text.secondary"
                                >
                                    Last synchronized
                                </Typography>

                                <Typography
                                    sx={{
                                        mt: 1,
                                        fontWeight: 700,
                                    }}
                                >
                                    {formatDate(
                                        product.icecatLastSynced
                                    )}
                                </Typography>
                            </Grid>

                            <Grid size={{ xs: 12 }}>
                                <Typography
                                    variant="body2"
                                    color="text.secondary"
                                >
                                    Icecat product name
                                </Typography>

                                <Typography
                                    sx={{
                                        mt: 1,
                                        fontWeight: 700,
                                    }}
                                >
                                    {product.icecatName ??
                                        "Not assigned"}
                                </Typography>
                            </Grid>
                        </Grid>
                    </Paper>

                    <AdminProductImageManager
                        productId={product.id}
                        initialImages={product.images}
                        disabled={saving || enriching}
                    />

                    <Paper
                        variant="outlined"
                        sx={{
                            p: {
                                xs: 2,
                                md: 3,
                            },
                        }}
                    >
                        <Typography
                            variant="h5"
                            component="h2"
                            sx={{
                                fontWeight: 800,
                            }}
                        >
                            Editable content
                        </Typography>

                        <Typography
                            color="text.secondary"
                            sx={{ mt: 0.5 }}
                        >
                            Edit the webshop
                            description and raw
                            specification data.
                        </Typography>

                        <Divider sx={{ my: 3 }} />

                        <Stack spacing={3}>
                            <TextField
                                fullWidth
                                multiline
                                minRows={5}
                                label="Description"
                                value={
                                    form.description
                                }
                                onChange={(event) =>
                                    setForm(
                                        (current) => ({
                                            ...current,
                                            description:
                                                event
                                                    .target
                                                    .value,
                                        })
                                    )
                                }
                                disabled={saving || enriching}
                                helperText="The product description displayed in the webshop."
                            />

                            <TextField
                                fullWidth
                                multiline
                                minRows={10}
                                label="Specifications JSON"
                                value={
                                    form.specificationsJson
                                }
                                onChange={(event) => {
                                    setForm(
                                        (current) => ({
                                            ...current,
                                            specificationsJson:
                                                event
                                                    .target
                                                    .value,
                                        })
                                    );

                                    setSpecificationsError(
                                        null
                                    );
                                }}
                                onBlur={
                                    validateSpecifications
                                }
                                disabled={saving || enriching}
                                error={Boolean(
                                    specificationsError
                                )}
                                helperText={
                                    specificationsError ??
                                    "Specifications must be stored as valid JSON."
                                }
                                sx={{
                                    "& textarea": {
                                        fontFamily:
                                            "monospace",
                                    },
                                }}
                            />

                            <FormControlLabel
                                control={
                                    <Switch
                                        checked={
                                            form.contentLocked
                                        }
                                        onChange={(
                                            event
                                        ) =>
                                            setForm(
                                                (
                                                    current
                                                ) => ({
                                                    ...current,
                                                    contentLocked:
                                                        event
                                                            .target
                                                            .checked,
                                                })
                                            )
                                        }
                                        disabled={saving || enriching}
                                    />
                                }
                                label="Protect this content from automatic enrichment"
                            />

                            <Alert severity="info">
                                When content protection
                                is enabled, automatic
                                Icecat enrichment will
                                not overwrite this
                                product’s manually
                                maintained content.
                            </Alert>
                        </Stack>

                        <Divider sx={{ my: 3 }} />

                        <Stack
                            direction="row"
                            sx={{
                                justifyContent:
                                    "flex-end",
                            }}
                        >
                            <Button
                                variant="contained"
                                size="large"
                                startIcon={<Save />}
                                onClick={() =>
                                    void handleSave()
                                }
                                disabled={saving || enriching}
                                sx={{
                                    minWidth: 180,
                                }}
                            >
                                {saving
                                    ? "Saving..."
                                    : "Save content"}
                            </Button>
                        </Stack>
                    </Paper>
                </Stack>
            )}

            <ConfirmDialog
                open={enrichmentDialogOpen}
                title="Enrich product from Icecat?"
                description={
                    product
                        ? `Icecat enrichment will update the available content and images for "${product.name}". Manually maintained content may be replaced. This action does not write anything to Rackbeat.`
                        : ""
                }
                confirmLabel="Enrich product"
                loading={enriching}
                onClose={() =>
                    setEnrichmentDialogOpen(false)
                }
                onConfirm={() =>
                    void handleConfirmEnrichment()
                }
            />
        </Box>
    );
};

export default AdminProductContentPage;