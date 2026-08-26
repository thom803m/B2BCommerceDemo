import { Box, Button, Grid, Pagination, Stack, Typography, } from "@mui/material";
import { Download, Inventory2, RestartAlt, UploadFile, } from "@mui/icons-material";
import { useState, } from "react";
import ProductFilters from "../../components/products/ProductFilters";
import ProductGrid from "../../components/products/ProductGrid";
import ProductImportDialog from "../../components/products/import/ProductImportDialog";
import ProductExportDialog from "../../components/products/export/ProductExportDialog";
import LoadingSpinner from "../../components/common/LoadingSpinner";
import ProductSort from "../../components/products/ProductSort";
import EmptyState from "../../components/common/EmptyState";
import { useProducts } from "../../hooks/products/useProducts";
import { useAuth, } from "../../context/AuthContext";

const ProductGridPage = () => {
    const {
        isAuthenticated,
        isAdmin,
    } = useAuth();

    const [
        exportDialogOpen,
        setExportDialogOpen,
    ] = useState(false);

    const [
        importDialogOpen,
        setImportDialogOpen,
    ] = useState(false);

    const {
        products,
        filters,
        setFilters,
        brands,
        categories,
        optionsLoading,
        loading,
        error,
        applyFilters,
        resetFilters,
        sort,
        setSort,
        page,
        pageSize,
        totalCount,
        pageCount,
        changePage,
        refreshProducts,
    } = useProducts();

    const firstProductNumber =
        totalCount === 0
            ? 0
            : (page - 1) * pageSize + 1;

    const lastProductNumber = Math.min(
        page * pageSize,
        totalCount
    );

    return (
        <Box>
            <Box
                sx={{
                    mb: 5,
                    display: "grid",
                    gridTemplateColumns: {
                        xs: "1fr",
                        xl: "minmax(180px, 1fr) auto minmax(460px, 1fr)",
                    },
                    alignItems: "center",
                    gap: 2,
                }}
            >
                <Box
                    sx={{
                        display: {
                            xs: "none",
                            xl: "block",
                        },
                    }}
                />

                <Stack
                    spacing={3}
                    sx={{
                        mb: 5,
                        alignItems: "center",
                    }}
                >
                    <Box sx={{ textAlign: "center" }}>
                        <Typography
                            variant="h3"
                            component="h1"
                            sx={{
                                fontWeight: 800,
                                lineHeight: 1.1,
                            }}
                        >
                            Products
                        </Typography>

                        <Typography
                            color="text.secondary"
                            sx={{
                                mt: 1,
                                maxWidth: 620,
                            }}
                        >
                            Browse IT products, stock availability and business pricing.
                        </Typography>
                    </Box>

                    <Stack
                        direction={{
                            xs: "column",
                            sm: "row",
                        }}
                        spacing={1.5}
                        useFlexGap
                        sx={{
                            width: {
                                xs: "100%",
                                sm: "auto",
                            },
                            alignItems: "center",
                            justifyContent: "center",
                            flexWrap: {
                                xs: "nowrap",
                                sm: "wrap",
                            },
                        }}
                    >
                        {isAdmin && (
                            <Button
                                variant="outlined"
                                startIcon={<UploadFile />}
                                onClick={() =>
                                    setImportDialogOpen(true)
                                }
                                sx={{
                                    width: {
                                        xs: "100%",
                                        sm: "auto",
                                    },
                                    minWidth: {
                                        sm: 170,
                                    },
                                    whiteSpace: "nowrap",
                                    flexShrink: 0,
                                }}
                            >
                                Import data
                            </Button>
                        )}

                        {isAuthenticated && (
                            <Button
                                variant="outlined"
                                startIcon={<Download />}
                                onClick={() =>
                                    setExportDialogOpen(true)
                                }
                                sx={{
                                    width: {
                                        xs: "100%",
                                        sm: "auto",
                                    },
                                    minWidth: {
                                        sm: 190,
                                    },
                                    whiteSpace: "nowrap",
                                    flexShrink: 0,
                                }}
                            >
                                Export products
                            </Button>
                        )}

                        <Box
                            sx={{
                                width: {
                                    xs: "100%",
                                    sm: 260,
                                },
                                flexShrink: 0,
                            }}
                        >
                            <ProductSort
                                value={sort}
                                onChange={setSort}
                            />
                        </Box>
                    </Stack>
                </Stack>
            </Box>

            <Grid container spacing={3}>
                <Grid size={{ xs: 12, md: 3 }}>
                    <Box
                        sx={{
                            position: {
                                xs: "static",
                                md: "sticky",
                            },
                            top: { md: 96 },
                            alignSelf: "flex-start",
                        }}
                    >
                        <ProductFilters
                            values={filters}
                            brands={brands}
                            categories={categories}
                            optionsLoading={optionsLoading}
                            onChange={setFilters}
                            onApply={applyFilters}
                            onReset={resetFilters}
                        />
                    </Box>
                </Grid>

                <Grid size={{ xs: 12, md: 9 }}>
                    {loading && (
                        <LoadingSpinner text="Loading products..." />
                    )}

                    {!loading && error && (
                        <EmptyState
                            title="Unable to load products"
                            description={error}
                        />
                    )}

                    {!loading &&
                        !error &&
                        products.length === 0 && (
                            <Box
                                sx={{
                                    minHeight: 420,
                                    border: "1px dashed",
                                    borderColor: "divider",
                                    borderRadius: 3,
                                    display: "flex",
                                    alignItems: "center",
                                    justifyContent: "center",
                                    px: 3,
                                    py: 6,
                                    textAlign: "center",
                                    bgcolor: "background.paper",
                                }}
                            >
                                <Stack
                                    spacing={2}
                                    sx={{
                                        maxWidth: 460,
                                        alignItems: "center",
                                    }}
                                >
                                    <Box
                                        sx={{
                                            width: 72,
                                            height: 72,
                                            borderRadius: "50%",
                                            bgcolor: "action.hover",
                                            color: "text.secondary",
                                            display: "flex",
                                            alignItems: "center",
                                            justifyContent: "center",
                                        }}
                                    >
                                        <Inventory2
                                            sx={{ fontSize: 36 }}
                                        />
                                    </Box>

                                    <Typography
                                        variant="h5"
                                        component="h2"
                                        sx={{ fontWeight: 800 }}
                                    >
                                        No products found
                                    </Typography>

                                    <Typography color="text.secondary">
                                        We could not find any products
                                        matching the selected search and
                                        filters.
                                    </Typography>

                                    <Button
                                        variant="outlined"
                                        startIcon={<RestartAlt />}
                                        onClick={resetFilters}
                                    >
                                        Clear filters
                                    </Button>
                                </Stack>
                            </Box>
                        )}

                    {!loading &&
                        !error &&
                        products.length > 0 && (
                            <>
                                <ProductGrid products={products} />

                                <Stack
                                    direction={{
                                        xs: "column",
                                        sm: "row",
                                    }}
                                    spacing={2}
                                    sx={{
                                        mt: 4,
                                        alignItems: "center",
                                        justifyContent:
                                            "space-between",
                                    }}
                                >
                                    <Typography color="text.secondary">
                                        Showing{" "}
                                        <Box
                                            component="span"
                                            sx={{
                                                color: "text.primary",
                                                fontWeight: 700,
                                            }}
                                        >
                                            {firstProductNumber}–
                                            {lastProductNumber}
                                        </Box>{" "}
                                        of{" "}
                                        <Box
                                            component="span"
                                            sx={{
                                                color: "text.primary",
                                                fontWeight: 700,
                                            }}
                                        >
                                            {totalCount}
                                        </Box>{" "}
                                        {totalCount === 1
                                            ? "product"
                                            : "products"}
                                    </Typography>

                                    {pageCount > 1 && (
                                        <Pagination
                                            page={page}
                                            count={pageCount}
                                            onChange={(_, newPage) =>
                                                changePage(newPage)
                                            }
                                            color="primary"
                                            shape="rounded"
                                        />
                                    )}
                                </Stack>
                            </>
                        )}
                </Grid>
            </Grid>

            <ProductExportDialog
                open={exportDialogOpen}
                onClose={() =>
                    setExportDialogOpen(false)
                }
            />

            <ProductImportDialog
                open={importDialogOpen}
                onClose={() =>
                    setImportDialogOpen(false)
                }
                onImportCompleted={
                    refreshProducts
                }
            />
        </Box>
    );
};

export default ProductGridPage;