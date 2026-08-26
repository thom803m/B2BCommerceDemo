import { ArrowBack } from "@mui/icons-material";
import { Alert, Box, Button, } from "@mui/material";
import { useEffect, useState, } from "react";
import { Link, useLocation, useNavigate, useParams, } from "react-router-dom";
import { getProductById, updateProduct, type ProductWriteRequest, } from "../../../api/productApi";
import ProductForm from "../../../components/products/ProductForm";
import LoadingSpinner from "../../../components/common/LoadingSpinner";
import PageHeader from "../../../components/common/PageHeader";

type ProductNavigationState = {
    productListUrl?: string;
};

const EditProductPage = () => {
    const { id } = useParams();

    const navigate = useNavigate();
    const location = useLocation();

    const [product, setProduct] =
        useState<ProductWriteRequest | null>(
            null
        );

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    const navigationState =
        location.state as
            | ProductNavigationState
            | null;

    const productListUrl =
        navigationState?.productListUrl ??
        "/admin/products";

    const productId = Number(id);

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

                const mappedProduct:
                    ProductWriteRequest = {
                    sku: result.sku,
                    name: result.name,
                    ean: result.ean,
                    basePrice: result.basePrice,
                    availableStock: result.availableStock,
                    brandId:
                        result.brand?.id ??
                        0,
                    categoryId:
                        result.category?.id ??
                        0,
                };

                setProduct(
                    mappedProduct
                );
            } catch (error) {
                console.error(
                    "Failed to load product",
                    error
                );

                setError(
                    "The product could not be loaded. It may no longer exist."
                );
            } finally {
                setLoading(false);
            }
        };

        void loadProduct();
    }, [productId]);

    const handleSubmit = async (
        data: ProductWriteRequest
    ) => {
        if (
            !Number.isInteger(productId) ||
            productId <= 0
        ) {
            return;
        }

        setError(null);

        try {
            await updateProduct(
                productId,
                {
                    sku: data.sku,
                    name: data.name,
                    basePrice: data.basePrice,
                    ean: data.ean,
                    brandId: data.brandId,
                    categoryId: data.categoryId,
                }
            );

            navigate(productListUrl, {
                replace: true,
            });
        } catch (error) {
            console.error(
                "Failed to update product",
                error
            );

            setError(
                "The product could not be updated. Please check the entered information and try again."
            );
        }
    };

    return (
        <Box>
            <PageHeader
                title="Edit product"
                subtitle={
                    product
                        ? `Update the catalogue information for ${product.name}.`
                        : "Update the product's catalogue information."
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

            {loading && (
                <LoadingSpinner text="Loading product..." />
            )}

            {!loading && product && (
                <ProductForm
                    initialData={product}
                    onSubmit={handleSubmit}
                />
            )}
        </Box>
    );
};

export default EditProductPage;