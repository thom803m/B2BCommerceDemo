import { Box, Typography } from "@mui/material";

type ProductInfoProps = {
    name: string;
    sku: string;
    brand?: string | null;
};

const ProductInfo = ({ name, sku, brand }: ProductInfoProps) => {
    return (
        <Box>
            {brand && (
                <Typography
                    variant="overline"
                    sx={{
                        display: "block",
                        mb: 0.5,
                        color: "secondary.main",
                        fontWeight: 800,
                        lineHeight: 1.2,
                        letterSpacing: 1,
                    }}
                >
                    {brand}
                </Typography>
            )}

            <Typography
                variant="h6"
                component="h2"
                title={name}
                sx={{
                    fontWeight: 750,
                    lineHeight: 1.3,

                    display: "-webkit-box",
                    WebkitBoxOrient: "vertical",
                    WebkitLineClamp: 3,
                    overflow: "hidden",

                    minHeight: "3.9em",
                }}
            >
                {name}
            </Typography>

            <Typography
                variant="body2"
                title={sku}
                sx={{
                    mt: 1,
                    color: "text.secondary",
                    overflowWrap: "anywhere",
                }}
            >
                SKU: {sku}
            </Typography>
        </Box>
    );
};

export default ProductInfo;