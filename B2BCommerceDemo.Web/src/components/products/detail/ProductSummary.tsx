import {
    Box,
    Chip,
    Divider,
    Stack,
    Typography,
} from "@mui/material";
import type { Product } from "../../../api/productApi";

type ProductSummaryProps = {
    product: Product;
};

const ProductSummary = ({
    product,
}: ProductSummaryProps) => {
    return (
        <Stack spacing={3}>
            <Box>
                {product.brand?.name && (
                    <Typography
                        variant="overline"
                        component="p"
                        sx={{
                            mb: 1,
                            color: "primary.main",
                            fontWeight: 800,
                            letterSpacing: "0.12em",
                            lineHeight: 1.4,
                        }}
                    >
                        {product.brand.name}
                    </Typography>
                )}

                <Typography
                    component="h1"
                    sx={{
                        maxWidth: 720,
                        fontSize: {
                            xs: "2rem",
                            sm: "2.4rem",
                            md: "2.75rem",
                        },
                        lineHeight: 1.12,
                        fontWeight: 800,
                        letterSpacing: "-0.035em",
                        overflowWrap: "anywhere",
                    }}
                >
                    {product.name}
                </Typography>

                {product.category?.name && (
                    <Box
                        sx={{
                            mt: 2,
                        }}
                    >
                        <Chip
                            label={product.category.name}
                            variant="outlined"
                            size="small"
                            sx={{
                                borderRadius: 2,
                                fontWeight: 600,
                            }}
                        />
                    </Box>
                )}
            </Box>

            <Divider />

            <Box>
                <Typography
                    variant="subtitle1"
                    component="h2"
                    sx={{
                        mb: 2,
                        fontWeight: 800,
                    }}
                >
                    Product information
                </Typography>

                <Stack spacing={1.5}>
                    <ProductMetaRow
                        label="SKU"
                        value={product.sku}
                    />

                    <ProductMetaRow
                        label="EAN"
                        value={product.ean}
                    />

                    <ProductMetaRow
                        label="Manufacturer"
                        value={product.brand?.name}
                    />

                    <ProductMetaRow
                        label="Category"
                        value={product.category?.name}
                    />
                </Stack>
            </Box>
        </Stack>
    );
};

type ProductMetaRowProps = {
    label: string;
    value?: string | null;
};

const ProductMetaRow = ({
    label,
    value,
}: ProductMetaRowProps) => {
    if (!value) {
        return null;
    }

    return (
        <Box
            sx={{
                display: "grid",
                gridTemplateColumns: {
                    xs: "1fr",
                    sm: "130px minmax(0, 1fr)",
                },
                columnGap: 2,
                rowGap: 0.35,
                alignItems: "baseline",
            }}
        >
            <Typography
                variant="body2"
                color="text.secondary"
                sx={{
                    fontWeight: 500,
                }}
            >
                {label}
            </Typography>

            <Typography
                variant="body1"
                sx={{
                    minWidth: 0,
                    fontWeight: 650,
                    overflowWrap: "anywhere",
                }}
            >
                {value}
            </Typography>
        </Box>
    );
};

export default ProductSummary;