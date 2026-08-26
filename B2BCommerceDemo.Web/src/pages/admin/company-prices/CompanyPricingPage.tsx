import {
    Add,
    DeleteOutlined,
    Edit,
    PriceChange,
    Refresh,
    Save,
} from "@mui/icons-material";
import {
    Alert,
    Box,
    Button,
    Grid,
    MenuItem,
    Paper,
    Snackbar,
    Stack,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    TextField,
    Typography,
} from "@mui/material";
import { type FormEvent, useCallback, useEffect, useMemo, useState, } from "react";
import { createCompanyPrice, deleteCompanyPrice, getCompanyPrices, updateCompanyPrice, type CompanyPrice,} from "../../../api/companyPriceApi";
import { getAdminCompanies, type Company, } from "../../../api/companyApi";
import { getProducts, type Product, } from "../../../api/productApi";
import ConfirmDialog from "../../../components/common/ConfirmDialog";
import EmptyState from "../../../components/common/EmptyState";
import LoadingSpinner from "../../../components/common/LoadingSpinner";
import PageHeader from "../../../components/common/PageHeader";
import { formatPrice } from "../../../utils/formatPrice";

type CompanyPriceFormValues = {
    productId: string;
    companyId: string;
    price: string;
};

const defaultFormValues:
    CompanyPriceFormValues = {
    productId: "",
    companyId: "",
    price: "",
};

