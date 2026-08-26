import { ArrowBack } from "@mui/icons-material";
import { Alert, Box, Button, } from "@mui/material";
import { useState } from "react";
import { Link, useLocation, useNavigate, } from "react-router-dom";
import { createProduct, type ProductWriteRequest,
} from "../../../api/productApi";
import ProductForm from "../../../components/products/ProductForm";
import PageHeader from "../../../components/common/PageHeader";

type ProductNavigationState = {
    productListUrl?: string;
};

const CreateProductPage = () => {
    const navigate = useNavigate();
    const location = useLocation();

    const [error, setError] =
        useState<string | null>(null);

    const navigationState =
        location.state as
        | ProductNavigationState
        | null;

    const productListUrl =
        navigationState?.productListUrl ??
        "/admin/products";

    const handleSubmit = async (
        data: ProductWriteRequest
    ) => {
        setError(null);

        try {
            await createProduct(data);

            navigate(productListUrl, {
                replace: true,
            });
        } catch (error) {
            console.error(
                "Failed to create product",
                error
            );

            setError(
                "The product could not be created. Please check the entered information and try again."
            );
        }
    };

    return (
        <Box>
            <PageHeader
                title="Create product"
                subtitle="Add a new product to the webshop catalogue."
                action={
                    <Button
                        component={Link}
                        to={productListUrl}
                        variant="outlined"
                        startIcon={<ArrowBack />}
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

            <ProductForm
                onSubmit={handleSubmit}
            />
        </Box>
    );
};

export default CreateProductPage;