import { Alert, Box, Breadcrumbs, Grid, Link, Stack, Typography, } from "@mui/material";
import { Link as RouterLink, useLocation, useParams, } from "react-router-dom";
import { useEffect, useState, } from "react";
import { getProductById, type Product, } from "../../api/productApi";
import LoadingSpinner from "../../components/common/LoadingSpinner";
import ProductGallery from "../../components/products/detail/ProductGallery";
import ProductSummary from "../../components/products/detail/ProductSummary";
import ProductPurchaseCard from "../../components/products/detail/ProductPurchaseCard";
import ProductDescription from "../../components/products/detail/ProductDescription";
import ProductSpecifications from "../../components/products/detail/ProductSpecifications";

const ProductDetailPage = () => {
    const { id } = useParams();

    const [product, setProduct] = useState<Product | null>(null);

    const [loading, setLoading] = useState(true);

    const [error, setError] = useState<string | null>(null);

    const location = useLocation();

    const productListUrl =
        (
            location.state as {
                productListUrl?: string;
            } | null
        )?.productListUrl ?? "/products";

    useEffect(() => {
        const loadProduct = async () => {
            const productId = Number(id);

            if (!Number.isInteger(productId)) {
                setError("The product ID is invalid.");
                setLoading(false);
                return;
            }

            try {
                setLoading(true);
                setError(null);

                const productData =
                    await getProductById(productId);

                setProduct(productData);
            } catch {
                setError(
                    "The product could not be loaded."
                );
            } finally {
                setLoading(false);
            }
        };

        loadProduct();
    }, [id]);

    if (loading) {
        return (
            <LoadingSpinner text="Loading product..." />
        );
    }

    if (error) {
        return (
            <Alert severity="error">
                {error}
            </Alert>
        );
    }

    if (!product) {
        return (
            <Alert severity="warning">
                Product not found.
            </Alert>
        );
    }

    return (
        <Stack spacing={4}>
            <Breadcrumbs aria-label="breadcrumb">
                <Link
                    component={RouterLink}
                    to="/"
                    underline="hover"
                    color="inherit"
                >
                    Home
                </Link>

                <Link
                    component={RouterLink}
                    to={productListUrl}
                    underline="hover"
                    color="inherit"
                >
                    Products
                </Link>

                <Typography
                    color="text.primary"
                    noWrap
                    sx={{ maxWidth: 320 }}
                >
                    {product.name}
                </Typography>
            </Breadcrumbs>

            <Grid container columnSpacing={{ xs: 0, md: 4, lg: 6, }}
                rowSpacing={{ xs: 4, md: 5, }}
                sx={{
                    alignItems: "flex-start",
                }}
            >
                <Grid size={{ xs: 12, md: 5, lg: 4, }}>
                    <ProductGallery product={product} />
                </Grid>

                <Grid size={{ xs: 12, md: 7, lg: 5, }}>
                    <ProductSummary product={product} />
                </Grid>

                <Grid
                    size={{ xs: 12, lg: 3, }}
                    sx={{
                        display: "flex",
                        justifyContent: {
                            xs: "center",
                            lg: "flex-end",
                        },
                    }}
                >
                    <ProductPurchaseCard
                        product={product}
                    />
                </Grid>
            </Grid>

            <Box
                sx={{
                    mt: {
                        xs: 2,
                        md: 4,
                    },
                }}
            >
                <Grid
                    container
                    spacing={{
                        xs: 3,
                        md: 4,
                    }}
                >
                    <Grid size={{ xs: 12, md: 8 }}>
                        <ProductDescription
                            product={product}
                        />
                    </Grid>

                    <Grid size={{ xs: 12, md: 4 }}>
                        <ProductSpecifications
                            product={product}
                        />
                    </Grid>
                </Grid>
            </Box>
        </Stack>
    );
};

export default ProductDetailPage;