const CompanyPricingPage = () => {
    const [prices, setPrices] =
        useState<CompanyPrice[]>([]);

    const [products, setProducts] =
        useState<Product[]>([]);

    const [companies, setCompanies] =
        useState<Company[]>([]);

    const [form, setForm] =
        useState<CompanyPriceFormValues>({
            ...defaultFormValues,
        });

    const [
        editingCompanyPriceId,
        setEditingCompanyPriceId,
    ] = useState<number | null>(null);

    const [editingPrice, setEditingPrice] =
        useState("");

    const [
        companyPriceToDelete,
        setCompanyPriceToDelete,
    ] = useState<CompanyPrice | null>(
        null
    );

    const [loading, setLoading] =
        useState(true);

    const [processing, setProcessing] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState<string | null>(null);

    const loadData = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const [
                companyPrices,
                productResult,
                availableCompanies,
            ] = await Promise.all([
                getCompanyPrices(),
                getProducts({
                    sortBy: "name",
                    sortDirection: "asc",
                    page: 1,
                    pageSize: 1000,
                }),
                getAdminCompanies(),
            ]);

            setPrices(companyPrices);
            setProducts(
                productResult.items
            );
            setCompanies(
                availableCompanies
            );
        } catch (error) {
            console.error(
                "Failed to load company pricing data",
                error
            );

            setError(
                "The company-specific prices could not be loaded. Please try again."
            );
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        void loadData();
    }, [loadData]);

    const productLookup = useMemo(
        () =>
            new Map(
                products.map((product) => [
                    product.id,
                    product,
                ])
            ),
        [products]
    );

    const companyLookup = useMemo(
        () =>
            new Map(
                companies.map((company) => [
                    company.id,
                    company,
                ])
            ),
        [companies]
    );

    const activeCompanies = useMemo(
        () =>
            companies.filter(
                (company) =>
                    company.status ===
                    "Active"
            ),
        [companies]
    );

    const activeProducts = useMemo(
        () =>
            products.filter(
                (product) =>
                    product.isActive !== false
            ),
        [products]
    );

    const sortedPrices = useMemo(
        () =>
            [...prices].sort((a, b) => {
                const companyA =
                    companyLookup.get(
                        a.companyId
                    )?.name ?? "";

                const companyB =
                    companyLookup.get(
                        b.companyId
                    )?.name ?? "";

                const companyComparison =
                    companyA.localeCompare(
                        companyB
                    );

                if (
                    companyComparison !== 0
                ) {
                    return companyComparison;
                }

                const productA =
                    productLookup.get(
                        a.productId
                    )?.name ?? "";

                const productB =
                    productLookup.get(
                        b.productId
                    )?.name ?? "";

                return productA.localeCompare(
                    productB
                );
            }),
        [
            prices,
            companyLookup,
            productLookup,
        ]
    );

    const handleCreate = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        const productId = Number(
            form.productId
        );

        const companyId = Number(
            form.companyId
        );

        const price = Number(form.price);

        if (
            !Number.isInteger(productId) ||
            productId <= 0
        ) {
            setError(
                "Please select a product."
            );
            return;
        }

        if (
            !Number.isInteger(companyId) ||
            companyId <= 0
        ) {
            setError(
                "Please select a company."
            );
            return;
        }

        if (
            !Number.isFinite(price) ||
            price <= 0
        ) {
            setError(
                "The company-specific price must be greater than zero."
            );
            return;
        }

        const alreadyExists =
            prices.some(
                (companyPrice) =>
                    companyPrice.productId ===
                    productId &&
                    companyPrice.companyId ===
                    companyId
            );

        if (alreadyExists) {
            setError(
                "A company-specific price already exists for this company and product."
            );
            return;
        }

        setProcessing(true);
        setError(null);
        setSuccessMessage(null);

        try {
            const createdPrice =
                await createCompanyPrice({
                    productId,
                    companyId,
                    price,
                });

            setPrices((current) => [
                ...current,
                createdPrice,
            ]);

            setForm({
                ...defaultFormValues,
            });

            const product =
                productLookup.get(productId);

            const company =
                companyLookup.get(companyId);

            setSuccessMessage(
                `The company-specific price for ${product?.name ?? "the product"} and ${company?.name ?? "the company"} was created.`
            );
        } catch (error) {
            console.error(
                "Failed to create company price",
                error
            );

            setError(
                "The company-specific price could not be created. It may already exist."
            );
        } finally {
            setProcessing(false);
        }
    };

    const handleStartEdit = (
        companyPrice: CompanyPrice
    ) => {
        setEditingCompanyPriceId(
            companyPrice.id
        );

        setEditingPrice(
            String(companyPrice.price)
        );

        setError(null);
    };

    const handleCancelEdit = () => {
        setEditingCompanyPriceId(null);
        setEditingPrice("");
    };

    const handleSaveEdit = async (
        companyPrice: CompanyPrice
    ) => {
        const price = Number(
            editingPrice
        );

        if (
            !Number.isFinite(price) ||
            price <= 0
        ) {
            setError(
                "The company-specific price must be greater than zero."
            );
            return;
        }

        setProcessing(true);
        setError(null);
        setSuccessMessage(null);

        try {
            const updatedPrice =
                await updateCompanyPrice(
                    companyPrice.id,
                    {
                        price,
                    }
                );

            setPrices((current) =>
                current.map((item) =>
                    item.id ===
                        updatedPrice.id
                        ? updatedPrice
                        : item
                )
            );

            setEditingCompanyPriceId(
                null
            );

            setEditingPrice("");

            setSuccessMessage(
                "The company-specific price was updated successfully."
            );
        } catch (error) {
            console.error(
                "Failed to update company price",
                error
            );

            setError(
                "The company-specific price could not be updated. Please try again."
            );
        } finally {
            setProcessing(false);
        }
    };

    const handleConfirmDelete =
        async () => {
            if (!companyPriceToDelete) {
                return;
            }

            const companyPrice =
                companyPriceToDelete;

            setProcessing(true);
            setError(null);
            setSuccessMessage(null);

            try {
                await deleteCompanyPrice(
                    companyPrice.id
                );

                setPrices((current) =>
                    current.filter(
                        (item) =>
                            item.id !==
                            companyPrice.id
                    )
                );

                setCompanyPriceToDelete(
                    null
                );

                setSuccessMessage(
                    "The company-specific price was deleted successfully."
                );
            } catch (error) {
                console.error(
                    "Failed to delete company price",
                    error
                );

                setCompanyPriceToDelete(
                    null
                );

                setError(
                    "The company-specific price could not be deleted. Please try again."
                );
            } finally {
                setProcessing(false);
            }
        };

    return (
        <Box>
            <PageHeader
                title="Company-specific prices"
                subtitle="Create product prices that override the assigned price group for individual companies."
                action={
                    <Button
                        variant="outlined"
                        startIcon={<Refresh />}
                        onClick={() =>
                            void loadData()
                        }
                        disabled={
                            loading ||
                            processing
                        }
                    >
                        Refresh
                    </Button>
                }
            />

            <Alert
                severity="info"
                sx={{ mb: 3 }}
            >
                A company-specific price
                overrides the normal product
                price and price-group adjustment
                for the selected company.
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

            {loading && (
                <LoadingSpinner text="Loading company-specific prices..." />
            )}

            {!loading && (
                <Stack spacing={4}>
                    <Paper
                        component="form"
                        variant="outlined"
                        onSubmit={
                            handleCreate
                        }
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
                            Add company-specific
                            price
                        </Typography>

                        <Typography
                            color="text.secondary"
                            sx={{
                                mt: 0.5,
                                mb: 3,
                            }}
                        >
                            Select a company and
                            product, then enter the
                            exact price excluding
                            VAT.
                        </Typography>

                        <Grid
                            container
                            spacing={2}
                            sx={{
                                alignItems:
                                    "flex-start",
                            }}
                        >
                            <Grid
                                size={{
                                    xs: 12,
                                    md: 4,
                                }}
                            >
                                <TextField
                                    select
                                    fullWidth
                                    label="Company"
                                    value={
                                        form.companyId
                                    }
                                    onChange={(
                                        event
                                    ) =>
                                        setForm(
                                            (
                                                current
                                            ) => ({
                                                ...current,
                                                companyId:
                                                    event
                                                        .target
                                                        .value,
                                            })
                                        )
                                    }
                                    disabled={
                                        processing
                                    }
                                    helperText="Only active companies can receive new prices."
                                >
                                    <MenuItem
                                        value=""
                                        disabled
                                    >
                                        Select
                                        company
                                    </MenuItem>

                                    {activeCompanies.map(
                                        (
                                            company
                                        ) => (
                                            <MenuItem
                                                key={
                                                    company.id
                                                }
                                                value={
                                                    company.id
                                                }
                                            >
                                                {
                                                    company.name
                                                }
                                            </MenuItem>
                                        )
                                    )}
                                </TextField>
                            </Grid>

                            <Grid
                                size={{
                                    xs: 12,
                                    md: 5,
                                }}
                            >
                                <TextField
                                    select
                                    fullWidth
                                    label="Product"
                                    value={
                                        form.productId
                                    }
                                    onChange={(
                                        event
                                    ) =>
                                        setForm(
                                            (
                                                current
                                            ) => ({
                                                ...current,
                                                productId:
                                                    event
                                                        .target
                                                        .value,
                                            })
                                        )
                                    }
                                    disabled={
                                        processing
                                    }
                                    helperText="Select the product that receives the override."
                                >
                                    <MenuItem
                                        value=""
                                        disabled
                                    >
                                        Select
                                        product
                                    </MenuItem>

                                    {activeProducts.map(
                                        (
                                            product
                                        ) => (
                                            <MenuItem
                                                key={
                                                    product.id
                                                }
                                                value={
                                                    product.id
                                                }
                                            >
                                                {
                                                    product.name
                                                }{" "}
                                                —{" "}
                                                {
                                                    product.sku
                                                }
                                            </MenuItem>
                                        )
                                    )}
                                </TextField>
                            </Grid>

                            <Grid
                                size={{
                                    xs: 12,
                                    md: 3,
                                }}
                            >
                                <TextField
                                    fullWidth
                                    type="number"
                                    label="Price"
                                    value={
                                        form.price
                                    }
                                    onChange={(
                                        event
                                    ) =>
                                        setForm(
                                            (
                                                current
                                            ) => ({
                                                ...current,
                                                price:
                                                    event
                                                        .target
                                                        .value,
                                            })
                                        )
                                    }
                                    disabled={
                                        processing
                                    }
                                    helperText="Price excluding VAT."
                                    slotProps={{
                                        htmlInput: {
                                            min: 0.01,
                                            step: 0.01,
                                        },
                                    }}
                                />
                            </Grid>
                        </Grid>

                        <Stack
                            direction="row"
                            sx={{
                                mt: 3,
                                justifyContent:
                                    "flex-end",
                            }}
                        >
                            <Button
                                type="submit"
                                variant="contained"
                                startIcon={<Add />}
                                disabled={
                                    processing
                                }
                            >
                                {processing
                                    ? "Saving..."
                                    : "Add price"}
                            </Button>
                        </Stack>
                    </Paper>

                    <Box>
                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={1}
                            sx={{
                                mb: 2,
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
                                    Price overrides
                                </Typography>

                                <Typography
                                    color="text.secondary"
                                    sx={{ mt: 0.5 }}
                                >
                                    Review, update or
                                    remove existing
                                    company-specific
                                    prices.
                                </Typography>
                            </Box>

                            <Typography
                                color="text.secondary"
                            >
                                {prices.length}{" "}
                                {prices.length === 1
                                    ? "price"
                                    : "prices"}
                            </Typography>
                        </Stack>

                        {prices.length === 0 ? (
                            <EmptyState
                                title="No company-specific prices"
                                description="There are currently no product price overrides."
                                icon={
                                    <PriceChange />
                                }
                            />
                        ) : (
                            <TableContainer
                                component={
                                    Paper
                                }
                                variant="outlined"
                            >
                                <Table
                                    sx={{
                                        minWidth: 950,
                                    }}
                                >
                                    <TableHead>
                                        <TableRow>
                                            <TableCell>
                                                Company
                                            </TableCell>

                                            <TableCell>
                                                Product
                                            </TableCell>

                                            <TableCell>
                                                SKU
                                            </TableCell>

                                            <TableCell align="right">
                                                Base
                                                price
                                            </TableCell>

                                            <TableCell align="right">
                                                Company
                                                price
                                            </TableCell>

                                            <TableCell align="right">
                                                Actions
                                            </TableCell>
                                        </TableRow>
                                    </TableHead>

                                    <TableBody>
                                        {sortedPrices.map(
                                            (
                                                companyPrice
                                            ) => {
                                                const product =
                                                    productLookup.get(
                                                        companyPrice.productId
                                                    );

                                                const company =
                                                    companyLookup.get(
                                                        companyPrice.companyId
                                                    );

                                                const editing =
                                                    editingCompanyPriceId ===
                                                    companyPrice.id;

                                                return (
                                                    <TableRow
                                                        key={
                                                            companyPrice.id
                                                        }
                                                        hover
                                                    >
                                                        <TableCell>
                                                            <Typography
                                                                sx={{
                                                                    fontWeight: 700,
                                                                }}
                                                            >
                                                                {company?.name ??
                                                                    `Company #${companyPrice.companyId}`}
                                                            </Typography>
                                                        </TableCell>

                                                        <TableCell>
                                                            {product?.name ??
                                                                `Product #${companyPrice.productId}`}
                                                        </TableCell>

                                                        <TableCell>
                                                            {product?.sku ??
                                                                "Unknown"}
                                                        </TableCell>

                                                        <TableCell
                                                            align="right"
                                                            sx={{
                                                                whiteSpace:
                                                                    "nowrap",
                                                            }}
                                                        >
                                                            {product
                                                                ? formatPrice(
                                                                    product.basePrice
                                                                )
                                                                : "—"}
                                                        </TableCell>

                                                        <TableCell
                                                            align="right"
                                                            sx={{
                                                                minWidth: 180,
                                                            }}
                                                        >
                                                            {editing ? (
                                                                <TextField
                                                                    type="number"
                                                                    size="small"
                                                                    value={
                                                                        editingPrice
                                                                    }
                                                                    onChange={(
                                                                        event
                                                                    ) =>
                                                                        setEditingPrice(
                                                                            event
                                                                                .target
                                                                                .value
                                                                        )
                                                                    }
                                                                    disabled={
                                                                        processing
                                                                    }
                                                                    slotProps={{
                                                                        htmlInput:
                                                                        {
                                                                            min: 0.01,
                                                                            step: 0.01,
                                                                        },
                                                                    }}
                                                                    sx={{
                                                                        width: 140,
                                                                    }}
                                                                />
                                                            ) : (
                                                                <Typography
                                                                    sx={{
                                                                        fontWeight: 800,
                                                                        whiteSpace:
                                                                            "nowrap",
                                                                    }}
                                                                >
                                                                    {formatPrice(
                                                                        companyPrice.price
                                                                    )}
                                                                </Typography>
                                                            )}
                                                        </TableCell>

                                                        <TableCell align="right">
                                                            <Stack
                                                                direction="row"
                                                                spacing={0.5}
                                                                sx={{
                                                                    justifyContent:
                                                                        "flex-end",
                                                                }}
                                                            >
                                                                {editing ? (
                                                                    <>
                                                                        <Button
                                                                            size="small"
                                                                            startIcon={
                                                                                <Save />
                                                                            }
                                                                            onClick={() =>
                                                                                void handleSaveEdit(
                                                                                    companyPrice
                                                                                )
                                                                            }
                                                                            disabled={
                                                                                processing
                                                                            }
                                                                        >
                                                                            Save
                                                                        </Button>

                                                                        <Button
                                                                            size="small"
                                                                            color="inherit"
                                                                            onClick={
                                                                                handleCancelEdit
                                                                            }
                                                                            disabled={
                                                                                processing
                                                                            }
                                                                        >
                                                                            Cancel
                                                                        </Button>
                                                                    </>
                                                                ) : (
                                                                    <>
                                                                        <Button
                                                                            size="small"
                                                                            startIcon={
                                                                                <Edit />
                                                                            }
                                                                            onClick={() =>
                                                                                handleStartEdit(
                                                                                    companyPrice
                                                                                )
                                                                            }
                                                                            disabled={
                                                                                processing
                                                                            }
                                                                        >
                                                                            Edit
                                                                        </Button>

                                                                        <Button
                                                                            size="small"
                                                                            color="error"
                                                                            startIcon={
                                                                                <DeleteOutlined />
                                                                            }
                                                                            onClick={() =>
                                                                                setCompanyPriceToDelete(
                                                                                    companyPrice
                                                                                )
                                                                            }
                                                                            disabled={
                                                                                processing
                                                                            }
                                                                        >
                                                                            Delete
                                                                        </Button>
                                                                    </>
                                                                )}
                                                            </Stack>
                                                        </TableCell>
                                                    </TableRow>
                                                );
                                            }
                                        )}
                                    </TableBody>
                                </Table>
                            </TableContainer>
                        )}
                    </Box>
                </Stack>
            )}

            <ConfirmDialog
                open={
                    companyPriceToDelete !==
                    null
                }
                title="Delete company-specific price?"
                description={
                    companyPriceToDelete
                        ? `Deleting this override will return ${companyLookup.get(companyPriceToDelete.companyId)?.name ?? "the company"} to its normal price-group pricing for ${productLookup.get(companyPriceToDelete.productId)?.name ?? "the product"}.`
                        : ""
                }
                confirmLabel="Delete price"
                loading={processing}
                onClose={() =>
                    setCompanyPriceToDelete(
                        null
                    )
                }
                onConfirm={() =>
                    void handleConfirmDelete()
                }
            />

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

export default CompanyPricingPage;