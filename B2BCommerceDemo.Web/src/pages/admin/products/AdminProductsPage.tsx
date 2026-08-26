import { Add, Refresh, RestartAlt, Search, } from "@mui/icons-material";
import { Alert, Box, Button, Pagination, Paper, Snackbar, Stack, TextField, Typography, } from "@mui/material";
import { type FormEvent, useCallback, useEffect, useState, } from "react";
import { Link, useSearchParams, } from "react-router-dom";
import { deleteProduct, getProducts, type Product, } from "../../../api/productApi";
import AdminProductTable from "../../../components/admin/products/AdminProductTable";
import ConfirmDialog from "../../../components/common/ConfirmDialog";
import EmptyState from "../../../components/common/EmptyState";
import LoadingSpinner from "../../../components/common/LoadingSpinner";
import PageHeader from "../../../components/common/PageHeader";

const pageSize = 20;

const getPageFromSearchParams = (
    searchParams: URLSearchParams
) => {
    const pageValue = Number(
        searchParams.get("page")
    );

    return Number.isInteger(pageValue) &&
        pageValue > 0
        ? pageValue
        : 1;
};

const AdminProductsPage = () => {
    const [products, setProducts] =
        useState<Product[]>([]);

    const [
        searchParams,
        setSearchParams,
    ] = useSearchParams();

    const appliedSearch =
        searchParams.get("search") ?? "";

    const page =
        getPageFromSearchParams(
            searchParams
        );

    const [search, setSearch] =
        useState(appliedSearch);

    const [totalCount, setTotalCount] =
        useState(0);

    const [loading, setLoading] =
        useState(true);

    const [
        deletingProductId,
        setDeletingProductId,
    ] = useState<number | null>(null);

    const [
        productToDelete,
        setProductToDelete,
    ] = useState<Product | null>(null);

    const [error, setError] =
        useState<string | null>(null);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState<string | null>(null);

    useEffect(() => {
        setSearch(appliedSearch);
    }, [appliedSearch]);

    const loadProducts =
        useCallback(async () => {
            setLoading(true);
            setError(null);

            try {
                const result =
                    await getProducts({
                        search:
                            appliedSearch.trim() ||
                            undefined,
                        sortBy: "name",
                        sortDirection: "asc",
                        page,
                        pageSize,
                    });

                setProducts(result.items);
                setTotalCount(
                    result.totalCount
                );
            } catch (error) {
                console.error(
                    "Failed to load admin products",
                    error
                );

                setError(
                    "The products could not be loaded. Please try again."
                );
            } finally {
                setLoading(false);
            }
        }, [appliedSearch, page]);

    useEffect(() => {
        void loadProducts();
    }, [loadProducts]);

    const updateProductSearchParams = (
        searchValue: string,
        pageValue: number
    ) => {
        const nextSearchParams =
            new URLSearchParams();

        const normalizedSearch =
            searchValue.trim();

        if (normalizedSearch) {
            nextSearchParams.set(
                "search",
                normalizedSearch
            );
        }

        if (pageValue > 1) {
            nextSearchParams.set(
                "page",
                String(pageValue)
            );
        }

        setSearchParams(
            nextSearchParams
        );
    };

    const handleSearchSubmit = (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        updateProductSearchParams(
            search,
            1
        );
    };

    const handleResetSearch = () => {
        setSearch("");

        updateProductSearchParams(
            "",
            1
        );
    };

    const handleDelete = (
        product: Product
    ) => {
        setProductToDelete(product);
    };

    const handleConfirmDelete =
        async () => {
            if (!productToDelete) {
                return;
            }

            const product =
                productToDelete;

            setDeletingProductId(
                product.id
            );

            setError(null);

            try {
                await deleteProduct(
                    product.id
                );

                setSuccessMessage(
                    `"${product.name}" was deleted successfully.`
                );

                setProductToDelete(null);

                if (
                    products.length === 1 &&
                    page > 1
                ) {
                    updateProductSearchParams(
                        appliedSearch,
                        page - 1
                    );
                } else {
                    await loadProducts();
                }
            } catch (error) {
                console.error(
                    "Failed to delete product",
                    error
                );

                setProductToDelete(null);

                setError(
                    "The product could not be deleted. It may be used by an existing order."
                );
            } finally {
                setDeletingProductId(null);
            }
        };

    const pageCount = Math.max(
        1,
        Math.ceil(totalCount / pageSize)
    );

    const firstProductNumber =
        totalCount === 0
            ? 0
            : (page - 1) * pageSize + 1;

    const lastProductNumber =
        Math.min(
            page * pageSize,
            totalCount
        );

    const hasActiveSearch =
        appliedSearch.trim().length > 0;

    const productListUrl =
        searchParams.toString()
            ? `/admin/products?${searchParams.toString()}`
            : "/admin/products";

    return (
        <Box>
            <PageHeader
                title="Products"
                subtitle="Manage the webshop catalogue, stock information and product content."
                action={
                    <Button
                        component={Link}
                        to="/admin/products/create"
                        state={{
                            productListUrl,
                        }}
                        variant="contained"
                        startIcon={<Add />}
                    >
                        Create product
                    </Button>
                }
            />

            <Paper
                component="form"
                variant="outlined"
                onSubmit={
                    handleSearchSubmit
                }
                sx={{
                    mb: 3,
                    p: {
                        xs: 2,
                        md: 3,
                    },
                }}
            >
                <Typography
                    variant="h6"
                    component="h2"
                    sx={{
                        mb: 2,
                        fontWeight: 800,
                    }}
                >
                    Search products
                </Typography>

                <Stack
                    direction={{
                        xs: "column",
                        md: "row",
                    }}
                    spacing={1.5}
                    sx={{
                        alignItems: {
                            xs: "stretch",
                            md: "center",
                        },
                    }}
                >
                    <TextField
                        fullWidth
                        label="Search"
                        placeholder="Search by product name, SKU or EAN..."
                        value={search}
                        onChange={(event) =>
                            setSearch(
                                event.target.value
                            )
                        }
                        disabled={loading}
                    />

                    <Button
                        type="submit"
                        variant="contained"
                        startIcon={<Search />}
                        disabled={loading}
                        sx={{
                            minWidth: 120,
                            minHeight: 56,
                        }}
                    >
                        Search
                    </Button>

                    <Button
                        type="button"
                        variant="outlined"
                        startIcon={
                            <RestartAlt />
                        }
                        onClick={
                            handleResetSearch
                        }
                        disabled={
                            loading ||
                            (!search &&
                                !appliedSearch)
                        }
                        sx={{
                            minWidth: 120,
                            minHeight: 56,
                        }}
                    >
                        Reset
                    </Button>

                    <Button
                        type="button"
                        variant="outlined"
                        startIcon={<Refresh />}
                        onClick={() =>
                            void loadProducts()
                        }
                        disabled={loading}
                        sx={{
                            minWidth: 120,
                            minHeight: 56,
                        }}
                    >
                        Refresh
                    </Button>
                </Stack>
            </Paper>

            {error && (
                <Alert
                    severity="error"
                    sx={{ mb: 3 }}
                    action={
                        <Button
                            color="inherit"
                            size="small"
                            onClick={() =>
                                void loadProducts()
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
                <LoadingSpinner text="Loading products..." />
            )}

            {!loading &&
                !error &&
                products.length === 0 && (
                    <EmptyState
                        title={
                            hasActiveSearch
                                ? "No products match your search"
                                : "No products found"
                        }
                        description={
                            hasActiveSearch
                                ? "Try changing or clearing your search."
                                : "There are currently no products in the catalogue."
                        }
                        action={
                            hasActiveSearch ? (
                                <Button
                                    variant="outlined"
                                    startIcon={
                                        <RestartAlt />
                                    }
                                    onClick={
                                        handleResetSearch
                                    }
                                >
                                    Clear search
                                </Button>
                            ) : (
                                <Button
                                    component={Link}
                                    to="/admin/products/create"
                                    state={{
                                        productListUrl,
                                    }}
                                    variant="contained"
                                    startIcon={
                                        <Add />
                                    }
                                >
                                    Create product
                                </Button>
                            )
                        }
                    />
                )}

            {!loading &&
                products.length > 0 && (
                    <>
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
                            <Typography
                                variant="h5"
                                component="h2"
                                sx={{
                                    fontWeight: 800,
                                }}
                            >
                                Product catalogue
                            </Typography>

                            <Typography
                                color="text.secondary"
                            >
                                {totalCount}{" "}
                                {totalCount === 1
                                    ? "product"
                                    : "products"}
                            </Typography>
                        </Stack>

                        <AdminProductTable
                            products={products}
                            deletingProductId={
                                deletingProductId
                            }
                            productListUrl={
                                productListUrl
                            }
                            onDelete={
                                handleDelete
                            }
                        />

                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={2}
                            sx={{
                                mt: 3,
                                alignItems: "center",
                                justifyContent:
                                    "space-between",
                            }}
                        >
                            <Typography
                                color="text.secondary"
                                variant="body2"
                            >
                                Showing{" "}
                                {firstProductNumber}–
                                {lastProductNumber} of{" "}
                                {totalCount}
                            </Typography>

                            {pageCount > 1 && (
                                <Pagination
                                    page={page}
                                    count={pageCount}
                                    color="secondary"
                                    onChange={(
                                        _event,
                                        newPage
                                    ) => {
                                        updateProductSearchParams(
                                            appliedSearch,
                                            newPage
                                        );
                                    }}
                                />
                            )}
                        </Stack>
                    </>
                )}

            <ConfirmDialog
                open={productToDelete !== null}
                title="Delete product?"
                description={
                    productToDelete
                        ? `You are about to permanently delete "${productToDelete.name}". This action cannot be undone.`
                        : ""
                }
                confirmLabel="Delete permanently"
                loading={
                    deletingProductId !== null
                }
                onClose={() =>
                    setProductToDelete(null)
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

export default AdminProductsPage;